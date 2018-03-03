using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;

namespace HealthNet.Integrations.Runners
{
  internal interface IFixtureRunner
  {
    IApplicationBuilder Configure(IApplicationBuilder app, IHealthNetConfiguration configuration, IEnumerable<ISystemChecker> checkers);
  }
}