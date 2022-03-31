using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.Hive.Service
{
    public interface IHiveService
    {
        Task<Core.Entities.Hive> GetOrCreateHiveAsync(int HiveId);
        Task UpdateHive(Core.Entities.Hive hive);
    }
}
