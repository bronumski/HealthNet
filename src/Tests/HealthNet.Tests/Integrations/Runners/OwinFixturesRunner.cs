using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthNet.Owin;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin.Builder;
using Owin;
using Env = System.Collections.Generic.IDictionary<string, object>;

namespace HealthNet.Integrations.Runners
{
  using AppFunc = Func<Env, Task>;
  class OwinFixturesRunner : IFixtureRunner
  {
    public IApplicationBuilder Configure(IApplicationBuilder app)
    {
      return app.UseOwin(setup => setup(next =>
      {
        var builder = new AppBuilder();
        var versionProvider = app.ApplicationServices.GetService<IVersionProvider>();
        if (versionProvider != null)
        {
          builder.UseHealthNet(
            app.ApplicationServices.GetService<IHealthNetConfiguration>(),
            versionProvider,
            () => app.ApplicationServices.GetService<IEnumerable<ISystemChecker>>());
        }
        else
        {
          builder.UseHealthNet(
            app.ApplicationServices.GetService<IHealthNetConfiguration>(),
            () => app.ApplicationServices.GetService<IEnumerable<ISystemChecker>>());
        }

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