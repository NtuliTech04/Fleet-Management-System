using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FleetTours___Application.Models
{
    public class BookedVehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int VehicleID { get; set; }
        public int BookID { get; set; }
        public string Status { get; set; }

        public int DriverID { get; set; }
        public string Driver { get; set; }


        //public Vehicle Vehicles { get; set; }

        //public Booking Bookings { get; set; }

        //public BookDetail BookDetails { get; set; }
    }
}