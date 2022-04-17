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
    public class AssignDriversController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AssignDrivers
        public ActionResult Index()
        {
            var assignDrivers = db.AssignDrivers.Include(a => a.Booking);
            return View(assignDrivers.ToList());
        }

        // GET: AssignDrivers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssignDriver assignDriver = db.AssignDrivers.Find(id);
            if (assignDriver == null)
            {
                return HttpNotFound();
            }
            return View(assignDriver);
        }

        // GET: AssignDrivers/Create
        public ActionResult Create()
        {
            ViewBag.BkID = new SelectList(db.Bookings, "BkID", "PayMethod");
            return View();
        }

        // POST: AssignDrivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AssignID,SelectedDriver,BkID")] AssignDriver assignDriver)
        {
            if (ModelState.IsValid)
            {
                db.AssignDrivers.Add(assignDriver);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BkID = new SelectList(db.Bookings, "BkID", "PayMethod", assignDriver.BkID);
            return View(assignDriver);
        }

        // GET: AssignDrivers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssignDriver assignDriver = db.AssignDrivers.Find(id);
            if (assignDriver == null)
            {
                return HttpNotFound();
            }
            ViewBag.BkID = new SelectList(db.Bookings, "BkID", "PayMethod", assignDriver.BkID);
            return View(assignDriver);
        }

        // POST: AssignDrivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AssignID,SelectedDriver,BkID")] AssignDriver assignDriver)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assignDriver).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BkID = new SelectList(db.Bookings, "BkID", "PayMethod", assignDriver.BkID);
            return View(assignDriver);
        }

        // GET: AssignDrivers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssignDriver assignDriver = db.AssignDrivers.Find(id);
            if (assignDriver == null)
            {
                return HttpNotFound();
            }
            return View(assignDriver);
        }

        // POST: AssignDrivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AssignDriver assignDriver = db.AssignDrivers.Find(id);
            db.AssignDrivers.Remove(assignDriver);
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
