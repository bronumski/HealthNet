using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

namespace HealthNet.Integrations.Runners
{
  class DependencyResolver : IDependencyResolver
  {
    private readonly IHealthNetConfiguration configuration;
    private readonly IVersionProvider versionProvider;
    private readonly IEnumerable<ISystemChecker> checkers;

    public DependencyResolver(IHealthNetConfiguration configuration,
      IVersionProvider versionProvider, IEnumerable<ISystemChecker> checkers)
    {
      this.configuration = configuration;
      this.versionProvider = versionProvider;
      this.checkers = checkers;
    }

    public void Dispose() { }

    public object GetService(Type serviceType)
    {
      var healthCheckController = versionProvider == null
        ? new HealthCheckController(configuration, checkers)
        : new HealthCheckController(configuration, versionProvider, checkers);
      return serviceType == typeof(HealthCheckController) ? healthCheckController : null;
    }

    public IEnumerable<object> GetServices(Type serviceType)
    {
      return Enumerable.Empty<object>();
    }

    public IDependencyScope BeginScope()
    {
      return this;
    }
  }
}