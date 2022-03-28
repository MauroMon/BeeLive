using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.Core.Repositories
{
    public interface IReadOnlyRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetAsync(object id);
        Task<TEntity> GetAsync(object[] ids);
        Task<IQueryable<TEntity>> GetAllAsync();
        IQueryable<TEntity> GetAll();
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
