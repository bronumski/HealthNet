using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

namespace HealthNet.Integrations.Runners
{
  class DependencyResolver : IDependencyResolver
  {
    private readonly IHealthNetConfiguration configuration;
    private readonly IEnumerable<ISystemChecker> checkers;

    public DependencyResolver(IHealthNetConfiguration configuration, IEnumerable<ISystemChecker> checkers)
    {
      this.configuration = configuration;
      this.checkers = checkers;
    }

    public void Dispose() { }

    public object GetService(Type serviceType)
    {
      return serviceType == typeof(HealthCheckController) ? new HealthCheckController(configuration, checkers) : null;
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