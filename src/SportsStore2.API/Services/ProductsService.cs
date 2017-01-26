using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SportsStore2.API.Models;
using SportsStore2.API.Repository;

namespace SportsStore2.API.Services
{
    public class ProductsService : GenericService<Product>, IProductsService
    {
        public ProductsService(IGenericRepository<Product> genericRepository) : base(genericRepository)
        {
        }

        bool IProductsService.Add(Product product, Expression<Func<Product, bool>> filter = null)
        {
            // If the image already exists...nullify image so EF won't try to insert a new one...
            if (product.Brand?.ImageId > 0)
                product.Brand.Image = null;

            if (product.Image == null) return Add(product, filter);

            if (product.Image.Id > 0)
                product.Image = null;

            return Add(product, filter);
        }
    }

}
