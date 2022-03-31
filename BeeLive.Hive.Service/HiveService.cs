using BeeLive.Hive.Core.Repositories;

namespace BeeLive.Hive.Service
{
    public class HiveService : IHiveService
    {
        IHiveRepository repository;

        public HiveService(IHiveRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Core.Entities.Hive> GetOrCreateHiveAsync(int hiveId)
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

        public async Task UpdateHive(Core.Entities.Hive hive)
        {
            await repository.AddAsync(hive);
        }
    }
}