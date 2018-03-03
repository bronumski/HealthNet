using System.Collections.Generic;
using System.Linq;
using HealthNet.Owin;
using Microsoft.AspNetCore.Builder;
using Owin;

namespace HealthNet.Integrations.Runners
{
  class OwinFixturesRunner : IFixtureRunner
  {
    public IApplicationBuilder Configure(IApplicationBuilder app, IHealthNetConfiguration configuration, IEnumerable<ISystemChecker> checkers)
    {
      //return app.UseOwin(setup => setup.UseHealthNet(configuration, () =>
      //{
      //  var systemCheckers = checkers as ISystemChecker[] ?? checkers.ToArray();
      //  return systemCheckers;
      //});
      return null;
    }
  }
}