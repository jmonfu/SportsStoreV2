using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportsStore2.API.Data;
using SportsStore2.API.Models;
using SportsStore2.API.Models.AccountViewModels;
using SportsStore2.API.Repository;

namespace SportsStore2.API.Services
{
    public class UsersService : GenericService<User>, IUsersService
    {
        public UsersService(IGenericRepository<User> genericRepository) : base(genericRepository)
        {
        }

        bool IUsersService.Add(User user, Expression<Func<User, bool>> filter = null)
        {
            //check if the same email already exists in the DB
            var thisUser = GetById<User>(m => m.Email == user.Email).Result;
            if (thisUser == null)
            {
                //check that the user email already exists in AspNetUsers table
                if (CheckUserExists(user.Email) != null)
                {
                    //get the ASPNETUserId
                    user.ASPNETUsersId = CheckUserExists(user.Email).Id;
                    //add The user
                    return Add(user, filter);
                }
            }
            return false;
        }

        bool IUsersService.Update(User user)
        {
            //if the user updated his email address, then we need to update the email address
            //inside the ASPNetUsers Table
            UpdateUserEmailAspnetUsersTable(user);

            return Update(user);
        }

    }
}
