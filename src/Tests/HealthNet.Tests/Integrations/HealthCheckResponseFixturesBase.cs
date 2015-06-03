using FluentAssertions;
using HealthNet.Integrations.Runners;
using NUnit.Framework;

namespace HealthNet.Integrations
{
    abstract class HealthCheckResponseFixturesBase<TFixtureRunner> : IntegrationFixtures<TFixtureRunner> where TFixtureRunner : IFixtureRunner, new()
    {
        [Test]
        public void Should_have_json_content()
        {
            Response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Test]
        public void Should_return_health_check_result()
        {
            RawContent.Should().Contain("\"health\":");
        }
    }
}