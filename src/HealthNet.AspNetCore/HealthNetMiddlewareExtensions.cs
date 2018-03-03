using Microsoft.AspNetCore.Builder;

namespace HealthNet
{
  public static class HealthNetMiddlewareExtensions
  {
    public static IApplicationBuilder UseHealthNetMiddleware(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<HealthNetMiddleware>();
    }
  }
}