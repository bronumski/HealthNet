using System.Net;
using FluentAssertions;
using HealthNet.Integrations.Runners;
using NUnit.Framework;

namespace HealthNet.Integrations
{
    class When_calling_application_with_a_non_healthcheck_call<TFixtureRunner> : IntegrationFixtures<TFixtureRunner> where TFixtureRunner : IFixtureRunner, new()
    {
        protected override string Path
        {
            get { return "/foo"; }
        }

        [Test]
        public void Should_return_status_of_OK()
        {
            Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void Should_return_content_from_non_healthcheck_call()
        {
            Response.Content.ReadAsStringAsync().Result.Should().Be("Hello World");
        }
    }
}