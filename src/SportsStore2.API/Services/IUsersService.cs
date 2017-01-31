using System;
using System.Linq.Expressions;
using SportsStore2.API.Models;

namespace SportsStore2.API.Services
{
    public interface IUsersService : IGenericService<User>
    {
        bool Add(User user, Expression<Func<User, bool>> filter = null);

        bool Update(User user);
    }
}