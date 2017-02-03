using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SportsStore2.API.Models;
using SportsStore2.API.Models.AccountViewModels;

namespace SportsStore2.API.Services
{
    public interface IGenericService<T> where T : class
    {
        Task<List<T>> GetAll(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null);

        Task<T> GetById<TKey>(
            Expression<Func<T, bool>> filter = null, 
            string includeProperties = "");

        bool Add(T entity, Expression<Func<T, bool>> filter = null);
        bool Update(T entity);
        void Delete(T entity);
        bool AddAspNetUsers(RegisterViewModel registerViewModel);
        AspNetUsers CheckUserExists(string userEmail);
        bool UpdateUserEmailAspnetUsersTable(User aspnetUser);
    }
}