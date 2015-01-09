namespace HealthNet
{
    public class HealthCheckService
    {
        public HealthResult CheckHealth()
        {
            return new HealthResult { State = HealthState.Good };
        }
    }
}