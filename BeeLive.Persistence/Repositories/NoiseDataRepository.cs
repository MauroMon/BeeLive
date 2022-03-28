using BeeLive.Core.Entities;
using BeeLive.Core.Repositories;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.Persistence.Repositories
{
    public class NoiseDataRepository : Repository<NoiseData>, INoiseDataRepository
    {
        public NoiseDataRepository(ElasticSearchContext context, IOptions<ElasticSearchSettings> settings) : base(context, settings.Value.NoiseDataIndex)
        {
        }
    }
}
