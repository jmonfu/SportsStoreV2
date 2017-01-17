using System;
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
                #region Countries

                if (!context.Countries.Any())
                {
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
                }

                #endregion

                #region Images

                if (!context.Images.Any())
                {
                    context.Images.AddRange(
                            new Image
                            {
                                Url = "/Brands/adidas_logo.png"
                            },
                            new Image
                            {
                                Url = "/Brands/nike_logo.png"
                            },
                            new Image
                            {
                                Url = "/Brands/puma_logo.png"
                            },
                            new Image
                            {
                                Url = "/Products/adidas_shoes_women.png"
                            },
                            new Image
                            {
                                Url = "/Products/nike_shoes_men.png"
                            },
                            new Image
                            {
                                Url = "/Products/puma_shoes_children.png"
                            }
                        );

                }
                #endregion

                #region Products

                if (!context.Products.Any())
                {
                    context.Products.AddRange(
                            new Product
                            {
                                BrandId = 9,
                                CategoryId = 21,
                                Deal = false,
                                Description = "this is a test product",
                                Discount = "0",
                                Name = "Nike Shoes",
                                Price = new decimal(50.00),
                                Stock = 5,
                                ImageId = 29
                            },
                            new Product
                            {
                                BrandId = 8,
                                CategoryId = 22,
                                Deal = false,
                                Description = "this is a another test product",
                                Discount = "0",
                                Name = "Adidas Shoes",
                                Price = new decimal(65.00),
                                Stock = 5,
                                ImageId = 28
                            },
                            new Product
                            {
                                BrandId = 10,
                                CategoryId = 23,
                                Deal = false,
                                Description = "this is a another test product",
                                Discount = "0",
                                Name = "Puma Shoes",
                                Price = new decimal(65.00),
                                Stock = 5,
                                ImageId = 30
                            }

                        );
                }
                #endregion

                #region Categories

                if (!context.Categories.Any())
                {
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
                }

                #endregion

                #region Brands

                if (!context.Brands.Any())
                {
                    context.Brands.AddRange(
                            new Brand
                            {
                                Name = "Adidas",
                                ImageId = 25
                            },
                            new Brand
                            {
                                Name = "Nike",
                                ImageId = 26
                            },
                            new Brand
                            {
                                Name = "Puma",
                                ImageId = 27
                            }
                        );

                }
                #endregion



                context.SaveChanges();
            }
        }
    }
}
