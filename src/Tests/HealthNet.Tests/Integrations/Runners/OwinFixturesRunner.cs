using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthNet.Owin;
using Microsoft.AspNetCore.Builder;
using Microsoft.Owin.Builder;
using Owin;
using Env = System.Collections.Generic.IDictionary<string, object>;

namespace HealthNet.Integrations.Runners
{
  using AppFunc = Func<Env, Task>;
  class OwinFixturesRunner : IFixtureRunner
  {
    public IApplicationBuilder Configure(IApplicationBuilder app, IHealthNetConfiguration configuration, IEnumerable<ISystemChecker> checkers)
    {
      return app.UseOwin(setup => setup(next =>
      {
        var builder = new AppBuilder();
        builder.UseHealthNet(configuration, () => checkers);
        builder.Run(async context =>
        {
          context.Response.ContentType = "text/plain";
          await context.Response.WriteAsync("Hello World");
        });

        return builder.Build<AppFunc>();
      }));
    }
  }
}