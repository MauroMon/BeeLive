using BeeLive.Hive.Core.Repositories;
using BeeLive.Hive.TransferModels;
using BeeLive.Hive.TransferModels.Mapping;
using BeeLive.NoiseData.Core.Settings;
using BeeLive.NoiseData.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeeLive.Hive.Service
{
    public class HiveService : IHiveService
    {
        private IHiveRepository repository;
        private INoiseDataService noiseDataService;
        private NoiseDataSettings noiseDataSettings;
        private ILogger logger;

        public HiveService(IHiveRepository repository, INoiseDataService noiseDataService, IOptions<NoiseDataSettings> noiseDataSettings, ILogger<INoiseDataService> logger)
        {
            this.repository = repository;
            this.noiseDataService = noiseDataService;
            this.noiseDataSettings = noiseDataSettings.Value;
            this.logger = logger;
        }

        /// <summary>
        /// Get an Hive With noise data
        /// </summary>
        /// <param name="HiveId">Hive Id</param>
        /// <returns>An hive with noise data</returns>
        public async Task<HiveDto> GetHive(int HiveId)
        {
            //update the status
            var hive = await UpdateHiveStatusAsync(HiveId);
            return hive.ToDto(await noiseDataService.GetLastNoiseDataAsync(HiveId));
        }

        /// <summary>
        /// Get an Hive or create if not exists
        /// </summary>
        /// <param name="hiveId">Hive Id</param>
        /// <returns>An hive</returns>
        private async Task<Core.Entities.Hive> GetOrCreateHiveAsync(int hiveId)
        {
            Core.Entities.Hive hive = await repository.GetAsync(hiveId);
            if (hive == null)
            {
                //hive not found... inserting
                hive = new Hive.Core.Entities.Hive()
                {
                    Id = hiveId,
                    Status = Hive.Core.Entities.HiveStatus.Ok
                };
                await repository.AddAsync(hive);
            }
            return hive;
        }

        /// <summary>
        /// Update Hive status (ok, warn, alert) 
        /// </summary>
        /// <param name="hiveId">Hive ID</param>
        /// <returns>The Hive with the updates status</returns>
        private async Task<Core.Entities.Hive> UpdateHiveStatusAsync(int hiveId)
        {
            Hive.Core.Entities.Hive hive = await GetOrCreateHiveAsync(hiveId);
            var noiseDataCount = await noiseDataService.GetWarningCountAsync(DateTime.UtcNow.AddMinutes(-noiseDataSettings.WarningConsecutiveMinutes), DateTime.UtcNow, hiveId);
            if (hive.Status == Core.Entities.HiveStatus.Ok)
            {
                //there is a warning noise but hive is ok... need to check if hive need to be update to warning
                var warningMargin = Math.Floor(decimal.Divide(noiseDataCount.Total, 100) * noiseDataSettings.WarningConsecutiveMinutesPercentage);
                if (noiseDataCount.Warning > warningMargin)
                {
                    logger.LogInformation($"Hive {hiveId} nosie warning!");
                    hive.Status = Core.Entities.HiveStatus.Warning;
                    await repository.AddAsync(hive);
                }
            }
            else if (hive.Status == Core.Entities.HiveStatus.Ok)
            {
                //noise in not warning but hive is warning... need to check if hive can be restored to normal status
                var notWarningMargin = Math.Floor(decimal.Divide(noiseDataCount.Total - noiseDataCount.Warning, 100) * noiseDataSettings.WarningConsecutiveMinutesPercentage);
                if (noiseDataCount.Warning < notWarningMargin)
                {
                    //hive can be restored to normal status
                    logger.LogInformation($"Hive {hiveId} noise back to normal");
                    hive.Status = Core.Entities.HiveStatus.Ok;
                    await repository.AddAsync(hive);
                }
            }
            return hive;
        }
    }
}