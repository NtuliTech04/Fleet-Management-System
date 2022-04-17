using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetTours___Application.Models
{
    public class Booking
    {
        [Key]
        [Required]
        [Display(Name ="Book ID")]
        public int BkID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Book Date")]
        public DateTime DateCreated { get; set; }

        [Required]
        [Display(Name = "Payment Method")]
        public string PayMethod { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name ="Payment")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Confirmation")]
        public string Confirmation { get; set; }

        public int DriverID { get; set; }
        public string Driver { get; set; }

        [ForeignKey("BookDetails")]
        public int BookDetID { get; set; }

        public virtual BookDetail BookDetails { get; set; }

        [ForeignKey("UserProfile")]
        public string Email { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}