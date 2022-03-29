using BeeLive.Core.Repositories;
using BeeLive.NoiseData.Core.Repositories;
using BeeLive.NoiseData.TransferModels;
using BeeLive.NoiseData.TransferModels.Mapping;

namespace BeeLive.NoiseData.Service
{
    public class NoiseDataService : INoiseDataService
    {
        private INoiseDataRepository repository;

        public NoiseDataService(INoiseDataRepository noiseDataRepository)
        {
            this.repository = noiseDataRepository;
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
            await repository.AddAsync(noiseDataDto.ToEntity());
        }
    }
}