using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore2.API.Models;

namespace SportsStore2.API.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly SportsStore2Context Context;
        protected DbSet<T> DbSet;

        public GenericRepository(SportsStore2Context context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public async Task<T> Get<TKey>(Expression<Func<T, bool>> filter = null, string includeProperties = "", bool noTracking = false)

        {
            includeProperties = includeProperties.Trim() ?? string.Empty;
            IQueryable<T> query = Context.Set<T>();

            if (noTracking)
            {
                query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.SingleOrDefaultAsync();
        }

        public async Task<List<T>> GetAll(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            //m => m.OrderBy(o => o.Property1).ThenBy(o => o.Property2)
            includeProperties = includeProperties ?? string.Empty;
            IQueryable<T> query = Context.Set<T>();

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var collection = await query.ToListAsync();
            return collection;
        }

        public void Add(T entity)
        {
            Context.Set<T>().Add(entity);
            Save();
        }

        public void Update(T entity)
        {
            Context.Set<T>().Update(entity);
            Save();
        }

        public void Delete(T entity)
        {
            var dbSet = Context.Set<T>();
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);

            Save();
        }

        private void Save()
        {
            Context.SaveChanges();
        }

    }
}
