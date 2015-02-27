using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace HealthNet
{
    public class HealthCheckController : ApiController
    {
        private readonly IEnumerable<ISystemChecker> checkers;

        public HealthCheckController(IEnumerable<ISystemChecker> checkers)
        {
            this.checkers = checkers;
        }

        public HttpResponseMessage Get([FromUri] bool intrusive = false)
        {
            return new HttpResponseMessage
            {
                Content = new JsonHealthResultContent(new HealthCheckService(new VersionProvider(), checkers).CheckHealth(intrusive))
            };
        }
    }
}