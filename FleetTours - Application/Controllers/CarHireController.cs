using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FleetTours___Application.Models;
using System.Threading.Tasks;
using System.Text;
using Bytescout.BarCodeReader;
using System.Configuration;

namespace FleetTours___Application.Controllers
{
    [Authorize]
    public class CarHireController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult CarsForHire(string filter = null, int page = 1, int pageSize = 5, string sort = "VehicleID", string sortdir = "DESC")
        {
            var records = new PagedList<Vehicle>();
            ViewBag.filter = filter;
            records.Content = db.Vehicles
                        .Where(x => filter == null || (x.Model.Contains(filter)) || x.VehicleMake.Contains(filter))
                        .Where(x=>x.Duty=="Trips/Hire")
                        .OrderBy(x => x.VehicleID)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            // Count
            records.TotalRecords = db.Vehicles
                            .Where(x => filter == null ||
                            (x.Model.Contains(filter)) || x.VehicleMake.Contains(filter))
                            .Where(x=>x.Duty=="Trips/Hire")
                            .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            return View(records);
        }

        [HttpGet]
        public ActionResult Book(int Id)
        {
            Session["VehicleID"] = Id;
            var Username = db.UserProfiles.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            var GetVehicle = db.Vehicles.Where(x => x.VehicleID == Id).FirstOrDefault();

            BookDetail book = new BookDetail();

            if (Id == 0) 
            {
                return RedirectToAction("CarsForHire");
            }
            if (Username == null)
            {
                return RedirectToAction("Login", "Account");
            }
            book.VehicleID = Id;
            book.Email = Username.Email;
            book.FullName = Username.FirstName +" "+ Username.LastName;
            book.RSAID = Username.SAID;
            book.BookedVehicle = GetVehicle.VehicleMake + " " + GetVehicle.Model + " (" + GetVehicle.Type + ")";
            return View(book);
        }
        [HttpPost]
        public ActionResult Book(BookDetail book)
        {

            if (book.VacationDate.Date < DateTime.Today.Date || book.VacationDate.Date == DateTime.Today.Date)
            {
                ViewBag.WrongDate = "You cannot select the current or behind date. Please select a date that is ahead of today.";
                return View(book);
            }

            if (book.HirePeriod <= 0 || book.Passengers <= 0) 
            {
                book.HirePeriod = 1;
                ViewBag.PassengerError = "Number of passengers must be greater than zero.";
                return View(book);
            }

            book.Total = book.CalcTotal();
            book.ReturnDate = book.VacationDate.AddDays(book.HirePeriod);

            var GetBooked = db.BookedVehicles.ToList();
            var VCapacity = db.Vehicles.Where(x => x.VehicleID == book.VehicleID).FirstOrDefault();

            foreach (var vehicle in GetBooked.Where(x => x.VehicleID == book.VehicleID))
            {
         
                if(book.VacationDate>=vehicle.HireDate && book.VacationDate <= vehicle.ReturnDate)
                {
                    ViewBag.DateConflict = "Date Conflict - This vehicle has already been booked on this chosen date.";
                    ViewBag.Help = "Please refer to the calender before selecting a hire date.";
                    return View(book);
                }
                if (book.ReturnDate >= vehicle.HireDate && book.ReturnDate <= vehicle.ReturnDate) 
                {
                    ViewBag.DateConflict = "Date Conflict - The date at which your trip ends conflicted with another hiring period intervals.";
                    ViewBag.Help = "Help - Please refer to the calender and modifiy your hire period.";
                    return View(book);
                }
            }
            if (book.Passengers >= VCapacity.Capacity)
            {
                ViewBag.ErrorCap = "The entered number of passengers exceeded the maximum capacity of this Vehicle.";
                ViewBag.Help = "Help - you can reduce the number of passengers or select another car.";
                return View(book);
            }

            db.BookDetails.Add(book);
            db.SaveChanges();

            return RedirectToAction("Create", "UserAddresses", new { Id = book.BookDetID });
        }

        public ActionResult Confirmation(int Id)
        {
            var BKDetails = db.BookDetails.Find(Id);
            var vehicle = db.Vehicles.Where(x => x.VehicleID == BKDetails.VehicleID).FirstOrDefault();
            var address = db.UserAddresses.Where(x => x.BookDetID == Id).OrderByDescending(x=>x.AddressID).FirstOrDefault();

            ViewBag.PickUp = address.PickUp;
            ViewBag.Destination = address.Destination;

            var book = db.Bookings.Where(x => x.BookDetID == Id).FirstOrDefault();

            if (book != null)
            {
                ViewBag.BkID = book.BkID;
                ViewBag.Status = book.Status;
            }

            return View(BKDetails);
        }

