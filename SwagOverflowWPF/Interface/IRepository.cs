using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SwagOverflowWPF.Interface
{
    //https://codewithshadman.com/repository-pattern-csharp/
    //https://www.youtube.com/watch?v=rtXpYpZdOzM&feature=youtu.be
    //Benefits - Minimized duplicate query logic and decouples your application from persistance frameworks
    //Queries only exist in the Repository
    public interface IRepository<TEntity> where TEntity : class
    {
        void Delete(TEntity entityToDelete);
        void Delete(params object[] ids);
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");
        TEntity GetByID(params object[] ids);
        IEnumerable<TEntity> GetWithRawSql(string query,
            params object[] parameters);
        void Insert(TEntity entity);
        void Update(TEntity entityToUpdate);
    }
}
