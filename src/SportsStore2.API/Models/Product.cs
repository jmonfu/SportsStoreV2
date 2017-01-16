using System;
using System.Collections.Generic;

namespace SportsStore2.API.Models
{
    public partial class Product
    {
        public long Id { get; set; }
        public short CategoryId { get; set; }
        public long BrandId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public short Stock { get; set; }
        public bool? Deal { get; set; }
        public int? Discount { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
    }
}
