namespace HealthNet
{
    public interface ISystemChecker
    {
        SystemCheckResult CheckSystem();
        bool IsIntrusive { get; }
        string SystemName { get; }
        bool IsVital { get; }
    }

    public class TestSystemChecker : ISystemChecker
    {
        public SystemCheckResult CheckSystem()
        {
            if (true == true)
            {
                return this.CreateCriticalResult("Some error");
            }
            return this.CreateGoodResult();
        }

        public bool IsIntrusive { get { return false; } }
        public string SystemName { get { return "Test Checker"; } }
        public bool IsVital { get { return true; } }
    }
}