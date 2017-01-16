using System;
using System.Collections.Generic;

namespace SportsStore2.API.Models
{
    public partial class AddressType
    {
        public AddressType()
        {
            Address = new HashSet<Address>();
        }

        public short Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Address> Address { get; set; }
    }
}
