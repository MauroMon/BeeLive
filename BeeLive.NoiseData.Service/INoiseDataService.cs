using BeeLive.NoiseData.TransferModels;

namespace BeeLive.NoiseData.Service
{
    public interface INoiseDataService
    {
        Task<NoiseDataCountDto> GetWarningCountAsync(DateTime dtFrom, DateTime dtTo, int hiveId);
        Task InsertNoiseData(NoiseDataDto noiseDataDto);

        Task<decimal> GetLastNoiseDataAsync(int hiveId);
    }
}