using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;
using Owin;

namespace HealthNet.Integrations.Runners
{
    class WebApiFixturesRunner : IFixtureRunner
    {
        public IAppBuilder Configure(IAppBuilder app, IHealthNetConfiguration configuration, IEnumerable<ISystemChecker> checkers)
        {
            var httpConfiguration = new HttpConfiguration();
            httpConfiguration.Routes.MapHttpRoute(
                name: "HealthCheck",
                routeTemplate: "api/healthcheck",
                defaults: new { Controller = "HealthCheck" }
            );

            
            var assemblyResolver = new AssembliesResolver();
            httpConfiguration.Services.Replace(typeof(IAssembliesResolver), assemblyResolver);

            httpConfiguration.DependencyResolver = new DependencyResolver(checkers);

            return app.UseWebApi(httpConfiguration);
        }
    }

    public class DependencyResolver : IDependencyResolver
    {
        private readonly IEnumerable<ISystemChecker> checkers;

        public DependencyResolver(IEnumerable<ISystemChecker> checkers)
        {
            this.checkers = checkers;
        }

        public void Dispose()
        {
            
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof (HealthCheckController))
            {
                return new HealthCheckController(checkers);
            }
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Enumerable.Empty<object>();
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }
    }

    public class AssembliesResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            return new[]
            {
                typeof(HealthCheckController).Assembly
            };
        }
    }
}