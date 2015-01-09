using System.Diagnostics;
using System.Reflection;

namespace HealthNet
{
    public class VersionProvider : IVersionProvider
    {
        private static string CachedVersion;

        public string GetSystemVersion()
        {
            if (string.IsNullOrEmpty(CachedVersion))
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                CachedVersion = fvi.FileVersion;
            }
            return CachedVersion;
        }
    }
}