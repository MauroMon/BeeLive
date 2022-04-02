using BeeLive.NoiseData.TransferModels;

namespace BeeLive.NoiseData.Service
{
    public interface INoiseDataService
    {
        Task InsertNoiseDataAsync(NoiseDataDto noiseDataDto);

        Task<decimal> GetLastNoiseDataAsync(int hiveId);
        Task<NoiseDataStatus> GetHiveStatusAsync(int hiveId);
    }
}