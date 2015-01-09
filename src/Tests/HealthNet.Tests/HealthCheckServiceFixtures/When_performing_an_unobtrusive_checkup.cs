using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace HealthNet.HealthCheckServiceFixtures
{
    class When_performing_an_unobtrusive_checkup : HealthCheckServiceFixtureBase
    {
        private ISystemStateChecker intrusiveChecker;

        protected override IEnumerable<ISystemStateChecker> SystemStateCheckers()
        {
            intrusiveChecker = CreateChecker(HealthState.Critical, isIntrusive: true);
            intrusiveChecker.IsIntrusive.Returns(true);

            yield return CreateChecker(HealthState.Good);
            yield return intrusiveChecker;
        }

        [Test]
        public void Overall_health_is_Good()
        {
            Result.Health.Should().Be(HealthState.Good);
        }

        [Test]
        public void Has_a_system_with_a_Health_State_of_Undetermined()
        {
            Result.Systems.Should().ContainSingle(x => x.Health == HealthState.Undetermined);
        }

        [Test]
        public void Intrusive_checker_was_never_called()
        {
            intrusiveChecker.DidNotReceive().CheckSystemState();
        }
    }
}