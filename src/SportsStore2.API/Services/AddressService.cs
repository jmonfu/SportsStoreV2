using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SportsStore2.API.Models;
using SportsStore2.API.Repository;

namespace SportsStore2.API.Services
{
    public class AddressService : GenericService<Address>, IAddressService
    {
        public AddressService(IGenericRepository<Address> genericRepository) : base(genericRepository)
        {
        }

        bool IAddressService.Add(Address address, Expression<Func<Address, bool>> filter = null)
        {
            // If the image already exists...nullify image so EF won't try to insert a new one...
            if (address.Country != null && address.Country.Id > 0)
                address.Country = null;

            if (address.User != null && address.User.Id > 0)
                address.User = null;

            return Add(address, filter);
        }

    }
}
