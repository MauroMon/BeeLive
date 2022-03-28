using BeeLive.Core.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.Persistence
{
    public class ElasticSearchContext
    {
        public IElasticClient Client { get; }
        public ElasticSearchContext(IOptions<ElasticSearchSettings> elasticSearchSettings, ILogger<ElasticSearchContext> logger)
        {
            ElasticSearchSettings valueSettings = elasticSearchSettings.Value;
            var settings = new Nest.ConnectionSettings(new Uri(valueSettings.Uri))
                            .ThrowExceptions(true);

            settings.BasicAuthentication(valueSettings.UserName, valueSettings.Password);

#if DEBUG
            settings.EnableDebugMode().DisableDirectStreaming().PrettyJson();
#endif      

            Client = new ElasticClient(settings);

            //Check if temperature data index exists
            logger.LogInformation($"Check if index {valueSettings.NoiseDataIndex} exists");
            if (!Client.Indices.Exists(valueSettings.NoiseDataIndex).Exists)
            {
                logger.LogInformation($"Creating index {valueSettings.NoiseDataIndex}");
                Client.Indices.Create(new CreateIndexDescriptor(valueSettings.NoiseDataIndex).Map<NoiseData>(pu => pu.AutoMap()));
            }
        }
    }
}
