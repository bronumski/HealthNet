using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            IEnumerable<SystemCheckResult> systemCheckResults = systemStateCheckers
                .Select(checker => CheckSystemState(intrusive, checker))
                .Select(checkTask => checkTask.Result).ToArray();

            var result = new HealthResult
            {
                CheckupDate = DateTime.UtcNow,
                Health = GetOverallHealth(systemCheckResults),
                SystemStates = systemCheckResults,
                SystemVersion = versionProvider.GetSystemVersion(),
                Host = Environment.MachineName
            };

            stopwatch.Stop();
            
            result.TimeTaken = stopwatch.Elapsed;
            
            return result;
        }

        private Task<SystemCheckResult> CheckSystemState(bool performeIntrusive, ISystemChecker systemChecker)
        {
            return Task<SystemCheckResult>.Factory.StartNew(() =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                
                var result = GetSystemCheckResult(performeIntrusive, systemChecker);
                
                stopwatch.Stop();

                result.TimeTaken = stopwatch.Elapsed;

                return result;
            });
        }

        private static SystemCheckResult GetSystemCheckResult(bool performeIntrusive, ISystemChecker systemChecker)
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