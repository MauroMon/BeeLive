using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.Hive.Core.Entities
{
    public class Hive
    {
        public int Id { get; set; } 
        public HiveStatus Status { get; set; }
    }
}
