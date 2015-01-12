using System.Collections.Generic;
using Owin;

namespace HealthNet.Integrations.Runners
{
    internal interface IFixtureRunner
    {
        IAppBuilder Configure(IAppBuilder app, IHealthNetConfiguration configuration, IEnumerable<ISystemChecker> checkers);
    }
}