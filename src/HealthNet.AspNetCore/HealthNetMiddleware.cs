using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HealthNet.AspNetCore
{
  public class HealthNetMiddleware
  {
    private readonly RequestDelegate next;
    private readonly IHealthNetConfiguration configuration;
    private readonly Func<IEnumerable<ISystemChecker>> systemCheckerResolverFactory;

    public HealthNetMiddleware(RequestDelegate next, IHealthNetConfiguration configuration, Func<IEnumerable<ISystemChecker>> systemCheckerResolverFactory)
    {
      this.next = next;
      this.configuration = configuration;
      this.systemCheckerResolverFactory = systemCheckerResolverFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      if (IsCallToHealthCheck(context))
      {
        var responseHeaders = context.Request.Headers;
        responseHeaders["Content-Type"] = new[] { $"{Constants.Response.ContentType.Json}; charset=utf-8" };

        var responseStream = context.Response.Body;

        var healthCheckService = new HealthCheckService(configuration, new VersionProvider(configuration),
            systemCheckerResolverFactory());
        var result = healthCheckService.CheckHealth(IsIntrusive(context));

        using (var writeStream = new MemoryStream())
        {
          var contentLength = new HealthResultJsonSerializer().SerializeToStream(writeStream, result);
          responseHeaders["Content-Length"] = new[] { contentLength.ToString("D") };
          writeStream.Position = 0;

          await writeStream.CopyToAsync(responseStream);
        }
      }
      else
        await next(context);
    }

    private bool IsCallToHealthCheck(HttpContext context)
    {
      return context.Request.Path.Value.ToLowerInvariant().StartsWith(configuration.Path);
    }

    private bool IsIntrusive(HttpContext context)
    {
      return context.Request.Query
        .Where(x => x.Key.ToLower() == "intrusive")
        .Any(x => x.Value.FirstOrDefault()?.ToLower() == "true");
    }
  }
}