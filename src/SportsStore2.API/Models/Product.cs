using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SportsStore2.API.Models
{
    public partial class Product
    {
        public long Id { get; set; }
        public long ImageId { get; set; }
        public long BrandId { get; set; }
        public short CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool Deal { get; set; }
        public string Discount { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
        public virtual Image Image { get; set; }
    }
}
