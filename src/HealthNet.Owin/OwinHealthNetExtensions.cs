using System;
using System.Collections.Generic;
using Owin;

namespace HealthNet
{
    public static class OwinHealthNetExtensions
    {
        public static IAppBuilder UseHealthNet(this IAppBuilder appBuilder, Func<IEnumerable<ISystemChecker>> systemCheckerResolver)
        {
            appBuilder.UseHealthNet("/api/healthcheck", systemCheckerResolver);

            return appBuilder;
        }

        public static IAppBuilder UseHealthNet(this IAppBuilder appBuilder, string healthcheckPath, Func<IEnumerable<ISystemChecker>> systemCheckerResolver)
        {
            appBuilder.Use(typeof(HealthNetMiddleware), healthcheckPath, systemCheckerResolver);

            return appBuilder;
        }
    }
}