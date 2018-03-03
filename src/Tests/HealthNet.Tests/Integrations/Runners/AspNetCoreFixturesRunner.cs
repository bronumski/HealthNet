using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using HealthNet.AspNetCore;

namespace HealthNet.Integrations.Runners
{
  class AspNetCoreFixturesRunner : IFixtureRunner
  {
    public IApplicationBuilder Configure(IApplicationBuilder app, IHealthNetConfiguration configuration, IEnumerable<ISystemChecker> checkers)
    {
      return app.UseHealthNetMiddleware();
    }
  }
}