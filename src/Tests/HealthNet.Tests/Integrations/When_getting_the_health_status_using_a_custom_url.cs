using System.Net;
using FluentAssertions;
using HealthNet.Integrations.Runners;
using NUnit.Framework;

namespace HealthNet.Integrations
{
    class When_getting_the_health_status_using_a_custom_url<TFixtureRunner> : HealthCheckResponseFixturesBase<TFixtureRunner> where TFixtureRunner : IFixtureRunner, new()
    {
        protected override string Path
        {
            get { return "/foo/bar"; }
        }

        protected override IHealthNetConfiguration GetConfiguration()
        {
            return new TestHealthNetConfiguration(Path);
        }

        [Test]
        public void Should_return_status_of_OK()
        {
            Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}