using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace HealthNet.HealthCheckServiceFixtures
{
    abstract class HealthCheckServiceFixtureBase
    {
        protected abstract IEnumerable<ISystemStateChecker> SystemStateCheckers();
        
        protected HealthResult Result { get; private set; }

        protected virtual bool PerformeIntrusive { get { return false; } }

        [TestFixtureSetUp]
        public void SetUp()
        {
            var versionProvider = Substitute.For<IVersionProvider>();
            versionProvider.GetSystemVersion().Returns("1.2.3.4");

            var service = new HealthCheckService(versionProvider, SystemStateCheckers());
            Result = service.CheckHealth(PerformeIntrusive);
        }

        [Test]
        public void Date_and_time_of_the_check_should_be_returned()
        {
            Result.CheckupDate.Should().BeCloseTo(DateTime.UtcNow, 5000);
        }

        [Test]
        public void Version_of_service()
        {
            Result.Version.Should().Be("1.2.3.4");
        }

        protected ISystemStateChecker CreateChecker(HealthState state, bool isVital = true, bool isIntrusive = false)
        {
            var systemStateChecker = Substitute.For<ISystemStateChecker>();
            systemStateChecker.CheckSystemState().Returns(new SystemStateResult
            {
                Health = state,
                IsVital = isVital
            });

            systemStateChecker.IsIntrusive.Returns(isIntrusive);
            return systemStateChecker;
        }
    }
}