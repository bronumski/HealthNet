using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace HealthNet.HealthCheckServiceFixtures
{
  class When_performing_an_intrusive_checkup_on_a_critical_system : HealthCheckServiceFixtureBase
  {
    private ISystemChecker intrusiveChecker;

    protected override bool PerformeIntrusive => true;

    protected override IEnumerable<ISystemChecker> SystemStateCheckers()
    {
      intrusiveChecker = CreateChecker(HealthState.Critical, isIntrusive: true, name: "Intrusive Checker");

      yield return CreateChecker(HealthState.Good);
      yield return intrusiveChecker;
    }

    [Test]
    public void Overall_health_is_Critical()
    {
      Result.Health.Should().Be(HealthState.Critical);
    }

    [Test]
    public void Has_a_system_with_a_Health_State_of_Undetermined()
    {
      Result.SystemStates.Should().ContainSingle(x =>
        x.SystemName == "Intrusive Checker"
        && x.Health == HealthState.Critical);
    }

    [Test]
    public void Intrusive_checker_was_never_called()
    {
      intrusiveChecker.Received().CheckSystem();
    }
  }
}