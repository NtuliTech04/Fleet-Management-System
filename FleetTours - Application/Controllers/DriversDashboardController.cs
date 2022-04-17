using FleetTours___Application.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using FleetTours___Application.BusinessLogic;
using System.IO;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using ZXing;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FleetTours___Application.Controllers
{
    [Authorize(Roles = "Driver")]

    public class DriversDashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DriversDashboard
        [HttpGet]
        public ActionResult DriverProfile()
        {
            var getProfile = db.Drivers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

            if (getProfile == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(getProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DriverProfile(Driver driver, HttpPostedFileBase File)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (ModelState.IsValid)
                {
                    if (File != null)
                    {
                        driver.DriverImage = new byte[File.ContentLength];
                        File.InputStream.Read(driver.DriverImage, 0, File.ContentLength);
                    }
                    db.Entry(driver).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("DriverProfile");
                }
                return View();
            }
        }

        public ActionResult RemovePhoto()
        {
            var remove = db.Drivers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            remove.DriverImage = null;
            db.SaveChanges();

            return RedirectToAction("DriverProfile");
        }

        public ActionResult Index()
        {
            Driver getDriver = db.Drivers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            var AssPic = db.AssignDrivers.Where(x => x.SelectedDriver == getDriver.DriverID).Include(x => x.Booking).Include(x => x.Booking.UserProfile).Include(x => x.Booking.BookDetails).OrderByDescending(x=>x.BkID).ToList();
            
            return View(AssPic);
        }

        public ActionResult PickUpLocation(int id)
        {
            var book = db.Bookings.Find(id);

            var PickAddress = db.UserAddresses.Where(x => x.BookDetID == book.BookDetID).Select(x => x.PickUp).FirstOrDefault();

            ViewBag.PickUp = PickAddress;

            if (book.Status == "Processed")
            {
                book.Status = "On Pick-Up";
            }

            db.SaveChanges();
            return View(book);
        }

        public ActionResult ConfirmPickUp(int id)
        {
            Session["BkID"] = id;
            var bkdetID = db.Bookings.Where(x => x.BookDetID == id).FirstOrDefault();

            string url = "<a href=" + "https://localhost:44320/CarHire/Confirmation/" + bkdetID.BookDetID + ">Click Here" + "</a>";
            var book = db.Bookings.Find(id);
            var getName = db.Bookings.Where(x => x.BkID == id).Include(x => x.UserProfile).FirstOrDefault();
            var client = new SendGridClient("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");
            var from = new EmailAddress("nkonzo144@gmail.com", "Fleet Tours Transport Services");
            var subject = "Book " + id + " | Pick-Up Confirmation";

            var to = new EmailAddress(book.Email);
            var htmlContent = "Hi " + getName.UserProfile.FirstName +" "+ "<br/><br/>" + "Your Driver has arrived for Pick-Up. Please click on this link to confirm " + url;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = client.SendEmailAsync(msg);

            if (book.Status == "On Pick-Up")
            {
                book.Status = "Pick-Up Point";
            }
            db.SaveChanges();
            book.Confirmation = GenerateQRCode("Arrival Confirmed" + id);
            return View(book);
        }

        public ActionResult DriveToDestination(int id)
        {
            var book = db.Bookings.Find(id);

            var PickAddress = db.UserAddresses.Where(x => x.BookDetID == book.BookDetID).Select(x => x.PickUp).FirstOrDefault();
            var Destination = db.UserAddresses.Where(x => x.BookDetID == book.BookDetID).Select(x => x.Destination).FirstOrDefault();

            ViewBag.PickUp = PickAddress;
            ViewBag.Destination = Destination;

            if (book.Status == "Pick-Up Point")
            {
                book.Status = "Destination-Point";
            }

            db.SaveChanges();
            return View(book);
        }

        private string GenerateQRCode(string qrcodeText)
        {

            string folderPath = "~/QRCode/";
            string imagePath = "~/QRCode/QrCode" + Session["BkID"] + ".jpg";

            // create new Directory if not exist
            if (!Directory.Exists(Server.MapPath(folderPath)))
            {
                Directory.CreateDirectory(Server.MapPath(folderPath));
            }

            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            var result = barcodeWriter.Write(qrcodeText);

            string barcodePath = Server.MapPath(imagePath);
            var barcodeBitmap = new Bitmap(result);
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return imagePath;
        }

        public ActionResult RideDetails(int id)
        {
            var RideDetails = db.Rides.Where(x => x.RideID == id)
                .Include(x=>x.UserProfile)
                .Include(x=>x.RideAddress).FirstOrDefault();

            return PartialView(RideDetails);
        }

        public ActionResult AcceptRide(int id)
        {
            var RideDetails = db.Rides.Where(x => x.RideID == id).FirstOrDefault();
            var GetDriver = db.Drivers.Where(x => x.Email == User.Identity.Name).Select(x => x.DriverID).FirstOrDefault();

            if (GetDriver != 0) 
            {
                var GetCar = db.Vehicles.Where(x => x.DriverID == GetDriver).Select(x => x.VehicleID).FirstOrDefault();

                if (RideDetails.RideStatus == "Not Accepted" || RideDetails.RideStatus == "Canceled") 
                {
                    RideDetails.RideStatus = "Accepted";
                    RideDetails.DriverID = GetDriver;
                    RideDetails.VehicleID = GetCar;
                }
            }
            db.SaveChanges();

            return RedirectToAction("PickUpRider", new { id = RideDetails.RideID });
        }

        public ActionResult CancelRide(int id)
        {
            var cancel = db.Rides.Where(x => x.RideID == id).FirstOrDefault();

            cancel.DriverID = 0;
            cancel.VehicleID = 0;
            cancel.RideStatus = "Canceled";

            db.SaveChanges();

            return RedirectToAction("Index", "Rides");
        }

        public ActionResult AcceptedRides()
        {
            var driver = db.Drivers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

                var acceptRec = db.Rides.Where(x => x.DriverID == driver.DriverID)
                    .OrderByDescending(x=>x.RideID)
                    .Include(x=>x.UserProfile)
                    .Include(x=>x.RideAddress).ToList();

                foreach (var item in acceptRec)
                {
                    ViewBag.FullName = item.UserProfile.FirstName + " " + item.UserProfile.LastName;
                }

                return View(acceptRec);
        }

        public ActionResult PickUpRider(int id)
        {
            var getLoc = db.Rides.Where(x => x.RideID == id).Include(x=>x.RideAddress).FirstOrDefault();
            return View(getLoc);
        }
        public ActionResult OnPickUp(int id)
        {
            var ride = db.Rides.Find(id);
            var getName = db.Rides.Where(x => x.RideID == id).Include(x => x.UserProfile).FirstOrDefault();
            var client = new SendGridClient("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");
            var from = new EmailAddress("nkonzo144@gmail.com", "Fleet Tours Transport Services");
            var subject = "Ride No. " + id + " | Driver Arrived";

            var to = new EmailAddress(ride.Email);
            var htmlContent = "Hi " + getName.UserProfile.FirstName + " " + "<br/><br/>" + "Your driver has arrived for Pick-Up. Thank you for choosing us." + "<br/><br/>" + "Travel Safe And Enjoy Your Ride!";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = client.SendEmailAsync(msg);

            if (ride.RideStatus == "Accepted")
            {
                ride.RideStatus = "Driver Arrived";
            }
            db.SaveChanges();

            return RedirectToAction("PickUpRider", new { id = ride.RideID });
        }

        public ActionResult BeginRide(int id)
        {
            var getRoute = db.Rides.Where(x => x.RideID == id).Include(x => x.RideAddress).FirstOrDefault();

            getRoute.RideStatus = "In Progress";
            db.SaveChanges();

            return View(getRoute);
        }

        public ActionResult RideComplete(int id)
        {
            var complete = db.Rides.Where(x => x.RideID == id).FirstOrDefault();

            if (complete.PaymentMethod == "Cash")
            {
                complete.PaymentStatus = "Paid";
            }
            complete.RideStatus = "Completed";

            db.SaveChanges();
            return RedirectToAction("Index", "Rides");
        }



    }
}