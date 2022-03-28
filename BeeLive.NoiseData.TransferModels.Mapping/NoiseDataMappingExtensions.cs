namespace BeeLive.NoiseData.TransferModels.Mapping
{
    public static class NoiseDataMappingExtensions
    {
        public static BeeLive.Core.Entities.NoiseData ToEntity(this BeeLive.NoiseData.TransferModels.NoiseDataDto noiseDataDto)
        {
            return new Core.Entities.NoiseData()
            {
                Decibel = noiseDataDto.Decibel,
                HiveId = noiseDataDto.HiveId,
                Dt = DateTime.UtcNow
            };
        }
    }
}