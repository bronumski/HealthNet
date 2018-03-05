using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Env = System.Collections.Generic.IDictionary<string, object>;

namespace HealthNet.Owin
{
  using AppFunc = Func<Env, Task>;
  public class HealthNetMiddleware
  {
    private readonly AppFunc next;
    private readonly IHealthNetConfiguration configuration;
    private readonly IVersionProvider versionProvider;
    private readonly Func<IEnumerable<ISystemChecker>> systemCheckerResolverFactory;

    public HealthNetMiddleware(AppFunc next,
      IHealthNetConfiguration configuration,
      IVersionProvider versionProvider,
      Func<IEnumerable<ISystemChecker>> systemCheckerResolverFactory)
    {
      this.next = next;
      this.configuration = configuration;
      this.versionProvider = versionProvider;
      this.systemCheckerResolverFactory = systemCheckerResolverFactory;
    }

    public async Task Invoke(Env environment)
    {
      if (IsCallToHealthCheck(environment))
      {
        var responseHeaders = (IDictionary<string, string[]>)environment["owin.ResponseHeaders"];
        responseHeaders["Content-Type"] = new[] { $"{Constants.Response.ContentType.Json}; charset=utf-8" };

        var responseStream = (Stream)environment["owin.ResponseBody"];

        var healthCheckService = new HealthCheckService(configuration,
          versionProvider ?? new VersionProvider(configuration),
          systemCheckerResolverFactory());
        var result = healthCheckService.CheckHealth(IsIntrusive(environment));

        using (var writeStream = new MemoryStream())
        {
          var contentLength = new HealthResultJsonSerializer().SerializeToStream(writeStream, result);
          responseHeaders["Content-Length"] = new[] { contentLength.ToString("D") };
          writeStream.Position = 0;

          await writeStream.CopyToAsync(responseStream);
        }
      }
      else
        await next.Invoke(environment);
    }

    private bool IsCallToHealthCheck(Env environment)
    {
      var path = environment["owin.RequestPath"].ToString();

      return path.ToLowerInvariant().StartsWith(configuration.Path);
    }

    private bool IsIntrusive(Env environment)
    {
      var querystring = environment["owin.RequestQueryString"].ToString();
      var split = querystring.Split('&');
      return split.Any(x => x.ToLower() == "intrusive=true");
    }
  }
}