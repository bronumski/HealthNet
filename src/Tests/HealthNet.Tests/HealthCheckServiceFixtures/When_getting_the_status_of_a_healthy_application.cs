using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace HealthNet.HealthCheckServiceFixtures
{
    class When_getting_the_status_of_a_healthy_application : HealthCheckServiceFixtureBase
    {
        protected override IEnumerable<ISystemChecker> SystemStateCheckers()
        {
            yield return CreateChecker(HealthState.Good);
        }

        [Test]
        public void Overall_health_is_Good()
        {
            Result.Health.Should().Be(HealthState.Good);
        }

        [Test]
        public void All_system_states_are_Good()
        {
            Result.SystemStates.Should().OnlyContain(x => x.Health == HealthState.Good);
        }
    }
}