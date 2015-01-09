using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace HealthNet.HealthCheckServiceFixtures
{
    class When_getting_the_status_of_an_application_with_a_critical_non_vital_system : HealthCheckServiceFixtureBase
    {
        protected override IEnumerable<ISystemChecker> SystemStateCheckers()
        {
            yield return CreateChecker(HealthState.Critical, isVital: false);
        }

        [Test]
        public void Overall_health_is_Serious()
        {
            Result.Health.Should().Be(HealthState.Serious);
        }
    }
}