using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace HealthNet.HealthCheckServiceFixtures
{
    class When_getting_the_sysetem_checker_returns_a_null : HealthCheckServiceFixtureBase
    {
        protected override IEnumerable<ISystemChecker> SystemStateCheckers()
        {
            var systemStateChecker = Substitute.For<ISystemChecker>();
            systemStateChecker.CheckSystem().Returns((SystemCheckResult) null);
            yield return systemStateChecker;
            yield return CreateChecker(HealthState.Good);
        }

        [Test]
        public void Overall_health_is_Goof()
        {
            Result.Health.Should().Be(HealthState.Good);
        }
    }
}