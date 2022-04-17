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


namespace MVC_Modal_Crud.Controllers
{
    public class VehiclesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: /Vehicle/
        public ActionResult Index(string filter = null, int page = 1, int pageSize = 5, string sort = "VehicleID", string sortdir = "DESC")
        {
            var records = new PagedList<Vehicle>();
            ViewBag.filter = filter;
            records.Content = db.Vehicles
                        .Where(x => filter == null ||
                                (x.Duty.Contains(filter))
                                   || x.VehicleMake.Contains(filter)
                              )
                        .OrderBy(x => x.VehicleID /*sort + " " + sortdir*/)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        //.OrderByDescending(x=>x.VehicleID)
                        .ToList();

            // Count
            records.TotalRecords = db.Vehicles
                         .Where(x => filter == null ||
                               (x.Duty.Contains(filter)) || x.VehicleMake.Contains(filter)).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            return View(records);

        }


        // GET: /Vehicle/Details/5
        public ActionResult Details(int id = 0)
        {
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", vehicle);
        }


        // GET: /Vehicle/Create
        [HttpGet]
        public ActionResult Create()
        {
            var vehicle = new Vehicle();
            return PartialView("Create", vehicle);
        }


        // POST: /Vehicle/Create
        [HttpPost]
        public ActionResult Create(Vehicle vehicle, HttpPostedFileBase File)
        {
            if (ModelState.IsValid)
            {
                if (File != null)
                {
                    vehicle.VehicleImage = new byte[File.ContentLength];
                    File.InputStream.Read(vehicle.VehicleImage, 0, File.ContentLength);
                }

                if (vehicle.Duty == "Short Rides")
                {
                    vehicle.Driver = "Not Assigned";
                }
                vehicle.Status = "Available";

                db.Vehicles.Add(vehicle);
                db.SaveChanges();

                //return Json(new { success = true });
            }
            return RedirectToAction("Index","Vehicles");

            //return Json(vehicle, JsonRequestBehavior.AllowGet);
          
        }


        // GET: /Vehicle/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }

            return PartialView("Edit", vehicle);
        }


        // POST: /Vehicle/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Vehicle vehicle, HttpPostedFileBase File)
        {
            if (ModelState.IsValid)
            {
                if (File != null)
                {
                    vehicle.VehicleImage = new byte[File.ContentLength];
                    File.InputStream.Read(vehicle.VehicleImage, 0, File.ContentLength);
                }
                db.Entry(vehicle).State = EntityState.Modified;
                db.SaveChanges();
                //return Json(new { success = true });
            }
            return RedirectToAction("Index", "Vehicles",vehicle);
            //return PartialView("Edit", vehicle);
        }

        //
        // GET: /Vehicle/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", vehicle);
        }


        //
        // POST: /Vehicle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var vehicle = db.Vehicles.Find(id);
            db.Vehicles.Remove(vehicle);
            db.SaveChanges();
            //return Json(new { success = true });
            return RedirectToAction("Index", "Vehicles");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}


