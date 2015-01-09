using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Env = System.Collections.Generic.IDictionary<string, object>;

namespace HealthNet
{
    using AppFunc = Func<Env, Task>;
    public class HealthNetMiddleware
    {
        private readonly AppFunc next;
        private readonly Func<IEnumerable<ISystemChecker>> systemCheckerResolverFactory;
        private readonly string healthcheckPath;

        public HealthNetMiddleware(AppFunc next, string healthcheckPath, Func<IEnumerable<ISystemChecker>> systemCheckerResolverFactory)
        {
            this.next = next;
            this.systemCheckerResolverFactory = systemCheckerResolverFactory;
            this.healthcheckPath = healthcheckPath;
        }

        public async Task Invoke(Env environment)
        {
            if (!IsCallToHealthCheck(environment)) await next.Invoke(environment);

            var responseHeaders = (IDictionary<string, string[]>) environment["owin.ResponseHeaders"];
            responseHeaders["Content-Type"] = new[] {"application/json"};

            var responseStream = (Stream) environment["owin.ResponseBody"];

            var healthCheckService = new HealthCheckService(new VersionProvider(), systemCheckerResolverFactory());
            var result = healthCheckService.CheckHealth();

            var contentLength = new HealthResultJsonSerializer().SerializeToStream(responseStream, result);

            responseHeaders["Content-Length"] = new[] { contentLength.ToString("D") };
        }

        private bool IsCallToHealthCheck(Env environment)
        {
            var path = environment["owin.RequestPath"].ToString();

            return path.ToLowerInvariant().StartsWith(healthcheckPath);
        }
    }
}