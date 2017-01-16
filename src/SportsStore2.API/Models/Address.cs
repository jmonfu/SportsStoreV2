using System;
using System.Collections.Generic;

namespace SportsStore2.API.Models
{
    public partial class Address
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public short AddressTypeId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public int CountryId { get; set; }
        public string PostCode { get; set; }

        public virtual AddressType AddressType { get; set; }
        public virtual Country Country { get; set; }
        public virtual User User { get; set; }
    }
}
