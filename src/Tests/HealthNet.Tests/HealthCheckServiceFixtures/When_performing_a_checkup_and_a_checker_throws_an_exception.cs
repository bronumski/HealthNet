using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace HealthNet.HealthCheckServiceFixtures
{
  class When_performing_a_checkup_and_a_checker_throws_an_exception : HealthCheckServiceFixtureBase
  {
    protected override IEnumerable<ISystemChecker> SystemStateCheckers()
    {
      var failingChecker = Substitute.For<ISystemChecker>();
      failingChecker.IsVital.Returns(true);
      failingChecker.SystemName.Returns("Failing Checker");
      failingChecker.CheckSystem().Returns(x => throw new Exception("Some exception"));

      yield return CreateChecker(HealthState.Good);
      yield return failingChecker;
      yield return CreateChecker(HealthState.Good);
    }

    [Test]
    public void Overall_health_is_Critical()
    {
      Result.Health.Should().Be(HealthState.Critical);
    }

    [Test]
    public void All_checkers_should_still_be_called()
    {
      Result.SystemStates.Should().HaveCount(3);
    }

    [Test]
    public void Failing_system_result_is_retuned()
    {
      Result.SystemStates.Should().ContainSingle(x =>
        x.SystemName == "Failing Checker"
        && x.Health == HealthState.Critical
        && x.Message == "Some exception");
    }
  }
}