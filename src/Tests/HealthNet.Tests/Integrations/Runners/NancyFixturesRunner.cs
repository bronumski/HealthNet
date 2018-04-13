using System.Collections.Generic;
using HealthNet.Nancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using Nancy.Owin;
using Nancy.Testing;

namespace HealthNet.Integrations.Runners
{
  class NancyFixturesRunner : IFixtureRunner
  {
    public IApplicationBuilder Configure(IApplicationBuilder app)
    {
      return app
          .UseOwin(setup => setup
          .UseNancy(x =>
              x.Bootstrapper =
                  new ConfigurableBootstrapper(
                      y => y
                        .Module<HealthNetModule>()
                        .Module<FooModule>()
                        .DisableAutoRegistrations()
                        .Dependency(app.ApplicationServices.GetService<IVersionProvider>())
                        .Dependency(app.ApplicationServices.GetService<IEnumerable<ISystemChecker>>())
                        .Dependency(app.ApplicationServices.GetService<IHealthNetConfiguration>()))));
    }

    class FooModule : NancyModule
    {
      public FooModule() : base("Foo")
      {
        Get[""] = p => "Hello World";
      }
    }
  }
}