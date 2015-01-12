using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthNet
{
    public class HealthCheckService
    {
        private readonly IVersionProvider versionProvider;
        private readonly IEnumerable<ISystemChecker> systemStateCheckers;

        public HealthCheckService(IVersionProvider versionProvider, IEnumerable<ISystemChecker> systemStateCheckers)
        {
            this.versionProvider = versionProvider;
            this.systemStateCheckers = systemStateCheckers;
        }

        public HealthResult CheckHealth(bool intrusive = false)
        {
            IEnumerable<SystemCheckResult> systemCheckResults = systemStateCheckers
                .Select(checker => CheckSystemState(intrusive, checker))
                .Select(checkTask => checkTask.Result).ToArray();

            return new HealthResult
            {
                CheckupDate = DateTime.UtcNow,
                Health = GetOverallHealth(systemCheckResults),
                SystemStates = systemCheckResults,
                Version = versionProvider.GetSystemVersion(),
                Host = Environment.MachineName
            };
        }

        private Task<SystemCheckResult> CheckSystemState(bool performeIntrusive, ISystemChecker systemChecker)
        {
            return Task<SystemCheckResult>.Factory.StartNew(() =>
            {
                if (!performeIntrusive && systemChecker.IsIntrusive)
                {
                    return systemChecker.CreateSkippedResult();
                }
                try
                {
                    return systemChecker.CheckSystem();
                }
                catch (Exception ex)
                {
                    return systemChecker.CreateCriticalResult(ex.Message);
                }
            });
        }

        private static HealthState GetOverallHealth(IEnumerable<SystemCheckResult> systemStateResults)
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