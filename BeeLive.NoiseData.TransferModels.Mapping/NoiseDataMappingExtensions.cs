namespace BeeLive.NoiseData.TransferModels.Mapping
{
    public static class NoiseDataMappingExtensions
    {
        public static Core.Entities.NoiseData ToEntity(this NoiseDataDto noiseDataDto)
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