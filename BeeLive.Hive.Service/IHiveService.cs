using BeeLive.Hive.TransferModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.Hive.Service
{
    public interface IHiveService
    {
        Task<HiveDto> GetHive(int HiveId);
    }
}
