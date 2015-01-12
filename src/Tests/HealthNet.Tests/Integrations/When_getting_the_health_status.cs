using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using HealthNet.Integrations.Runners;
using NSubstitute;
using NUnit.Framework;

namespace HealthNet.Integrations
{
    class When_getting_the_health_status<TFixtureRunner> : IntegrationFixtures<TFixtureRunner> where TFixtureRunner : IFixtureRunner, new()
    {
        private ISystemChecker intrusiveSystemChecker;

        protected override IEnumerable<ISystemChecker> CreateCheckers()
        {
            var systemChecker = Substitute.For<ISystemChecker>();
            systemChecker.CheckSystem().Returns(new SystemCheckResult());
            intrusiveSystemChecker = Substitute.For<ISystemChecker>();
            intrusiveSystemChecker.IsIntrusive.Returns(true);

            yield return systemChecker;
            yield return intrusiveSystemChecker;
        }

        [Test]
        public void Should_return_status_Ok()
        {
            Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void Should_return_camlecase_json()
        {
            RawContent.Should().Contain("\"checkupDate\":");
        }

        [Test]
        public void Should_return_non_formatted_json()
        {
            RawContent.Should().NotContain("\n").And.NotContain("\r").And.NotContain("\t");
        }

        [Test]
        public void Should_not_return_json_with_null_values()
        {
            RawContent.Should().NotContain("null");
        }

        [Test]
        public void Should_return_enum_string_values()
        {
            RawContent.Should().Contain("\"health\":\"Good\"");
        }

        [Test]
        public void Should_not_call_the_intrusive_checker()
        {
            intrusiveSystemChecker.DidNotReceive().CheckSystem();
        }

        [Test]
        public void Should_have_host_name()
        {
            RawContent.Should().Contain(string.Format("\"host\":\"{0}\"", System.Environment.MachineName));
        }
    }
}