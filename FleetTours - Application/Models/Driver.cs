using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.ComponentModel;

namespace FleetTours___Application.Models
{
    public class Driver
    {
        [Key]
        [Display(Name ="Driver ID")]
        public int DriverID { get; set; }

        public int VehicleID { get; set; }

        [Required]
        [DisplayName("First Name")]
        [MinLength(3, ErrorMessage = "First Name cannot be less than 3 characters")]
        [StringLength(30, ErrorMessage = "First Name cannot be more than 30 characters")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Last Name cannot be less than 3 characters")]
        [StringLength(30, ErrorMessage = "Last Name cannot be more than 30 characters")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [MinLength(13, ErrorMessage = "Invalid ID Number")]
        [StringLength(13, ErrorMessage = "Invalid ID Number")]
        [DisplayName("SA ID")]
        public string SAID { get; set; }

        [Display(Name = "Date Of Birth:")]
        public string BirthDate
        {
            get
            {
                string prefix = "19";

                if (SAID.Substring(0, 1) == "0" ||
                     SAID.Substring(0, 1) == "1" ||
                     SAID.Substring(0, 1) == "2" ||
                     SAID.Substring(0, 1) == "3")
                {
                    prefix = "20";
                }
                else
                {
                    prefix = "19";
                }
                return SAID != null ? prefix + SAID.Substring(0, 2) + "/" + SAID.Substring(2, 2) + "/" + SAID.Substring(4, 2) : "";
            }
            set
            {

            }
        }

        [Display(Name = "Age ")]
        public string Cl_Age
        {
            get
            {
                DateTime dt = DateTime.ParseExact(BirthDate, "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);
                DateTime today = DateTime.Today;
                int age = today.Year - dt.Year;
                if (dt > today.AddYears(-age)) age--;
                return SAID != null ? age.ToString() : "";
            }
            set
            {

            }
        }
        [Display(Name = "Gender")]
        public string Cl_Gender
        {
            get
            {
                var gendeCode = (SAID.Substring(6, 4));
                var gender = int.Parse(gendeCode) < 5000 ? "Female" : "Male";
                return SAID != null ? gender : "";
            }
            set
            {

            }
        }

        [Required]
        [DisplayName("Phone")]
        [MinLength(10,ErrorMessage ="Phone number cannot be greater or less than 10 digits.")]
        [StringLength(10,ErrorMessage ="Phone number cannot be greater or less than 10 digits.")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name ="Profile Photo")]
        public byte[] DriverImage { get; set; }

        [NotMapped]
        public IEnumerable<Vehicle> VehicleList  { get; set; }
    }
}