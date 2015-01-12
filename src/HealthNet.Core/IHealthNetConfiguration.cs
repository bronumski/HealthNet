namespace HealthNet
{
    
    public interface IHealthNetConfiguration
    {
        string Path { get; }
    }

    public class HealthNetConfiguration : IHealthNetConfiguration
    {
        public string Path { get { return "/api/healthcheck"; } }
    }
}