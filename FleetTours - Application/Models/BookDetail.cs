using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FleetTours___Application.Models
{
    public class BookDetail
    {
        [Key]

        [DisplayName("Details ID")]
        public int BookDetID { get; set; }

        [NotMapped]
        [DisplayName("Full Name")]
        public string FullName { get; set; }

        [NotMapped]
        [DisplayName("RSAID Number")]
        public string RSAID { get; set; }

        [NotMapped]
        [DisplayName("Vehicle To Book")] 
        public string BookedVehicle { get; set; }

        [DisplayName("Vacation")]
        [Required]
        public string Vacation { get; set; }

        [Required]
        [DisplayName("Pick Up Date")]
        [DataType(DataType.Date)]
        public DateTime VacationDate { get; set; }

        [Required]
        [DisplayName("Pick Up Time")]
        [DataType(DataType.Time)]
        public TimeSpan LessonTime { get; set; }

        [Required]
        [DisplayName("Hire Period")]
        public int HirePeriod { get; set; }
        
        [DisplayName("Return Date")]
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }

        [DisplayName("Passengers")]
        public int Passengers { get; set; }

        public double Total { get; set; }

        [Required]
        [Display(Name ="Email")]
        [ForeignKey("UserProfiles")]
        public string Email { get; set; }
        public UserProfile UserProfiles { get; set; }

        public int VehicleID { get; set; }
        public Vehicle Vehicles { get; set; }

        [NotMapped]
        public virtual UserAddress UserAddresses { get; set; }
        public double CalcTotal()
        {
            return ((Passengers * 400) * HirePeriod);
        }
    }
}