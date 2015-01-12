using System;
using System.Collections.Generic;

namespace HealthNet
{
    public class HealthResult
    {
        public string Host { get; set; }
        public DateTime CheckupDate { get; set; }
        public HealthState Health { get; set; }
        public IEnumerable<SystemCheckResult> SystemStates { get; set; }
        public string Version { get; set; }
    }
}