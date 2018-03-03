using System.Collections.Generic;
using HealthNet.Nancy;
using Microsoft.AspNetCore.Builder;
using Nancy;
using Nancy.Owin;
using Nancy.Testing;
using Owin;

namespace HealthNet.Integrations.Runners
{
  class NancyFixturesRunner : IFixtureRunner
  {
    public IApplicationBuilder Configure(IApplicationBuilder app, IHealthNetConfiguration healthNetConfiguration, IEnumerable<ISystemChecker> checkers)
    {
      return app
          .UseOwin(setup => setup
          .UseNancy(x =>
              x.Bootstrapper =
                  new ConfigurableBootstrapper(
                      y => y.Module<HealthNetModule>().Module<FooModule>().DisableAutoRegistrations().Dependency(checkers).Dependency(healthNetConfiguration))));
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