using BeeLive.Core.Repositories;
using BeeLive.NoiseData.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.NoiseData.Core.Repositories
{
    public interface INoiseDataRepository : IRepository<Entities.NoiseData>
    {
        Task<NoiseDataCount> CountAsync(DateTime dtFrom, DateTime dtTo, int hiveId);
        Task<NoiseDataAvg> GetAverage(DateTime dtStart, DateTime dtEnd, int HiveId);
        Task<decimal> GetLastNoiseData(int HiveId);
    }
}
