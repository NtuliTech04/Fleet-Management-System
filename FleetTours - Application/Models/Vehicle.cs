using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace FleetTours___Application.Models
{
    public class Vehicle
    {
        [Key]
        [DisplayName("Vehichle ID")]
        [Required]
        public int VehicleID { get; set; }

        [DisplayName("Reg No.")]
        [Required]
        public string RegNo { get; set; }

        [DisplayName("Brand:")]
        [Required]
        public string VehicleMake { get; set; }

        [DisplayName("Model:")]
        [Required]
        public string Model { get; set; }

        [DisplayName("Type:")]
        [Required]
        public string Type { get; set; }

        [DisplayName("Capacity:")]
        [Required]
        public int Capacity { get; set; }

        public string Status { get; set; }

        public int DriverID { get; set; }
        public string Driver { get; set; }

        [DisplayName("Image:")]
        public byte[] VehicleImage { get; set; }

        [DisplayName("Duties:")]
        public string Duty { get; set; }
    }
}