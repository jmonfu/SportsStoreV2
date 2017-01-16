using System;
using System.Collections.Generic;

namespace SportsStore2.API.Models
{
    public partial class Image
    {
        public Image()
        {
            Brands = new HashSet<Brand>();
            Products = new HashSet<Product>();
        }

        public long Id { get; set; }
        public string ImageUrl { get; set; }

        public virtual ICollection<Brand> Brands { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
