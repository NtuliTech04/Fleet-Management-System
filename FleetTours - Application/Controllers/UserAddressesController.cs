using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FleetTours___Application.Models;

namespace FleetTours___Application.Controllers
{
    public class UserAddressesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserAddresses
        public ActionResult Index()
        {
            var userAddresses = db.UserAddresses.Include(u => u.BookDetail).Include(u => u.UserProfile);
            return View(userAddresses.ToList());
        }

        // GET: UserAddresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAddress userAddress = db.UserAddresses.Find(id);
            if (userAddress == null)
            {
                return HttpNotFound();
            }
            return View(userAddress);
        }

        // GET: UserAddresses/Create
        public ActionResult Create(int Id)
        {
            var Username = db.UserProfiles.Where(x => x.Email == User.Identity.Name).Select(x => x.Email).FirstOrDefault();

            UserAddress addresses = new UserAddress();
            if (Id == 0)
            {
                return RedirectToAction("CarsForHire");
            }
            if (Username == null)
            {
                return RedirectToAction("Login", "Account");
            }
            addresses.BookDetID = Id;
            addresses.Email = Username;

            ViewBag.BookDetID = new SelectList(db.BookDetails, "BookDetID", "BookDetID");
            ViewBag.Email = new SelectList(db.UserProfiles, "Email", "FirstName");
            return View(addresses);
        }

        // POST: UserAddresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AddressID,PickUp,Destination,Email,BookDetID")] UserAddress userAddress)
        {
            if (ModelState.IsValid)
            {
                db.UserAddresses.Add(userAddress);
                db.SaveChanges();
                return RedirectToAction("Confirmation","CarHire", new { Id = userAddress.BookDetID } );
            }

            ViewBag.BookDetID = new SelectList(db.BookDetails, "BookDetID", "Vacation", userAddress.BookDetID);
            ViewBag.Email = new SelectList(db.UserProfiles, "Email", "FirstName", userAddress.Email);
            return View(userAddress);
        }
    
        // GET: UserAddresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAddress userAddress = db.UserAddresses.Find(id);
            if (userAddress == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookDetID = new SelectList(db.BookDetails, "BookDetID", "Vacation", userAddress.BookDetID);
            ViewBag.Email = new SelectList(db.UserProfiles, "Email", "FirstName", userAddress.Email);
            return View(userAddress);
        }

        // POST: UserAddresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AddressID,PickUp,Destination,Email,BookDetID")] UserAddress userAddress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userAddress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookDetID = new SelectList(db.BookDetails, "BookDetID", "Vacation", userAddress.BookDetID);
            ViewBag.Email = new SelectList(db.UserProfiles, "Email", "FirstName", userAddress.Email);
            return View(userAddress);
        }

        // GET: UserAddresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAddress userAddress = db.UserAddresses.Find(id);
            if (userAddress == null)
            {
                return HttpNotFound();
            }
            return View(userAddress);
        }

        // POST: UserAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserAddress userAddress = db.UserAddresses.Find(id);
            db.UserAddresses.Remove(userAddress);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
