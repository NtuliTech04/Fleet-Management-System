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
    public class RideAddressesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RideAddresses
        public ActionResult Index()
        {
            var rideAddresses = db.RideAddresses.Include(r => r.UserProfile);
            return View(rideAddresses.ToList());
        }

        // GET: RideAddresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RideAddress rideAddress = db.RideAddresses.Find(id);
            if (rideAddress == null)
            {
                return HttpNotFound();
            }
            return View(rideAddress);
        }

        // GET: RideAddresses/Create
        public ActionResult Create()
        {
            ViewBag.Email = new SelectList(db.UserProfiles, "Email", "FirstName");
            return View();
        }

        // POST: RideAddresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RadID,PickUp,Destination,Email")] RideAddress rideAddress)
        {
            if (ModelState.IsValid)
            {
                db.RideAddresses.Add(rideAddress);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Email = new SelectList(db.UserProfiles, "Email", "FirstName", rideAddress.Email);
            return View(rideAddress);
        }

        // GET: RideAddresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RideAddress rideAddress = db.RideAddresses.Find(id);
            if (rideAddress == null)
            {
                return HttpNotFound();
            }
            ViewBag.Email = new SelectList(db.UserProfiles, "Email", "FirstName", rideAddress.Email);
            return View(rideAddress);
        }

        // POST: RideAddresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RadID,PickUp,Destination,Email")] RideAddress rideAddress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rideAddress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Email = new SelectList(db.UserProfiles, "Email", "FirstName", rideAddress.Email);
            return View(rideAddress);
        }

        // GET: RideAddresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RideAddress rideAddress = db.RideAddresses.Find(id);
            if (rideAddress == null)
            {
                return HttpNotFound();
            }
            return View(rideAddress);
        }

        // POST: RideAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RideAddress rideAddress = db.RideAddresses.Find(id);
            db.RideAddresses.Remove(rideAddress);
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
