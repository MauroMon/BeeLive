namespace BeeLive.Hive.TransferModels.Mapping
{
    public static class HiveMappingExtensions
    {
      
        public static HiveDto ToDto(this Core.Entities.Hive hive, decimal noise)
        {
            return new HiveDto()
            {
                Id = hive.Id,
                Noise = noise,
                Status = Enum.Parse<HiveStatus>(hive.Status.ToString())
            };
        }
       
    }
}