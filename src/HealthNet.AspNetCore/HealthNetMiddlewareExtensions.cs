using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HealthNet.AspNetCore
{
  public static class HealthNetMiddlewareExtensions
  {
    private static readonly Type VersionProviderType = typeof(IVersionProvider);
    private static readonly Type SystemCheckerType = typeof(ISystemChecker);
    public static IApplicationBuilder UseHealthNet(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<HealthNetMiddleware>();
    }

    public static IServiceCollection AddHealthNet(this IServiceCollection service)
    {
      return service.AddTransient<HealthCheckService>();
    }

    public static IServiceCollection AddHealthNet<THealthNetConfig>(this IServiceCollection service, bool autoRegisterCheckers = true) where THealthNetConfig : class, IHealthNetConfiguration
    {
      var assembyTypes = typeof(THealthNetConfig).Assembly.GetTypes();

      service.AddSingleton<IHealthNetConfiguration, THealthNetConfig>();

      var versionProvider = assembyTypes
        .FirstOrDefault(x => x.IsClass && !x.IsAbstract && VersionProviderType.IsAssignableFrom(x));

      service.AddSingleton(VersionProviderType, versionProvider ?? typeof(AssemblyFileVersionProvider));

      if (autoRegisterCheckers)
      {
        var systemCheckers = assembyTypes
          .Where(x => x.IsClass && !x.IsAbstract && SystemCheckerType.IsAssignableFrom(x));

        foreach (var checkerType in systemCheckers)
        {
          service.AddTransient(SystemCheckerType, checkerType);
        }
      }

      return service.AddHealthNet();
    }
  }
}