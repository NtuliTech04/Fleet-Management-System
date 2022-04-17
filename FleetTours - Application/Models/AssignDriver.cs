using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace FleetTours___Application.Models
{
    public class AssignDriver
    {
        [Key]
        public int AssignID { get; set; }
        public int SelectedDriver { get; set; }
        public int BkID { get; set; }
        public Booking Booking { get; set; }

        [NotMapped]
        public IEnumerable<Driver> Drivers { get; set; }

    }
}