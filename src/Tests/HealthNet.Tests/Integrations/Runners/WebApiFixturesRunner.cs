using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.AspNetCore.Builder;
using Microsoft.Owin.Builder;
using Owin;

namespace HealthNet.Integrations.Runners
{
  class WebApiFixturesRunner : IFixtureRunner
  {
    public IApplicationBuilder Configure(IApplicationBuilder app, IHealthNetConfiguration configuration, IEnumerable<ISystemChecker> checkers)
    {
      var httpConfiguration = new HttpConfiguration();
      httpConfiguration.Routes.MapHttpRoute(
          routeTemplate: configuration.Path.Remove(0, 1),
          name: "HealthCheck",
          defaults: new { Controller = "HealthCheck" }
          );

      var assemblyResolver = new AssembliesResolver();
      httpConfiguration.Services.Replace(typeof(IAssembliesResolver), assemblyResolver);

      httpConfiguration.DependencyResolver = new DependencyResolver(configuration, checkers);

      return app.UseOwin(setup => setup(next =>
      {
        var owinAppBuilder = new AppBuilder();
        owinAppBuilder.UseWebApi(httpConfiguration);
        return owinAppBuilder.Build<Func<IDictionary<string, object>, Task>>();
      }));
    }
  }
}