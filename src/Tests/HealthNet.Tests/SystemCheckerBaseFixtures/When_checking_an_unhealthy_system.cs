using System;
using FluentAssertions;
using NUnit.Framework;

namespace HealthNet.SystemCheckerBaseFixtures
{
    class When_checking_an_unhealthy_system
    {
        private SystemCheckResult result;
        private TestSystemSystemChecker testSystemSystemChecker;

        [SetUp]
        public void SetUp()
        {
            testSystemSystemChecker = new TestSystemSystemChecker();
            result = testSystemSystemChecker.CheckSystem();
        }

        [Test]
        public void System_name_is_derived_from_the_checker_class_name()
        {
            result.SystemName.Should().Be("Test System");
        }

        [Test]
        public void Error_results_in_a_critical_health_state()
        {
            result.Health.Should().Be(HealthState.Critical);
        }

        [Test]
        public void Error_message_returned_in_result_message()
        {
            result.Message.Should().Be("Fail");
        }

        [Test]
        public void Defaults_to_vital_system()
        {
            result.IsVital.Should().BeTrue();
        }

        [Test]
        public void Defaults_to_non_intrusive_checker()
        {
            testSystemSystemChecker.IsIntrusive.Should().BeFalse();
        }

        class TestSystemSystemChecker : SystemCheckerBase
        {
            protected override void PerformCheck()
            {
                throw new Exception("Fail");
            }
        }
    }
}