using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FleetTours___Application.Models
{
    public class Ride
    {
        [Key]
        public int RideID { get; set; }

        public int VehicleID { get; set; }

        public int RadID { get; set; }

        public int DriverID { get; set; }

        public string Email { get; set; }

        public double Distance { get; set; }

        public double BasicCost { get; set; }

        public double Discounts { get; set; }

        public double TotalCost { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string RideStatus { get; set; }

        [NotMapped]
        public Vehicle Vehicle { get; set; }

        public RideAddress RideAddress { get; set; }
        [NotMapped]
        public Driver Driver { get; set; }

        public UserProfile UserProfile { get; set; }
    }
}