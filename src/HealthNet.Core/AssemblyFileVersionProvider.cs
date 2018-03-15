using System.Diagnostics;

namespace HealthNet
{
  public class AssemblyFileVersionProvider : IVersionProvider
  {
    private readonly IHealthNetConfiguration healthNetConfiguration;
    private static string cachedVersion;

    public AssemblyFileVersionProvider(IHealthNetConfiguration healthNetConfiguration)
    {
      this.healthNetConfiguration = healthNetConfiguration;
    }

    public string GetSystemVersion()
    {
      if (!string.IsNullOrEmpty(cachedVersion)) return cachedVersion;

      FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(healthNetConfiguration.GetType().Assembly.Location);

      cachedVersion = fvi.FileVersion;

      return cachedVersion;
    }
  }
}