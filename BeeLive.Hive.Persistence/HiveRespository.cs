using BeeLive.Core.Repositories;
using BeeLive.Persistence;
using BeeLive.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.Hive.Persistence
{
    public class HiveRespository : Repository<Hive.Core.Entities.Hive>, IRepository<Hive.Core.Entities.Hive>
    {
        public HiveRespository(ElasticSearchContext context, string indexName) : base(context, indexName)
        {
        }
    }
}
