using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.NoiseData.Core.Settings
{
    public class NoiseDataSettings
    {
        public int HoursToCheck { get; set; } = 1;
        public int MinRequiredValues { get; set; }
        public int WarningNoiseIncreasePercentage { get; set; }
        public int WarningConsecutiveMinutes { get; set; }
        public int WarningConsecutiveMinutesPercentage { get; set; }
        public int AlarmConsecutiveMinutes { get; set; }
        public int AlarmCOnsecutiveMinutesPercentage { get; set; }
    }
}
