using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace HealthNet
{
  public class HealthCheckController : ApiController
  {
    private readonly IHealthNetConfiguration configuration;
    private readonly IVersionProvider versionProvider;
    private readonly IEnumerable<ISystemChecker> checkers;

    public HealthCheckController(
      IHealthNetConfiguration configuration,
      IEnumerable<ISystemChecker> checkers)
    : this(configuration, new AssemblyFileVersionProvider(configuration), checkers)
    {

    }
    public HealthCheckController(
      IHealthNetConfiguration configuration,
      IVersionProvider versionProvider,
      IEnumerable<ISystemChecker> checkers)
    {
      this.configuration = configuration;
      this.versionProvider = versionProvider;
      this.checkers = checkers;
    }

    public HttpResponseMessage Get([FromUri] bool intrusive = false)
    {
      return new HttpResponseMessage
      {
        Content = new JsonHealthResultContent(
          new HealthCheckService(configuration, versionProvider, checkers).CheckHealth(intrusive))
      };
    }
  }
}