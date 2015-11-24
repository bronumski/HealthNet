using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace HealthNet.HealthCheckServiceFixtures
{
    abstract class HealthCheckServiceFixtureBase
    {
        protected abstract IEnumerable<ISystemChecker> SystemStateCheckers();
        
        protected HealthResult Result { get; private set; }

        protected virtual bool PerformeIntrusive { get { return false; } }

        [TestFixtureSetUp]
        public void SetUp()
        {
            var versionProvider = Substitute.For<IVersionProvider>();
            versionProvider.GetSystemVersion().Returns("1.2.3.4");

            var service = new HealthCheckService(null, versionProvider, SystemStateCheckers());
            Result = service.CheckHealth(PerformeIntrusive);
        }

        [Test]
        public void Date_and_time_of_the_check_should_be_returned()
        {
            Result.CheckupDate.Should().BeCloseTo(DateTime.UtcNow, 5000);
        }

        [Test]
        public void Host_name_should_be_returned()
        {
            Result.Host.Should().Be(Environment.MachineName);
        }

        [Test]
        public void Version_of_service()
        {
            Result.SystemVersion.Should().Be("1.2.3.4");
        }

        [Test]
        public void Should_return_total_time_taken_to_perform_health_check()
        {
            Result.TimeTaken
                .Should().BeGreaterThan(TimeSpan.FromSeconds(0))
                .And.BeLessThan(TimeSpan.FromMilliseconds(500));
        }

        [Test]
        public void Should_return_time_taken_to_check_each_system()
        {
            foreach (var systemCheckResult in Result.SystemStates)
            {
                systemCheckResult.TimeTaken.Should().BeGreaterThan(TimeSpan.FromSeconds(0))
                .And.BeLessThan(TimeSpan.FromMilliseconds(500));
            }
        }

        protected ISystemChecker CreateChecker(HealthState state, bool isVital = true, bool isIntrusive = false, string name = "")
        {
            var systemStateChecker = Substitute.For<ISystemChecker>();
            systemStateChecker.CheckSystem().Returns(new SystemCheckResult
            {
                SystemName = name,
                Health = state,
                IsVital = isVital
            });

            systemStateChecker.SystemName.Returns(name);

            systemStateChecker.IsIntrusive.Returns(isIntrusive);
            return systemStateChecker;
        }
    }
}