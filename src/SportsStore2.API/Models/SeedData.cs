﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SportsStore2.API.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new SportsStore2Context(
                serviceProvider.GetRequiredService<DbContextOptions<SportsStore2Context>>()))
            {
                #region Images
                //if (context.Images.Any())
                //    return;
                context.Images.AddRange(
                        new Image
                        {
                            ImageUrl = "/Brands/adidas_logo.png"
                        },
                        new Image
                        {
                            ImageUrl = "/Brands/nike_logo.png"
                        },
                        new Image
                        {
                            ImageUrl = "/Brands/puma_logo.png"
                        },
                        new Image
                        {
                            ImageUrl = "/Products/adidas_shoes_women.png"
                        },
                        new Image
                        {
                            ImageUrl = "/Products/nike_shoes_men.png"
                        },
                        new Image
                        {
                            ImageUrl = "/Products/puma_shoes_children.png"
                        }
                    );

                #endregion

                #region Countries
                //look for any Countries
                //if (context.Countries.Any())
                //    return;

                context.Countries.AddRange(
                        new Country
                        {
                            Name = "Great Britain",
                            Type = "1 Tier",
                            Code = "GB"
                        },
                        new Country
                        {
                            Name = "Germany",
                            Type = "1 Tier",
                            Code = "DE"
                        },
                        new Country
                        {
                            Name = "France",
                            Type = "1 Tier",
                            Code = "FR"
                        },
                        new Country
                        {
                            Name = "Malta",
                            Type = "2 Tier",
                            Code = "MT"
                        }
                    );

                #endregion

                #region Categories
                //look for any category
                //if (context.Categories.Any())
                //    return;

                context.Categories.AddRange(
                    new Category
                    {
                        Name = "Men"
                    },
                    new Category
                    {
                        Name = "Women"
                    },
                    new Category
                    {
                        Name = "Children"
                    },
                    new Category
                    {
                        Name = "Unisex"
                    }
                );
                #endregion

                #region Brands
                //look for any brands
                //if (context.Brands.Any())
                //    return;

                context.Brands.AddRange(
                        new Brand
                        {
                            Name = "Adidas",
                            ImageId = 1
                        },
                        new Brand
                        {
                            Name = "Nike",
                            ImageId = 2
                        },
                        new Brand
                        {
                            Name = "Puma",
                            ImageId = 3
                        }
                    );

                #endregion

                #region Products
                //look for any products
                //if (context.Products.Any())
                //    return;

                context.Products.AddRange(
                        new Product
                        {
                            BrandId = 1,
                            CategoryId = 1,
                            Deal = false,
                            Description = "this is a test product",
                            Discount = 0,
                            Name = "Nike Shoes",
                            Price = new decimal(50.00),
                            Stock = 5,
                            ImageId = 5
                        },
                        new Product
                        {
                            BrandId = 2,
                            CategoryId = 2,
                            Deal = false,
                            Description = "this is a another test product",
                            Discount = 0,
                            Name = "Adidas Shoes",
                            Price = new decimal(65.00),
                            Stock = 5,
                            ImageId = 4
                        },
                        new Product
                        {
                            BrandId = 3,
                            CategoryId = 1,
                            Deal = false,
                            Description = "this is a another test product",
                            Discount = 0,
                            Name = "Puma Shoes",
                            Price = new decimal(65.00),
                            Stock = 5,
                            ImageId = 6
                        }

                    );
                #endregion

                context.SaveChanges();
            }
        }
    }
}
