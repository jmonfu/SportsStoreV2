using System;
using System.Linq.Expressions;
using SportsStore2.API.Models;
using SportsStore2.API.Models.AccountViewModels;

namespace SportsStore2.API.Services
{
    public interface IAspNetUsersService : IGenericService<AspNetUsers>
    {
        bool Delete(string email);
        bool Add(RegisterViewModel registerViewModel, Expression<Func<User, bool>> filter = null);
    }
}