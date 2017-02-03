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
        [Required(ErrorMessage = "This field is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid price")]
        public decimal Price { get; set; }
        [RegularExpression("[^0-9]", ErrorMessage = "Entry must be numeric")]
        public int Stock { get; set; }
        public bool Deal { get; set; }
        public string Discount { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
        public virtual Image Image { get; set; }
    }
}
