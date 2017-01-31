using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
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

        public async Task<T> Get<TKey>(Expression<Func<T, bool>> filter = null, string includeProperties = "")

        {
            IQueryable<T> query = Context.Set<T>();
            query = IncludePropertiesQuery(query, includeProperties);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.SingleOrDefaultAsync();
        }

        public async Task<List<T>> GetAll(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = Context.Set<T>();
            query = IncludePropertiesQuery(query, includeProperties);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var collection = await query.ToListAsync();
            return collection;
        }

        public void Add(T entity, Expression<Func<T, bool>> filter = null)
        {
            var existing = Get<T>(filter);
            if (existing.Result != null) return;
            Context.Add(entity);
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

        public AspNetUsers CheckUserExists(string userEmail)
        {
            return Context.AspNetUsers.FirstOrDefault(x => x.Email == userEmail);
        }

        public string GetUserIdFromAspNetUsers(string userEmail)
        {
            var aspnetUser = Context.AspNetUsers.FirstOrDefault(x => x.Email == userEmail);
            return aspnetUser.Id;
        }

        public bool UpdateUserEmailAspnetUsersTable(User user)
        {
            var aspnetUser = Context.AspNetUsers.FirstOrDefault(x => x.Id == user.ASPNETUsersId);
            if (aspnetUser != null)
            {
                aspnetUser.Email = user.Email;
                aspnetUser.NormalizedEmail = user.Email.ToUpper();
                Context.Update(aspnetUser);
                Save();
                return true;
            }
            return false;
        }

        private void Save()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private IQueryable<T> IncludePropertiesQuery(IQueryable<T> query, string includeProperties = "")
        {
            if (includeProperties == null)
            {
                includeProperties = "";
            }

            includeProperties = includeProperties.Trim() ?? string.Empty;
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return query;

        }

    }
}