        public ActionResult PickUpConfirm(int id)
        {
            var bookCon = db.Bookings.Where(x => x.BkID == id).FirstOrDefault();
            bookCon.Confirmation = "Confirmed";
            db.SaveChanges();

            return RedirectToAction("Success");
        }

        public JsonResult GetEvents()
        {
            int vID = Convert.ToInt32(Session["VehicleID"]);
            using (ApplicationDbContext dc = new ApplicationDbContext())
            {
                var events = dc.BookedVehicles.Where(x => x.VehicleID == vID).ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public ActionResult TrackBook()
        {
            var bookings = new List<Booking>();
            BookDetail booked = new BookDetail();
            var GetCustomer = db.Bookings.Where(x => x.Email == User.Identity.Name).ToList();

            if (User.Identity.IsAuthenticated)
            {
                foreach (var book in GetCustomer)
                {
                    bookings = db.Bookings.Where(x => x.Email == book.Email)
                        .Include(x => x.BookDetails)
                        .Include(x => x.UserProfile)
                        .Include(x => x.BookDetails.Vehicles).OrderByDescending(x=>x.BkID).ToList();

                    var GetVehicle = db.Vehicles.Where(x => x.VehicleID == book.BookDetails.VehicleID).FirstOrDefault();
                    ViewBag.BookedVehicle = GetVehicle.VehicleMake + " " + GetVehicle.Model;

                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
           
            return View(bookings);
        }

        public ActionResult GetDriver(int id)
        {
            var dID = db.AssignDrivers.Where(x => x.BkID == id).Select(x => x.SelectedDriver).FirstOrDefault();
            var driver = db.Drivers.Find(dID);

            return PartialView(driver);
        }

        public ActionResult GetVehicle(int id)
        {
            var vehicle = db.Vehicles.Where(x => x.VehicleID == id).FirstOrDefault();
            return PartialView(vehicle);
        }

        public ActionResult QRScanner(int id)
        {
            Session["BkID"] = id;
            return View();
        }
        public ActionResult Success()
        {
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DecodeQR(string image, string type)
        {
            var id = Session["BkID"];
            StringBuilder send = new StringBuilder();

            byte[] bitmapArrayOfBytes = Convert.FromBase64String(image);
            Reader reader = new Reader();
            reader.BarcodeTypesToFind = Barcode.GetBarcodeTypeToFindFromCombobox(type);
            reader.ReadFromMemory(bitmapArrayOfBytes);

            try
            {
                lock (send)
                {
                    if (reader.FoundBarcodes != null)
                    {
                        foreach (FoundBarcode barcode in reader.FoundBarcodes)
                        {
                            using (ApplicationDbContext db = new ApplicationDbContext())
                            {
                                var book = db.Bookings.Find(id);

                                if (barcode.Value == "Arrival Confirmed" + id)
                                {
                                    book.Confirmation = "Confirmed";
                                    db.SaveChanges();

                                    send.AppendLine(String.Format("{0} : {1}", barcode.Type, barcode.Value));
                                    return Json(new { d = send.ToString() });
                                }
                                else
                                {
                                    //send.AppendLine(String.Format("Oops, Confirmation Failed.\nPlease Try Again..."));
                                    return View("Redirect to Error page.");
                                }
                            }
                        }
                    }
                    return Json(new { d = send.ToString() });
                }
            }
            catch (Exception ex)
            {
                return Json(new { d = ex.Message + "\r\n" + ex.StackTrace });
            }
        }


        // Uber Like Ride Requests
        [HttpGet]
        public ActionResult RequestRide()
        {
            RideAddress rideAddress = new RideAddress();

            var GetUser = db.UserProfiles.Where(x => x.Email == User.Identity.Name).Select(x=>x.Email).FirstOrDefault();

            rideAddress.Email = GetUser;

            return View(rideAddress);
        }

        [HttpPost]
        public ActionResult RequestRide(RideAddress rideAddress)
        {
            if (ModelState.IsValid)
            {
                db.RideAddresses.Add(rideAddress);
                db.SaveChanges();
                return RedirectToAction("BookRide");
            }

            return View(rideAddress);
        }

        [HttpGet]
        public ActionResult BookRide(int? id)
        {
            Ride ride = new Ride();
            var route = db.RideAddresses.Where(x => x.Email == User.Identity.Name).OrderByDescending(x => x.RadID).FirstOrDefault();

            if (route != null)
            {
                ViewBag.Start = route.PickUp;
                ViewBag.End = route.Destination;
                ride.RadID = route.RadID;
                ride.Email = route.Email;
            }

            if (id != null)
            {
                var details = db.Rides.Where(x => x.RideID == id).FirstOrDefault();
                return View(details);
            }

            return View(ride);
        }

        [HttpPost]
        public ActionResult BookRide(Ride ride)
        {
            if (ModelState.IsValid)
            {
                double i = 0.0;
                while (i <= ride.Distance)
                {
                    ride.BasicCost = i * 10;
                    i++;
                }
                ride.TotalCost = (ride.BasicCost - ride.Discounts);
                ride.PaymentStatus = "Not Paid";
                ride.RideStatus = "Not Accepted";

                db.Rides.Add(ride);
                db.SaveChanges();

                    return RedirectToAction("BookRide", new { id = ride.RideID });
            }
            return View(ride);
        }

        public ActionResult RequestSuccess(int? id)
        {
            if (id != null)
            {
                var ride = db.Rides.Find(id);
                try
                {
                    if (ride.PaymentMethod == "Credit Card") 
                    {
                        ride.PaymentStatus = "Paid";
                        db.SaveChanges();
                    }
                }
                catch (Exception ex) { }
            }
            return View();
        }

        public ActionResult MyRides()
        {
            var myrides = db.Rides.Where(x => x.Email == User.Identity.Name)
                .OrderByDescending(x => x.RideID)
                .Include(x=>x.UserProfile)
                .Include(x=>x.RideAddress).ToList();

            return View(myrides);
        }

        public ActionResult Secure_Payment(int id)
        {
            var book = db.Rides.Find(id);

            ViewBag.Booking = book;
            ViewBag.Account = db.UserProfiles.Find(book.Email);
            ViewBag.Total = db.Rides.Where(x => x.RideID == book.RideID).Select(x => x.TotalCost).FirstOrDefault();

            string tot = book.TotalCost.ToString();

            return Redirect(PaymentLink(tot, "Ride Request Payment | Request No: " + book.RideID, book.RideID));
        }

        public string PaymentLink(string totalCost, string paymentSubjetc, int BkID)
        {
            string paymentMode = ConfigurationManager.AppSettings["PaymentMode"], site, merchantId, merchantKey, returnUrl;

            site = "https://sandbox.payfast.co.za/eng/process?";
            merchantId = "10022900";
            merchantKey = "qq34viiias2on";
            returnUrl = "https://grp61fleetmanagement.azurewebsites.net/CarHire/RequestSuccess/";

            var stringBuilder = new StringBuilder();

            stringBuilder.Append("merchant_id=" + HttpUtility.HtmlEncode(merchantId));
            stringBuilder.Append("&merchant_key=" + HttpUtility.HtmlEncode(merchantKey));
            stringBuilder.Append("&return_url= " + HttpUtility.HtmlEncode("https://grp61fleetmanagement.azurewebsites.net/CarHire/RequestSuccess/" + BkID));
            //stringBuilder.Append("cancel_url" + HttpUtility.HtmlEncode("https://grp61fleetmanagement.azurewebsites.net/Home/Payment_Cancelled/" + order_id));
            //stringBuilder.Append("notify_url" + HttpUtility.HtmlEncode(ConfigurationManager.AppSettings["PF_NotifyURL"]));

            string amt = totalCost;
            amt = amt.Replace(",", ".");

            stringBuilder.Append("&amount=" + HttpUtility.HtmlEncode(amt));
            stringBuilder.Append("&item_name=" + HttpUtility.HtmlEncode(paymentSubjetc));
            stringBuilder.Append("&email_confirmation=" + HttpUtility.HtmlEncode("1"));
            stringBuilder.Append("&confirmation_address=" + HttpUtility.HtmlEncode(ConfigurationManager.AppSettings["PF_ConfirmationAddress"]));

            return (site + stringBuilder);
        }
    }
}