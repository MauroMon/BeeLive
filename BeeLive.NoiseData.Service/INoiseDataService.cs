using BeeLive.NoiseData.TransferModels;

namespace BeeLive.NoiseData.Service
{
    public interface INoiseDataService
    {
        Task InsertNoiseData(NoiseDataDto noiseDataDto);
    }
}