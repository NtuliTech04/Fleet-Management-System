using FleetTours___Application.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;


[assembly: OwinStartupAttribute(typeof(FleetTours___Application.Startup))]
namespace FleetTours___Application
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateUserAndRoles();
            UpdateStatus();
        }
        public void CreateUserAndRoles()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            if (!roleManager.RoleExists("Administrator"))
            {
                //create super admin role
                var role = new IdentityRole("Administrator");
                roleManager.Create(role);

                //create default user
                var user = new ApplicationUser();
                user.UserName = "fleetours@gmail.com";
                user.Email = "fleetours@gmail.com";
                user.FirstName = "Super-Admin";
                user.LastName = "Canfordit R&H";
                user.EmailConfirmed = true;
                string pwd = "FleeTours";

                var newuser = userManager.Create(user, pwd);
                if (newuser.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Administrator");
                }
            }

            if (!roleManager.RoleExists("Driver"))
            {
                var role = new IdentityRole("Driver");
                roleManager.Create(role);
            }
        }

        //Updates the Status of Booked Vehicles
        public void UpdateStatus()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var bookedList = db.BookedVehicles.ToList();
            var vehicleList = db.Vehicles.ToList();

            foreach (var item in bookedList)
            {
                if (item.HireDate > DateTime.Now.Date)
                {
                    item.Status = "Pending";
                }
                else if (DateTime.Now.Date >= item.HireDate && DateTime.Now.Date <= item.ReturnDate)
                {
                    item.Status = "Busy";
                }
                else
                {
                   item.Status = "Not Associated";
                }
            }
            
            foreach(var item in bookedList)
            {
                if (item.Status == "Busy")
                {
                    var getVehicle = db.Vehicles.Where(x => x.VehicleID == item.VehicleID).FirstOrDefault();
                    getVehicle.Status = "Busy";
                }
            }

            foreach (var item in vehicleList) 
            {
                if (bookedList.Where(x => x.VehicleID == item.VehicleID).All(x => x.Status != "Busy")) 
                {
                    item.Status = "Available";
                }
                //if (bookedList.Where(x=>x.VehicleID==item.VehicleID).Select(x=>x.Status=="Not Associated").FirstOrDefault())
                //{
                //    item.Driver = "Not Assigned";
                //}
            }

            db.SaveChanges();
        }
    }
}