using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SportsStore2.API.Models
{
    public partial class Country
    {
        public Country()
        {
            Address = new HashSet<Address>();
        }

        public short Id { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Code { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Type { get; set; }

        public virtual ICollection<Address> Address { get; set; }
    }
}
