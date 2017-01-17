using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SportsStore2.API.Repository
{
    public interface IGenericRepository<T>
    {
        Task<T> Get<TKey>(Expression<Func<T, bool>> filter = null, string includeProperties = "", bool noTracking = false);
        Task<List<T>> GetAll(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}