using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin.Builder;
using Owin;
using Env = System.Collections.Generic.IDictionary<string, object>;

namespace HealthNet.Integrations.Runners
{
  using AppFunc = Func<Env, Task>;

  class WebApiFixturesRunner : IFixtureRunner
  {
    public IApplicationBuilder Configure(IApplicationBuilder app)
    {
      var configuration = app.ApplicationServices.GetService<IHealthNetConfiguration>();
      var httpConfiguration = new HttpConfiguration();
      httpConfiguration.Routes.MapHttpRoute(
          routeTemplate: configuration.Path.Remove(0, 1),
          name: "HealthCheck",
          defaults: new { Controller = "HealthCheck" }
          );

      var assemblyResolver = new AssembliesResolver();
      httpConfiguration.Services.Replace(typeof(IAssembliesResolver), assemblyResolver);

      var versionProvider = app.ApplicationServices.GetService<IVersionProvider>();
      httpConfiguration.DependencyResolver = new DependencyResolver(
        configuration,
        versionProvider,
        app.ApplicationServices.GetService<IEnumerable<ISystemChecker>>());

      return app.UseOwin(setup => setup(next =>
      {
        var builder = new AppBuilder();
        builder.UseWebApi(httpConfiguration);
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