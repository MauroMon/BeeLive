using BeeLive.Core.Repositories;
using BeeLive.Hive.Core.Entities;
using BeeLive.Hive.Core.Repositories;
using BeeLive.Hive.Service;
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
        private IHiveService hiveService;
        private NoiseDataSettings config;
        private ILogger logger;

        public NoiseDataService(INoiseDataRepository noiseDataRepository, IHiveService hiveService, IOptions<NoiseDataSettings> config, ILogger<INoiseDataService> logger)
        {
            this.repository = noiseDataRepository;
            this.hiveService = hiveService;
            this.config = config.Value;
            this.logger = logger;
        }

        public async Task InsertNoiseData(NoiseDataDto noiseDataDto)
        {
            if (noiseDataDto == null)
            {
                throw new ArgumentNullException(nameof(noiseDataDto));
            }
            if (noiseDataDto.Decibel < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(noiseDataDto), "Decibel must be >=0");
            }
            bool warning = await IsWarning(noiseDataDto.HiveId, noiseDataDto.Decibel);

            await UpdateHiveStatusAsync(noiseDataDto.HiveId, warning);
            await repository.AddAsync(noiseDataDto.ToEntity(warning));
        }

        public async Task<bool> IsWarning(int hiveId, decimal decibel)
        {
            var average = await repository.GetAverage(DateTime.UtcNow.AddHours(-config.HoursToCheck), DateTime.UtcNow, hiveId);
            if (average.Count >= config.MinRequiredValues)
            {
                var warningDecibel = average.Average + (decimal.Divide(average.Average, 100) * config.WarningNiseIncreasePercentage);
                if (decibel > warningDecibel)
                {
                    logger.LogInformation($"Hive {hiveId}: SUSPECT NOISE!, warning average is {warningDecibel}, noise is {decibel} db");
                    return true;
                }
                else
                {
                    logger.LogInformation($"Hive {hiveId}: reciceved {decibel} db");
                    return false;
                }
            }
            else
            {
                logger.LogInformation($"Hive {hiveId}: We have only {average.Count} noise values. In order to work at least {config.MinRequiredValues} values are required");
                return false;
            }
        }

        public async Task UpdateHiveStatusAsync(int hiveId, bool warning)
        {
            Hive.Core.Entities.Hive hive = await hiveService.GetOrCreateHiveAsync(hiveId);
            if (warning && hive.Status == HiveStatus.Ok)
            {
                var noiseDataCount = await repository.CountAsync(DateTime.UtcNow.AddMinutes(-config.WarningConsecutiveMinutes), DateTime.UtcNow, hiveId);
                var warningCount = Math.Floor(decimal.Divide(noiseDataCount.Total, 100) * config.WarningConsecutiveMinutesPercentage);
                if(noiseDataCount.Warning > warningCount)
                {
                    logger.LogInformation($"Hive {1} nosie warning!");
                    hive.Status = HiveStatus.Warning;
                }
            }
        }
    }
}