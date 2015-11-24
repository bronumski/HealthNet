using System;

namespace HealthNet
{
    public abstract class HealthNetConfiguration : IHealthNetConfiguration
    {
        public virtual string Path { get { return "/api/healthcheck"; } }

        public TimeSpan DefaultSystemCheckTimeout { get { return TimeSpan.FromSeconds(10); } }
    }
}