using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetTours___Application.Models
{
    public class UserAddress
    {
        [Key]
        public int AddressID { get; set; }

        public string PickUp { get; set; }

        public string Destination { get; set; }

        [ForeignKey("UserProfile")]
        public string Email { get; set; }
        public UserProfile UserProfile { get; set; }

        public int BookDetID { get; set; }
        public BookDetail BookDetail { get; set; }

    }
}