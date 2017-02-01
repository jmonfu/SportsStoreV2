using System;
using System.Linq.Expressions;
using SportsStore2.API.Models;

namespace SportsStore2.API.Services
{
    public interface IAddressService : IGenericService<Address>
    {
        bool Add(Address address, Expression<Func<Address, bool>> filter = null);
    }
}