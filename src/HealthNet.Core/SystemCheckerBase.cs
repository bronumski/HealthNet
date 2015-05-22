using System;
using System.Text.RegularExpressions;

namespace HealthNet
{
    public abstract class SystemCheckerBase : ISystemChecker
    {
        public virtual string SystemName
        {
            get { return Regex.Replace(GetType().Name.Replace("SystemChecker", string.Empty), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 "); }
        }

        public SystemCheckResult CheckSystem()
        {
            try
            {
                PerformCheck();

                return this.CreateGoodResult();
            }
            catch (Exception ex)
            {
                return this.CreateCriticalResult(ex.Message);
            }
        }

        protected abstract void PerformCheck();

        public virtual bool IsIntrusive { get { return false; } }
        public virtual bool IsVital { get { return true; } }
    }
}