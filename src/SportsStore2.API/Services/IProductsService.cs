using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SportsStore2.API.Models;

namespace SportsStore2.API.Services
{
    public interface IProductsService : IGenericService<Product>
    {
        new bool Add(Product product, Expression<Func<Product, bool>> filter = null);
    }
}