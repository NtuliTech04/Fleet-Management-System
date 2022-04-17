using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FleetTours___Application.Models
{
    public class UserProfile
    {
        [Key]
        public string Email { get; set; }

        [Required]
        [DisplayName("Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Surname")]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Phone")]
        public string Phone { get; set; }

        [Required]
        [DisplayName("SA ID")]
        public string SAID { get; set; }

        [Display(Name = "Date Of Birth:")]
        public string BirthDate
        {
            get
            {
                string prefix;

                if (year() >= 00 && year() < today())
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

        public byte[] ProfileImage { get; set; }

        public ICollection<UserAddress> UserAddresses { get; set; }

        //Convert Birth-Year to INT: Function
        public int year()
        {
            return int.Parse(SAID.Substring(0, 2));
        }

        //Get Date Today - Select Year - Convert to INT
        public int today()
        {
            var year = DateTime.Today.Year;
            string yearString = Convert.ToString(year).Substring(2, 2);
            return int.Parse(yearString);
        }
    }
}