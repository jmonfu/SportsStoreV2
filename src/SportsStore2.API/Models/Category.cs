using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SportsStore2.API.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public short Id { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
