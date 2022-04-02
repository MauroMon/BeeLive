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
        private NoiseDataSettings settings;
        private ILogger logger;

        public NoiseDataService(INoiseDataRepository noiseDataRepository, IOptions<NoiseDataSettings> settings, ILogger<INoiseDataService> logger)
        {
            this.repository = noiseDataRepository;
            this.settings = settings.Value;
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
            bool warning = await IsWarningAsync(noiseDataDto.HiveId, noiseDataDto.Decibel);
            await repository.AddAsync(noiseDataDto.ToEntity(warning));
        }

        private async Task<bool> IsWarningAsync(int hiveId, decimal decibel)
        {
            var average = await repository.GetAverage(DateTime.UtcNow.AddHours(-settings.HoursToCheck), DateTime.UtcNow, hiveId);
            if (average.Count >= settings.MinRequiredValues)
            {
                var warningDecibel = average.Average + (decimal.Divide(average.Average, 100) * settings.WarningNoiseIncreasePercentage);
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
                logger.LogInformation($"Hive {hiveId}: We have only {average.Count} noise values. In order to work at least {settings.MinRequiredValues} values are required");
                return false;
            }
        }

        public async Task<NoiseDataCountDto> GetWarningCountAsync(DateTime dtFrom, DateTime dtTo, int hiveId)
        {
            return (await repository.CountAsync(dtFrom, dtTo, hiveId))?.ToDto();
        }

        public async Task<decimal> GetLastNoiseDataAsync(int hiveId)
        {
            return await repository.GetLastNoiseData(hiveId);
        }

        public async Task<NoiseDataStatus> GetHiveStatus(int hiveId)
        {
            //chec alarm
            var alarmNoiseDataCount = await GetWarningCountAsync(DateTime.UtcNow.AddMinutes(-settings.AlarmConsecutiveMinutes), DateTime.UtcNow, hiveId);
            var alarmaMargin = GetPercentage(alarmNoiseDataCount.Total, settings.AlarmCOnsecutiveMinutesPercentage);
            if(alarmNoiseDataCount.Total > alarmaMargin)
            {
                logger.LogInformation($"Hive {hiveId} noise alarm!");
                return NoiseDataStatus.Alarm;
            }
            
            //check warning
            var warningNoiseDataCount = await GetWarningCountAsync(DateTime.UtcNow.AddMinutes(-settings.WarningConsecutiveMinutes), DateTime.UtcNow, hiveId);
            var warningMargin = GetPercentage(warningNoiseDataCount.Total, settings.WarningConsecutiveMinutesPercentage);
            if (warningNoiseDataCount.Warning > warningMargin)
            {
                logger.LogInformation($"Hive {hiveId} nosie warning!");
                return NoiseDataStatus.Warning;
            }
            logger.LogInformation($"Hive {hiveId} noise ok");
            return NoiseDataStatus.Ok;
        }

        private decimal GetPercentage(decimal d1, decimal d2)
        {
            return Math.Floor(decimal.Divide(d1, 100) * d2);
        }
    }
}