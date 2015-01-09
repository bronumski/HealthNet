namespace HealthNet
{
    public interface ISystemStateChecker
    {
        SystemStateResult CheckSystemState();
        bool IsIntrusive { get; }
    }
}