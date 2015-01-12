using System.Collections.Generic;
using System.Linq;
using Owin;

namespace HealthNet.Integrations.Runners
{
    class OwinFixturesRunner : IFixtureRunner
    {
        public IAppBuilder Configure(IAppBuilder app, IHealthNetConfiguration configuration, IEnumerable<ISystemChecker> checkers)
        {
            return app.UseHealthNet(configuration, () =>
            {
                var systemCheckers = checkers as ISystemChecker[] ?? checkers.ToArray();
                return systemCheckers;
            });
        }
    }
}