namespace HealthNet
{
    public abstract class HealthNetConfiguration : IHealthNetConfiguration
    {
        public virtual string Path { get { return "/api/healthcheck"; } }
    }
}