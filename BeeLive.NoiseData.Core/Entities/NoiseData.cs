using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.NoiseData.Core.Entities
{
    public class NoiseData
    {
        public DateTime Dt { get; set; }
        public decimal Decibel { get; set; }
        public int HiveId { get; set; }
        public bool Warning { get; set; }
    }
}
