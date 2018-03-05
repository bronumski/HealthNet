using Microsoft.AspNetCore.Builder;
using HealthNet.AspNetCore;

namespace HealthNet.Integrations.Runners
{
  class AspNetCoreFixturesRunner : IFixtureRunner
  {
    public IApplicationBuilder Configure(IApplicationBuilder app)
    {
      return app.UseHealthNetMiddleware();
    }
  }
}