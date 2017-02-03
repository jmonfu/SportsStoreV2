using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SportsStore2.API.Models;
using SportsStore2.API.Models.AccountViewModels;
using SportsStore2.API.Repository;

namespace SportsStore2.API.Services
{
    public class AspNetUsersService : GenericService<AspNetUsers>, IAspNetUsersService
    {
        public AspNetUsersService(IGenericRepository<AspNetUsers> genericRepository) : base(genericRepository)
        {
        }

        bool IAspNetUsersService.Add(RegisterViewModel registerViewModel, Expression<Func<User, bool>> filter = null)
        {
            return AddAspNetUsers(registerViewModel);
        }

        bool IAspNetUsersService.Delete(string email)
        {
            var aspNetUser = CheckUserExists(email);
            if (aspNetUser != null)
            {
                Delete(aspNetUser);
                return true;
            }
            return false;
        }

    }
}
