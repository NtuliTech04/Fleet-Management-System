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
using System.Configuration;
using System.Text;

namespace MVC_Modal_Crud.Controllers
{
    public class BookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: /Booking/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "BkID", string sortdir = "DESC")
        {
            var records = new PagedList<Booking>();
            ViewBag.filter = filter;
            records.Content = db.Bookings
                        .Where(x => filter == null ||
                                (x.Status.Contains(filter))
                                   || x.Email.Contains(filter))
                        .Include(x => x.UserProfile).Include(x => x.BookDetails.Vehicles)
                        .OrderBy(x => x.BkID /*sort + " " + sortdir*/)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .OrderByDescending(x=>x.BkID)
                        .ToList();

            // Count
            records.TotalRecords = db.Bookings
                         .Where(x => filter == null ||
                               (x.Status.Contains(filter)) || x.Email.Contains(filter))
                         .Include(x=>x.UserProfile).Include(x=>x.BookDetails.Vehicles).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            return View(records);

        }


        // GET: /Booking/Details/5
        public ActionResult Details(int id = 0)
        {
            Booking booking = db.Bookings.Find(id);

            if (booking == null)
            {
                return HttpNotFound();
            }
            var BkDet = db.Bookings.Where(x => x.BkID == booking.BkID)
                .Include(x => x.UserProfile)
                .Include(x => x.BookDetails.Vehicles).FirstOrDefault();

            ViewBag.PickUp = db.UserAddresses.Where(x => x.BookDetID == BkDet.BookDetID).Select(x=>x.PickUp).FirstOrDefault();
            ViewBag.Destination = db.UserAddresses.Where(x => x.BookDetID == BkDet.BookDetID).Select(x=>x.Destination).FirstOrDefault();

            return View("Details", booking);
        }

        // GET: /Booking/Create
        [HttpGet]
        public ActionResult Create(int Id)
        {
            var booking = new Booking();
            var BkUser = db.BookDetails.Where(x => x.BookDetID == Id).Select(x => x.Email).FirstOrDefault();

            booking.BookDetID = Id;
            booking.Email = BkUser;
            
            return PartialView("Create", booking);
        }

        // POST: /Booking/Create
        [HttpPost]
        public ActionResult Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                //Create and Save Data of a Complete Booking

                booking.Status = "Pending";
                booking.Status = "Pending";
                booking.PaymentStatus = "Not Paid";
                booking.DateCreated = DateTime.Today.Date;
                booking.Confirmation = "Not Confirmed";

                db.Bookings.Add(booking);
                db.SaveChanges();

                //Get and Save Booked Vehicle and its Booking Details

                BookedVehicle bookedVehicle = new BookedVehicle();

                var GetBookDet_ID = db.Bookings.Where(x=>x.BookDetID==booking.BookDetID).Select(x=>x.BookDetID).FirstOrDefault();
                var findV = db.BookDetails.Where(x => x.BookDetID == GetBookDet_ID).FirstOrDefault();

                bookedVehicle.BookID = booking.BkID;
                bookedVehicle.VehicleID = findV.VehicleID;
                bookedVehicle.HireDate = findV.VacationDate;
                bookedVehicle.ReturnDate = findV.ReturnDate;
                bookedVehicle.Status = "Not Yet Specified";
                bookedVehicle.Driver = "Not Yet Assigned";
           
                db.BookedVehicles.Add(bookedVehicle);
                db.SaveChanges();

                if (booking.PayMethod == "Credit Card") 
                {
                    return RedirectToAction("Secure_Payment", new { id = booking.BkID });
                }
                else
                {
                    return RedirectToAction("HireSuccess", "Bookings");
                }
            }
            return View(booking);
        }

        // GET: /Booking/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var booking = db.Bookings.Find(id);

            if (booking == null)
            {
                return HttpNotFound();
            }
            var BkDet = db.Bookings.Where(x => x.BkID == booking.BkID).Include(x => x.UserProfile).Include(x => x.BookDetails.Vehicles).FirstOrDefault();

            return View("Edit", booking);
        }


        // POST: /Booking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                //return Json(new { success = true });
            }
            return RedirectToAction("Index", "Bookings", booking);
            //return PartialView("Edit", Booking);
        }

        //
        // GET: /Booking/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            var BkDet = db.Bookings.Where(x => x.BkID == booking.BkID).Include(x => x.UserProfile).Include(x => x.BookDetails.Vehicles).FirstOrDefault();

            return View("Delete", booking);
        }


        //
        // POST: /Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            //return Json(new { success = true });
            return RedirectToAction("Index", "Bookings");
        }

        public ActionResult Secure_Payment(int id)
        {
            var book = db.Bookings.Find(id);

            ViewBag.Booking = book;
            ViewBag.Account = db.UserProfiles.Find(book.Email);
            ViewBag.Total = db.BookDetails.Where(x => x.BookDetID == book.BookDetID).Select(x => x.Total).FirstOrDefault();

            string tot = book.BookDetails.Total.ToString();

            return Redirect(PaymentLink(tot, "Vehicle Hire Payment | Book No: " + book.BkID, book.BkID));
        }

        public string PaymentLink(string totalCost, string paymentSubjetc, int BkID)
        {
            string paymentMode = ConfigurationManager.AppSettings["PaymentMode"], site, merchantId, merchantKey, returnUrl;

            site = "https://sandbox.payfast.co.za/eng/process?";
            merchantId = "10022900";
            merchantKey = "qq34viiias2on";
            returnUrl = "https://grp61fleetmanagement.azurewebsites.net/Bookings/HireSuccess/";

            var stringBuilder = new StringBuilder();
         
            stringBuilder.Append("merchant_id=" + HttpUtility.HtmlEncode(merchantId));
            stringBuilder.Append("&merchant_key=" + HttpUtility.HtmlEncode(merchantKey));
            stringBuilder.Append("&return_url= " + HttpUtility.HtmlEncode("https://grp61fleetmanagement.azurewebsites.net/Bookings/HireSuccess/" + BkID));
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

        public ActionResult HireSuccess(int ? id)
        {
            if (id != null)
            {
                var book = db.Bookings.Find(id);
                try
                {
                    
                    book.PaymentStatus = "Paid";
                    db.SaveChanges();

                }
                catch (Exception ex) { }
            }
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
