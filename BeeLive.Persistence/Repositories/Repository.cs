using BeeLive.Core.Repositories;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.Persistence.Repositories
{
    public class Repository<TEntity> : ReadOnlyRepository<TEntity>, Core.Repositories.IRepository<TEntity> where TEntity : class
    {
        public Repository(ElasticSearchContext context, string indexName) : base(context, indexName)
        {

        }

        public virtual void Add(TEntity entity)
        {
            this.Context.Client.Index<TEntity>(entity, i => i.Index(this.IndexName));
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await this.Context.Client.IndexAsync<TEntity>(entity, i => i.Index(this.IndexName));
        }

        public virtual async Task AddAsync(IEnumerable<TEntity> entities)
        {
            await this.Context.Client.IndexManyAsync<TEntity>(entities, this.IndexName);
        }

        public virtual async Task RefreshAsync()
        {
            await this.Context.Client.Indices.RefreshAsync(this.IndexName);
        }
    }
}
