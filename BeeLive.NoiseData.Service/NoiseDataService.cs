using BeeLive.Core.Repositories;
using BeeLive.NoiseData.Core.Repositories;
using BeeLive.NoiseData.Core.Settings;
using BeeLive.NoiseData.TransferModels;
using BeeLive.NoiseData.TransferModels.Mapping;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeeLive.NoiseData.Service
{
    public class NoiseDataService : INoiseDataService
    {
        private INoiseDataRepository repository;
        private NoiseDataSettings config;
        private ILogger logger;

        public NoiseDataService(INoiseDataRepository noiseDataRepository, IOptions<NoiseDataSettings> config, ILogger<INoiseDataService> logger)
        {
            this.repository = noiseDataRepository;
            this.config = config.Value;
            this.logger = logger;
        }

        public async Task InsertNoiseData(NoiseDataDto noiseDataDto)
        {
            if(noiseDataDto == null)
            {
                throw new ArgumentNullException(nameof(noiseDataDto));
            }
            if(noiseDataDto.Decibel < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(noiseDataDto), "Decibel must be >=0");
            }

            var average = await repository.GetAverage(DateTime.UtcNow.AddHours(-config.HoursToCheck), DateTime.UtcNow, noiseDataDto.HiveId);

            if(average.Count < config.MinRequiredValues)
            {
                logger.LogInformation($"Hive {noiseDataDto.HiveId}: We have only {average.Count} noise values. In order to work at least {config.MinRequiredValues} values are required");
            }
            else
            {
                var warningAverage = average.Average + (decimal.Divide(average.Average, 100) * config.WarningPercentage);
                if(noiseDataDto.Decibel > warningAverage)
                {
                    logger.LogInformation($"Hive {noiseDataDto.HiveId}: SUSPECT NOISE!, warning average is {warningAverage}, noise is {noiseDataDto.Decibel} db");
                }
                else 
                {
                    logger.LogInformation($"Hive {noiseDataDto.HiveId}: reciceved {noiseDataDto.Decibel} db");
                }
            }
            await repository.AddAsync(noiseDataDto.ToEntity());
        }
    }
}