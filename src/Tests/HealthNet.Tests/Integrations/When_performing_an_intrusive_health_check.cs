using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using HealthNet.Integrations.Runners;
using NSubstitute;
using NUnit.Framework;

namespace HealthNet.Integrations
{
    class When_performing_an_intrusive_health_check<TFixtureRunner> : HealthCheckResponseFixturesBase<TFixtureRunner> where TFixtureRunner : IFixtureRunner, new()
    {
        private ISystemChecker intrusiveSystemChecker;

        protected override IEnumerable<ISystemChecker> CreateCheckers()
        {
            intrusiveSystemChecker = Substitute.For<ISystemChecker>();
            intrusiveSystemChecker.CheckSystem().Returns(new SystemCheckResult());
            intrusiveSystemChecker.IsIntrusive.Returns(true);

            yield return intrusiveSystemChecker;
        }

        protected override bool IsIntrusive { get { return true; } }

        [Test]
        public void Should_return_status_Ok()
        {
            Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void Should_call_the_intrusive_checker()
        {
            intrusiveSystemChecker.Received().CheckSystem();
        }
    }
}