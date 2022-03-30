using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.Persistence
{
    public class ElasticSearchSettings
    {
        public string Uri { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NoiseDataIndex { get; set; }
        public string HiveIndex { get; set; }
    }
}
