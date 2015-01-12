using System.Collections.Generic;
using HealthNet.Nancy;
using Nancy.Testing;
using Owin;

namespace HealthNet.Integrations.Runners
{
    class NancyFixturesRunner : IFixtureRunner
    {
        public IAppBuilder Configure(IAppBuilder app, IHealthNetConfiguration healthNetConfiguration, IEnumerable<ISystemChecker> checkers)
        {
            return app
                .UseNancy(x =>
                    x.Bootstrapper =
                        new ConfigurableBootstrapper(
                            y => y.Module<HealthNetModule>().DisableAutoRegistrations().Dependency(checkers).Dependency(healthNetConfiguration)));
        }
    }
}