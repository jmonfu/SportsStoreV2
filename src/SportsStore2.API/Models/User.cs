using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SportsStore2.API.Models
{
    public partial class User
    {
        public User()
        {
            Address = new HashSet<Address>();
        }

        public long Id { get; set; }
        public string ASPNETUsersId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string HomeNo { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string MobNo { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual AspNetUsers AspNetUsers { get; set; }

    }
}
