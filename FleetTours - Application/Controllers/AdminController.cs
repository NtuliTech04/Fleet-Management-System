using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using FleetTours___Application.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using FleetTours___Application.BusinessLogic;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FleetTours___Application.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRole(FormCollection form)
        {
            string rolename = form["RoleName"];
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            if (!roleManager.RoleExists(rolename))
            {
                //create Administrator role
                var role = new IdentityRole(rolename);
                roleManager.Create(role);
            }
            return View();
        }

        public ActionResult CreateUserAndAssignRole()
        {
            ViewBag.Roles = db.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CreateUserAndAssignRole(FormCollection form)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            string Name = form["txtName"];
            string LastName = form["txtSurname"];
            string UserName = form["txtEmail"];
            string email = form["txtEmail"];
            string pwd = form["txtPassword"];

            //create default user

            var user = new ApplicationUser();
            user.FirstName = Name;
            user.LastName = LastName;
            user.UserName = UserName;
            user.UserName = email;
            user.Email = UserName;

            //string password = pwd;

            var newuser = userManager.Create(user, pwd);
            string rol = form["RoleName"];
            ApplicationUser users = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            userManager.AddToRole(user.Id, rol);
            return RedirectToAction("Index", "Drivers");
        }

        [HttpGet]
        public ActionResult AssignDriver(int id)
        {
            AssignDriver process = new AssignDriver();

            process.Drivers = db.Drivers.ToList();

            process.BkID = id;
            return PartialView(process);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignDriver(AssignDriver assign)
        {
    
            db.AssignDrivers.Add(assign);
            db.SaveChanges();

            var updateOrder = db.Bookings.Where(x => x.BkID == assign.BkID).FirstOrDefault();
            updateOrder.Status = "Processed";

            var GetDriver = db.Drivers.Find(assign.SelectedDriver).DriverID;
            var GetVehicle = db.Bookings.Where(x => x.BkID == assign.BkID).Select(x => x.BookDetails.VehicleID).FirstOrDefault();

            var inject = db.BookedVehicles.Where(x => x.VehicleID == GetVehicle && x.BookID == assign.BkID).FirstOrDefault();
            inject.DriverID = GetDriver;
            inject.Driver = "Assigned";

            var insert = db.Bookings.Where(x => x.BkID == assign.BkID).FirstOrDefault();
            insert.DriverID = GetDriver;
            insert.Driver = "Assigned";
            db.SaveChanges();

            var driver = db.Drivers.Where(x => x.DriverID == GetDriver).FirstOrDefault();

            string url = "<a href=" + "https://grp61fleetmanagement.azurewebsites.net/Account/Login" + ">Click Here" + "</a>";
            var client = new SendGridClient("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");
            var from = new EmailAddress("nkonzo144@gmail.com", "Fleet Tours Transport Services");
            var subject = "New Trip - Transportation";

            var to = new EmailAddress(driver.Email);
            var htmlContent = "Hi " + driver.FirstName + " " + "<br/><br/>" + "You have a new trip assigned for transportation. Please log into the system for details. " +url;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = client.SendEmailAsync(msg);

            return RedirectToAction("Index", "Bookings");
        }

        public ActionResult Rides()
        {
            var rides = db.Rides.Include(r => r.RideAddress)
              .OrderByDescending(x => x.RideID)
              .Include(r => r.UserProfile).ToList();

            return View(rides);
        }

    }
}