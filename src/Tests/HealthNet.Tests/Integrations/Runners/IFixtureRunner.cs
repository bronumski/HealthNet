using Microsoft.AspNetCore.Builder;

namespace HealthNet.Integrations.Runners
{
  internal interface IFixtureRunner
  {
    IApplicationBuilder Configure(IApplicationBuilder app);
  }
}