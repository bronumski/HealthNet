using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Env = System.Collections.Generic.IDictionary<string, object>;

namespace HealthNet
{
    using AppFunc = Func<Env, Task>;
    public class HealthNetMiddleware
    {
        private readonly AppFunc next;
        private readonly IHealthNetConfiguration configuration;
        private readonly Func<IEnumerable<ISystemChecker>> systemCheckerResolverFactory;

        public HealthNetMiddleware(AppFunc next, IHealthNetConfiguration configuration, Func<IEnumerable<ISystemChecker>> systemCheckerResolverFactory)
        {
            this.next = next;
            this.configuration = configuration;
            this.systemCheckerResolverFactory = systemCheckerResolverFactory;
        }

        public async Task Invoke(Env environment)
        {
            if (!IsCallToHealthCheck(environment)) await next.Invoke(environment);

            var responseHeaders = (IDictionary<string, string[]>) environment["owin.ResponseHeaders"];
            responseHeaders["Content-Type"] = new[] {Constants.Response.ContentType.Json};

            var responseStream = (Stream) environment["owin.ResponseBody"];

            var healthCheckService = new HealthCheckService(new VersionProvider(), systemCheckerResolverFactory());
            var result = healthCheckService.CheckHealth(IsIntrusive(environment));

            var contentLength = new HealthResultJsonSerializer().SerializeToStream(responseStream, result);

            responseHeaders["Content-Length"] = new[] { contentLength.ToString("D") };
        }

        private bool IsCallToHealthCheck(Env environment)
        {
            var path = environment["owin.RequestPath"].ToString();

            return path.ToLowerInvariant().StartsWith(configuration.Path);
        }

        private bool IsIntrusive(Env environment)
        {
            var querystring = environment["owin.RequestQueryString"].ToString();
            var split = querystring.Split(new[] {'&'});
            return split.Any(x => x.ToLower() == "intrusive=true");
        }
    }
}