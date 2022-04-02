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

        /// <summary>
        /// Insert noise data in db
        /// </summary>
        /// <param name="noiseDataDto">Noise data to insert</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task InsertNoiseDataAsync(NoiseDataDto noiseDataDto)
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

        /// <summary>
        /// Calculate if noise is warning
        /// </summary>
        /// <param name="hiveId">hive id</param>
        /// <param name="decibel">noise data in decibel</param>
        /// <returns>true if warning</returns>
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

        /// <summary>
        /// Get last recorded noise data
        /// </summary>
        /// <param name="hiveId">hive id</param>
        /// <returns>Noise data in decibel</returns>
        public async Task<decimal> GetLastNoiseDataAsync(int hiveId)
        {
            return await repository.GetLastNoiseData(hiveId);
        }

        /// <summary>
        /// Calculate Hive status
        /// </summary>
        /// <param name="hiveId">Hive id</param>
        /// <returns>The hive status</returns>
        public async Task<NoiseDataStatus> GetHiveStatusAsync(int hiveId)
        {
            //chec alarm
            var alarmNoiseDataCount = await repository.CountAsync(DateTime.UtcNow.AddMinutes(-settings.AlarmConsecutiveMinutes), DateTime.UtcNow, hiveId);
            if(alarmNoiseDataCount.Total == 0)
            {
                logger.LogInformation($"Hive {hiveId} no data. return ok");
                return NoiseDataStatus.Ok;
            }
            var alarmaMargin = GetPercentage(alarmNoiseDataCount.Total, settings.AlarmConsecutiveMinutesPercentage);
            if(alarmNoiseDataCount.Warning >= alarmaMargin)
            {
                logger.LogInformation($"Hive {hiveId} noise alarm!");
                return NoiseDataStatus.Alarm;
            }
            
            //check warning
            var warningNoiseDataCount = await repository.CountAsync(DateTime.UtcNow.AddMinutes(-settings.WarningConsecutiveMinutes), DateTime.UtcNow, hiveId);
            var warningMargin = GetPercentage(warningNoiseDataCount.Total, settings.WarningConsecutiveMinutesPercentage);
            if (warningNoiseDataCount.Warning >= warningMargin)
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