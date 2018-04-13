using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace HealthNet.HealthCheckServiceFixtures
{
  class When_performing_an_unobtrusive_checkup : HealthCheckServiceFixtureBase
  {
    private ISystemChecker intrusiveChecker;

    protected override IEnumerable<ISystemChecker> SystemStateCheckers()
    {
      intrusiveChecker = CreateChecker(HealthState.Critical, isIntrusive: true, name: "Intrusive Checker");

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
      Result.SystemStates.Should().ContainSingle(x =>
        x.SystemName == "Intrusive Checker"
        && x.Health == HealthState.Undetermined);
    }

    [Test]
    public void Has_a_message_indicating_that_the_check_was_skipped_due_to_being_intrusive()
    {
      Result.SystemStates.Should().ContainSingle(x =>
        x.SystemName == "Intrusive Checker"
        && x.Message == "Intrusive check skipped");
    }

    [Test]
    public void Intrusive_checker_was_never_called()
    {
      intrusiveChecker.DidNotReceive().CheckSystem();
    }
  }
}