using System;
using System.Linq.Expressions;
using SportsStore2.API.Models;
using SportsStore2.API.Models.AccountViewModels;

namespace SportsStore2.API.Services
{
    public interface IUsersService : IGenericService<User>
    {

        bool Update(User user);
        bool Add(User user, Expression<Func<User, bool>> filter = null);
    }
}