using System;
using System.Collections.Generic;
using System.IO;
using Nancy;

namespace HealthNet.Nancy
{
    public class HealthNetModule : NancyModule
    {
        public HealthNetModule(IHealthNetConfiguration configuration, IEnumerable<ISystemChecker> systemCheckers)
            : base(configuration.Path)
        {
            Get[""] = p =>
            {
                var healthChecker = new HealthCheckService(configuration, new VersionProvider(configuration), systemCheckers);

                var intrusive = false;
                if (Request.Query.intrusive != null)
                {
                    intrusive = Request.Query.intrusive == "true";
                }

                Action<Stream> performeHealthCheck = stream => new HealthResultJsonSerializer().SerializeToStream(stream, healthChecker.CheckHealth(intrusive));

                return new Response { Contents = performeHealthCheck, ContentType = Constants.Response.ContentType.Json + "; charset=utf-8", StatusCode = HttpStatusCode.OK };
            };
        }
    }
}