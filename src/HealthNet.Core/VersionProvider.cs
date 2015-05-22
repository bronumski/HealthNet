using System.Diagnostics;

namespace HealthNet
{
    public class VersionProvider : IVersionProvider
    {
        private readonly IHealthNetConfiguration healthNetConfiguration;
        private static string CachedVersion;

        public VersionProvider(IHealthNetConfiguration healthNetConfiguration)
        {
            this.healthNetConfiguration = healthNetConfiguration;
        }

        public string GetSystemVersion()
        {
            if (!string.IsNullOrEmpty(CachedVersion)) return CachedVersion;

            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(healthNetConfiguration.GetType().Assembly.Location);
            
            CachedVersion = fvi.FileVersion;
            
            return CachedVersion;
        }
    }
}