using BeeLive.Hive.TransferModels;
using BeeLive.NoiseData.Core.Settings;
using BeeLive.NoiseData.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeeLive.Hive.Service
{
    public class HiveService : IHiveService
    {
        private INoiseDataService noiseDataService;
        private NoiseDataSettings noiseDataSettings;
        private ILogger logger;

        public HiveService(INoiseDataService noiseDataService, IOptions<NoiseDataSettings> noiseDataSettings, ILogger<INoiseDataService> logger)
        {
            this.noiseDataService = noiseDataService;
            this.noiseDataSettings = noiseDataSettings.Value;
            this.logger = logger;
        }

        /// <summary>
        /// Get an Hive With noise data
        /// </summary>
        /// <param name="hiveId">Hive Id</param>
        /// <returns>An hive with noise data</returns>
        public async Task<HiveDto> GetHive(int hiveId)
        {
            return new HiveDto()
            {
                Id = hiveId,
                Status = await GetHiveStatus(hiveId),
                Noise = await noiseDataService.GetLastNoiseDataAsync(hiveId)
            };
        }

        /// <summary>
        /// Update Hive status (ok, warn, alert) 
        /// </summary>
        /// <param name="hiveId">Hive ID</param>
        /// <returns>The Hive with the updates status</returns>
        private async Task<HiveStatus> GetHiveStatus(int hiveId)
        {
            var noiseDataCount = await noiseDataService.GetWarningCountAsync(DateTime.UtcNow.AddMinutes(-noiseDataSettings.WarningConsecutiveMinutes), DateTime.UtcNow, hiveId);

            var warningMargin = Math.Floor(decimal.Divide(noiseDataCount.Total, 100) * noiseDataSettings.WarningConsecutiveMinutesPercentage);
            if (noiseDataCount.Warning > warningMargin)
            {
                logger.LogInformation($"Hive {hiveId} nosie warning!");
                return HiveStatus.Warning;
            }
            return HiveStatus.Ok;
        }
    }
}