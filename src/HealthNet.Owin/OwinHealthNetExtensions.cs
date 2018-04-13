using System;
using System.Collections.Generic;
using Owin;

namespace HealthNet.Owin
{
  public static class OwinHealthNetExtensions
  {
    public static IAppBuilder UseHealthNet(
      this IAppBuilder appBuilder,
      IHealthNetConfiguration configuration,
      Func<IEnumerable<ISystemChecker>> systemCheckerResolver)
    {
      return appBuilder.UseHealthNet(configuration, new AssemblyFileVersionProvider(configuration), systemCheckerResolver);
    }

    public static IAppBuilder UseHealthNet(
      this IAppBuilder appBuilder,
      IHealthNetConfiguration configuration,
      IVersionProvider versionProvider,
      Func<IEnumerable<ISystemChecker>> systemCheckerResolver)
    {
      appBuilder.Use(typeof(HealthNetMiddleware), configuration, versionProvider, systemCheckerResolver);

      return appBuilder;
    }
  }
}