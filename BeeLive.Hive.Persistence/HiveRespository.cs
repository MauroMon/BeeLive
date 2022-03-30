using BeeLive.Core.Repositories;
using BeeLive.Persistence;
using BeeLive.Persistence.Repositories;
using BeeLive.Hive.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BeeLive.Hive.Persistence
{
    public class HiveRespository : Repository<Hive.Core.Entities.Hive>, IHiveRepository
    {
        public HiveRespository(ElasticSearchContext context, IOptions<ElasticSearchSettings> settings) : base(context, settings.Value.HiveIndex)
        {
        }
    }
}
