using BeeLive.Core.Repositories;
using BeeLive.NoiseData.Core.Repositories;
using BeeLive.NoiseData.Core.Settings;
using BeeLive.NoiseData.TransferModels;
using BeeLive.NoiseData.TransferModels.Mapping;
using Microsoft.Extensions.Options;

namespace BeeLive.NoiseData.Service
{
    public class NoiseDataService : INoiseDataService
    {
        private INoiseDataRepository repository;
        private NoiseDataSettings config;

        public NoiseDataService(INoiseDataRepository noiseDataRepository, IOptions<NoiseDataSettings> config)
        {
            this.repository = noiseDataRepository;
            this.config = config.Value;
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

            await repository.AddAsync(noiseDataDto.ToEntity());
        }
    }
}