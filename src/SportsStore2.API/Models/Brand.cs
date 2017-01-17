using System;
using System.Collections.Generic;

namespace SportsStore2.API.Models
{
    public partial class Brand
    {
        public Brand()
        {
            Products = new HashSet<Product>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long ImageId { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual Image Image { get; set; }
    }
}
