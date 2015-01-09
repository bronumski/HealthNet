using System;
using System.Collections.Generic;

namespace HealthNet
{
    public class HealthResult
    {
        public HealthState Health { get; set; }
        public IEnumerable<SystemStateResult> Systems { get; set; }
        public DateTime CheckupDate { get; set; }
        public string Version { get; set; }
    }
}