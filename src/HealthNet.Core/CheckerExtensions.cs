using System;

namespace HealthNet
{
    public static class CheckerExtensions
    {
        public static SystemCheckResult CreateGoodResult(this ISystemChecker systemChecker)
        {
            return systemChecker.CreateResult(HealthState.Good, null);
        }

        public static SystemCheckResult CreateCriticalResult(this ISystemChecker systemChecker, string message)
        {
            return systemChecker.CreateResult(HealthState.Critical, message);
        }

        public static SystemCheckResult CreateSkippedResult(this ISystemChecker systemChecker)
        {
            return systemChecker.CreateResult(HealthState.Undetermined, "Intrusive check skipped");
        }

        public static SystemCheckResult CreateTimeoutResult(this ISystemChecker systemChecker)
        {
            return systemChecker.CreateResult(HealthState.Serious, "System check timed out");
        }

        public static SystemCheckResult CreateResult(this ISystemChecker systemChecker, HealthState state, string message)
        {
            return new SystemCheckResult
            {
                Health = state,
                IsVital = systemChecker.IsVital,
                SystemName = systemChecker.SystemName,
                Message = message
            };
        }
    }
}