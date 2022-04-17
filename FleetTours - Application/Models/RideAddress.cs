using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FleetTours___Application.Models
{
    public class RideAddress
    {
        [Key]
        public int RadID { get; set; }

        [Required]
        public string PickUp { get; set; }
        [Required]
        public string Destination { get; set; }

        [Required]
        public string Email { get; set; }

        public UserProfile UserProfile { get; set; }
    }
}