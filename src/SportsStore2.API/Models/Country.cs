using System;
using System.Collections.Generic;

namespace SportsStore2.API.Models
{
    public partial class Country
    {
        public Country()
        {
            Address = new HashSet<Address>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }

        public virtual ICollection<Address> Address { get; set; }
    }
}
