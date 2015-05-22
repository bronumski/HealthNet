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

            httpConfiguration.DependencyResolver = new DependencyResolver(configuration, checkers);

            return app.UseWebApi(httpConfiguration);
        }
    }

    class DependencyResolver : IDependencyResolver
    {
        private readonly IHealthNetConfiguration configuration;
        private readonly IEnumerable<ISystemChecker> checkers;

        public DependencyResolver(IHealthNetConfiguration configuration, IEnumerable<ISystemChecker> checkers)
        {
            this.configuration = configuration;
            this.checkers = checkers;
        }

        public void Dispose()
        {
            
        }

        public object GetService(Type serviceType)
        {
            return serviceType == typeof(HealthCheckController) ? new HealthCheckController(configuration, checkers) : null;
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

    class AssembliesResolver : DefaultAssembliesResolver
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