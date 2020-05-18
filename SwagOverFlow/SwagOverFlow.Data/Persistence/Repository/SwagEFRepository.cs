using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SwagOverFlow.Data.Persistence
{
    //https://codewithshadman.com/repository-pattern-csharp/
    public class SwagEFRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected SwagContext context;
        internal DbSet<TEntity> dbSet;

        public SwagEFRepository(SwagContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> GetWithRawSql(string query,
            params object[] parameters)
        {
            return dbSet.FromSqlRaw(query, parameters).ToList();
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }


            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetByID(params object[] ids)
        {
            return dbSet.Find(ids);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(params object [] ids)
        {
            TEntity entityToDelete = dbSet.Find(ids);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            var entry = context.Entry(entityToUpdate);
            if (entry.State != EntityState.Added)
            {
                entry.State = EntityState.Modified;
            }
        }

        public virtual void Attach(TEntity entityToAttach)
        {
            dbSet.Attach(entityToAttach);
        }

        public virtual void Detach(TEntity entityToDetach)
        {
            context.Entry(entityToDetach).State = EntityState.Detached;
        }
    }
}
