using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HealthNet.AspNetCore
{
  public static class HealthNetMiddlewareExtensions
  {
    public static IApplicationBuilder UseHealthNetMiddleware(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<HealthNetMiddleware>();
    }

    public static IServiceCollection AddHealthNet(this IServiceCollection service, IHealthNetConfiguration config)
    {
      service.AddTransient(x => config);
      service.AddSingleton<IVersionProvider, VersionProvider>();
      return service.AddTransient<HealthCheckService>();
    }
  }
}