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
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> _genericRepository;

        public GenericService(IGenericRepository<T> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<List<T>> GetAll(
            Func<IQueryable<T>,
                IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null)
        {
            return await _genericRepository.GetAll(orderBy, includeProperties);
        }

        public async Task<T> GetById<TKey>(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = "")
        {
            return await _genericRepository.Get<T>(filter, includeProperties);
        }

        public bool Add(T entity, Expression<Func<T, bool>> filter = null)
        {
            try
            {
                _genericRepository.Add(entity, filter);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool Update(T entity)
        {
            _genericRepository.Update(entity);
            return true;
        }

        public void Delete(T entity)
        {
            _genericRepository.Delete(entity);
        }

        public bool AddAspNetUsers(RegisterViewModel registerViewModel)
        {
            return _genericRepository.AddAspNetUsers(registerViewModel);
        }

        public AspNetUsers CheckUserExists(string userEmail)
        {
            return _genericRepository.CheckUserExists(userEmail);
        }

        public bool UpdateUserEmailAspnetUsersTable(User aspnetUser)
        {
            return _genericRepository.UpdateUserEmailAspnetUsersTable(aspnetUser);
        }

    }
}
