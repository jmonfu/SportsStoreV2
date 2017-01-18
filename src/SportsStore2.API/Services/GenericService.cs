using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SportsStore2.API.Models;
using SportsStore2.API.Repository;

namespace SportsStore2.API.Services
{
    public class GenericService<T> : IGenericService<T>  where T : class
    {
        private readonly IGenericRepository<T> _genericRepository;
        private IGenericService<T> _genericServiceImplementation;

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
            string includeProperties = "", 
            bool noTracking = false)
        {
            return await _genericRepository.Get<T>(filter, includeProperties, noTracking);
        }

        public async Task<bool> Add(T entity, Expression<Func<T, bool>> filter = null)
        {
            var existing = await _genericRepository.Get<T>(filter);
            if (existing != null) return false;

            _genericRepository.Add(entity);
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

    }

}
