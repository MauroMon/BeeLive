using BeeLive.NoiseData.Core.Repositories;
using BeeLive.Persistence;
using BeeLive.Persistence.Repositories;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.NoiseData.Persistence
{
    public class NoiseDataRepository : Repository<Core.Entities.NoiseData>, INoiseDataRepository
    {
        public NoiseDataRepository(ElasticSearchContext context, IOptions<ElasticSearchSettings> settings) : base(context, settings.Value.NoiseDataIndex)
        {
        }
    }
}
