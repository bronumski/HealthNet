using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthNet
{
    public class HealthCheckService
    {
        private readonly IVersionProvider versionProvider;
        private readonly IEnumerable<ISystemStateChecker> systemStateCheckers;

        public HealthCheckService(IVersionProvider versionProvider, IEnumerable<ISystemStateChecker> systemStateCheckers)
        {
            this.versionProvider = versionProvider;
            this.systemStateCheckers = systemStateCheckers;
        }

        public HealthResult CheckHealth(bool intrusive = false)
        {
            IEnumerable<SystemStateResult> systemStateResults = systemStateCheckers
                .Select(x => CheckSystemState(intrusive, x)).Select(x => x.Result).ToArray();

            return new HealthResult
            {
                CheckupDate = DateTime.UtcNow,
                Health = GetOverallHealth(systemStateResults),
                Systems = systemStateResults,
                Version = versionProvider.GetSystemVersion()
            };
        }

        private Task<SystemStateResult> CheckSystemState(bool performeIntrusive, ISystemStateChecker systemState)
        {
            return Task<SystemStateResult>.Factory.StartNew(() =>
            {
                if (!performeIntrusive && systemState.IsIntrusive)
                {
                    return new SystemStateResult
                    {
                        Health = HealthState.Undetermined,
                    };
                }
                return systemState.CheckSystemState();
            });
        }

        private static HealthState GetOverallHealth(IEnumerable<SystemStateResult> systemStateResults)
        {
            var overallState = HealthState.Good;
            
            foreach (var systemStateResult in systemStateResults)
            {
                if (systemStateResult.IsVital && systemStateResult.Health == HealthState.Critical)
                {
                    return HealthState.Critical;
                }
                
                if (systemStateResult.Health > HealthState.Good)
                {
                    overallState = HealthState.Serious;
                }
            }
            return overallState;
        }
    }
}