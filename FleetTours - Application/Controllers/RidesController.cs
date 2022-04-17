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
    public class RidesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Rides
        public ActionResult Index()
        {
            var rides = db.Rides.Include(r => r.RideAddress)
                .OrderByDescending(x => x.RideID)
                .Include(r => r.UserProfile).ToList();

            return View(rides);
        }

        // GET: Rides/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ride ride = db.Rides.Find(id);
            if (ride == null)
            {
                return HttpNotFound();
            }
            return View(ride);
        }

        // GET: Rides/Create
        public ActionResult Create()
        {
            ViewBag.RadID = new SelectList(db.RideAddresses, "RadID", "PickUp");
            ViewBag.Email = new SelectList(db.UserProfiles, "Email", "FirstName");
            return View();
        }

        // POST: Rides/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RideID,VehicleID,RadID,DriverID,Email,Distance,BasicCost,Discounts,TotalCost")] Ride ride)
        {
            if (ModelState.IsValid)
            {
                db.Rides.Add(ride);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RadID = new SelectList(db.RideAddresses, "RadID", "PickUp", ride.RadID);
            ViewBag.Email = new SelectList(db.UserProfiles, "Email", "FirstName", ride.Email);
            return View(ride);
        }

        // GET: Rides/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ride ride = db.Rides.Find(id);
            if (ride == null)
            {
                return HttpNotFound();
            }
            ViewBag.RadID = new SelectList(db.RideAddresses, "RadID", "PickUp", ride.RadID);
            ViewBag.Email = new SelectList(db.UserProfiles, "Email", "FirstName", ride.Email);
            return View(ride);
        }

        // POST: Rides/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RideID,VehicleID,RadID,DriverID,Email,Distance,BasicCost,Discounts,TotalCost")] Ride ride)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ride).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RadID = new SelectList(db.RideAddresses, "RadID", "PickUp", ride.RadID);
            ViewBag.Email = new SelectList(db.UserProfiles, "Email", "FirstName", ride.Email);
            return View(ride);
        }

        // GET: Rides/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ride ride = db.Rides.Find(id);
            if (ride == null)
            {
                return HttpNotFound();
            }
            return PartialView(ride);
        }

        // POST: Rides/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ride ride = db.Rides.Find(id);
            db.Rides.Remove(ride);
            db.SaveChanges();
            return RedirectToAction("MyRides","CarHire");
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
