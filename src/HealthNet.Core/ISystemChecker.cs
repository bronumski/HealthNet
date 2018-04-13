namespace HealthNet
{
  public interface ISystemChecker
  {
    SystemCheckResult CheckSystem();
    bool IsIntrusive { get; }
    string SystemName { get; }
    bool IsVital { get; }
  }
}