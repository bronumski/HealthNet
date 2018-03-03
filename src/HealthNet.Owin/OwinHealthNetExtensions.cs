using System;
using System.Collections.Generic;
using Owin;

namespace HealthNet
{
  public static class OwinHealthNetExtensions
  {
    public static IAppBuilder UseHealthNet(this IAppBuilder appBuilder, IHealthNetConfiguration configuration, Func<IEnumerable<ISystemChecker>> systemCheckerResolver)
    {
      appBuilder.Use(typeof(HealthNetMiddleware), configuration, systemCheckerResolver);

      return appBuilder;
    }
  }
}