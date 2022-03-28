using BeeLive.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.Persistence.Repositories
{
    public class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        protected readonly ElasticSearchContext Context;
        protected readonly string IndexName;
        protected int maxResultWindow = 10000;
        protected int maxTermsCount = 65536;

        public ReadOnlyRepository(ElasticSearchContext context, string indexName)
        {
            Context = context;
            IndexName = indexName;
        }

        public async Task<TEntity> GetAsync(object id)
        {
            var response = await this.Context.Client.SearchAsync<TEntity>(req => req.Index(this.IndexName).
                Query(q => q.
                    Ids(i => i.Values(id.ToString()))
                ));
            return response.Hits.FirstOrDefault()?.Source;
        }

        public async Task<TEntity> GetAsync(object[] ids)
        {
            return await this.GetAsync(String.Join("@", ids));
        }

        public async Task<IEnumerable<TEntity>> GetManyAsync(string[] ids)
        {
            var response = await this.Context.Client.SearchAsync<TEntity>(req => req.Index(this.IndexName).
                Query(q => q.
                    Ids(i => i.Values(ids))
                ));
            return response.Hits.Select(s => s.Source);
        }

        public async Task<IQueryable<TEntity>> GetAllAsync()
        {
            var result = await this.Context.Client.SearchAsync<TEntity>(s => s.Index(this.IndexName)
              .Size(this.maxResultWindow)
            );

            return result.Documents.AsQueryable();
        }

        public async Task<IQueryable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> GetAll()
        {
            var result = this.Context.Client.Search<TEntity>(s => s.Index(this.IndexName)
              .Size(this.maxResultWindow)
            );

            return result.Documents.AsQueryable();
        }
    }

}
