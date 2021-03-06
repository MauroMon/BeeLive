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
        private ILogger logger;

        public HiveService(INoiseDataService noiseDataService, ILogger<INoiseDataService> logger)
        {
            this.noiseDataService = noiseDataService;
            this.logger = logger;
        }

        /// <summary>
        /// Get Hive data
        /// </summary>
        /// <param name="hiveId">Hive id</param>
        /// <returns>hive data</returns>
        public async Task<HiveDto> GetHive(int hiveId)
        {
            return new HiveDto()
            {
                Id = hiveId,
                Status = Enum.Parse<HiveStatus>((await noiseDataService.GetHiveStatusAsync(hiveId)).ToString()),
                Decibel = await noiseDataService.GetLastNoiseDataAsync(hiveId)
            };
        }
    }
}