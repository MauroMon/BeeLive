using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.Core.Repositories
{
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity);
        void Add(TEntity entity);
        Task AddAsync(IEnumerable<TEntity> entities);
        Task RefreshAsync();
    }
}
