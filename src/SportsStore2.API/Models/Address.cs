using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SportsStore2.API.Models
{
    public partial class Address
    {
        public short Id { get; set; }
        public short CountryId { get; set; }
        public long UserId { get; set; }
        public short AddressTypeId { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Only characters are allowed")]
        public string City { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string PostCode { get; set; }

        public virtual AddressType AddressType { get; set; }
        public virtual Country Country { get; set; }
        public virtual User User { get; set; }
    }
}
