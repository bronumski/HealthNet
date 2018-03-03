using Microsoft.AspNetCore.Builder;

namespace HealthNet.AspNetCore
{
  public static class HealthNetMiddlewareExtensions
  {
    public static IApplicationBuilder UseHealthNetMiddleware(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<HealthNetMiddleware>();
    }
  }
}