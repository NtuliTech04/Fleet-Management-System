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
    public class DriversController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Drivers
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "DriverID", string sortdir = "DESC")
        {
            var records = new PagedList<Driver>();
            ViewBag.filter = filter;
            records.Content = db.Drivers
                        .Where(x => filter == null ||
                                (x.Email.Contains(filter))
                                   || x.SAID.Contains(filter)
                              )
                        .OrderBy(x => x.DriverID /*sort + " " + sortdir*/)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        //.OrderByDescending(x => x.DriverID)
                        .ToList();

            // Count
            records.TotalRecords = db.Drivers
                         .Where(x => filter == null ||
                               (x.Email.Contains(filter)) || x.SAID.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            return View(records);

        }

        // GET: Drivers/Details/5
        public ActionResult Details(int id = 0)
        {
            Driver driver = db.Drivers.Find(id);

            var GetVehicle = db.Vehicles.Where(x => x.VehicleID == driver.VehicleID).FirstOrDefault();

            if (GetVehicle != null)
            {
                ViewBag.Vehicle = GetVehicle.VehicleMake + " " + GetVehicle.Model;
            }
            else
            {
                ViewBag.Vehicle = "Not Assigned Yet";
            }

            if (driver == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", driver);
        }


        // GET: Drivers/Create
        [HttpGet]
        public ActionResult Create()
        {
            Driver driver = new Driver();
            driver.VehicleList = db.Vehicles.Where(x => x.Driver == "Not Assigned" && x.Duty== "Short Rides").ToList();

            //var driver = new Driver();
            return PartialView("Create", driver);
        }

        // POST: Drivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Create(Driver driver, HttpPostedFileBase File)
        {
            
            if (ModelState.IsValid)
            {
                if (File != null)
                {
                    driver.DriverImage = new byte[File.ContentLength];
                    File.InputStream.Read(driver.DriverImage, 0, File.ContentLength);
                }
                db.Drivers.Add(driver);
                db.SaveChanges();

                var vehicle = db.Vehicles.Where(x => x.VehicleID == driver.VehicleID).FirstOrDefault();

                if (vehicle != null)
                {
                    vehicle.Driver = "Assigned";
                    vehicle.DriverID = driver.DriverID;
                }

                db.SaveChanges();

                //return Json(new { success = true });
            }
            return RedirectToAction("Index", "Drivers");

            //return Json(vehicle, JsonRequestBehavior.AllowGet);
        }

        // GET: Drivers/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var driver = db.Drivers.Find(id);
            driver.VehicleList = db.Vehicles.Where(x => x.Driver == "Not Assigned" && x.Duty == "Short Rides").ToList();

            if (driver == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", driver);
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Driver driver, HttpPostedFileBase File)
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

                var vehicle = db.Vehicles.Where(x => x.VehicleID == driver.VehicleID).FirstOrDefault();

                if (vehicle != null)
                {
                    vehicle.Driver = "Assigned";
                    vehicle.DriverID = driver.DriverID;
                }
                db.SaveChanges();

                //return Json(new { success = true });
            }
            return RedirectToAction("Index", "Drivers", driver);
            //return PartialView("Edit", Drivers);
        }


        // GET: Drivers/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Driver driver = db.Drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", driver);
        }

        // POST: Drivers/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var driver = db.Drivers.Find(id);
            db.Drivers.Remove(driver);
            db.SaveChanges();
            //return Json(new { success = true });
            return RedirectToAction("Index", "Drivers");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
