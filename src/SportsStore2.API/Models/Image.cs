﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "This field is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Url { get; set; }

        public virtual ICollection<Brand> Brands { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
