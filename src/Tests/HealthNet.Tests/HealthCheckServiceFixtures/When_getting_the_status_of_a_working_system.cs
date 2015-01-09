using FluentAssertions;
using NUnit.Framework;

namespace HealthNet.HealthCheckServiceFixtures
{
    class When_getting_the_status_of_a_working_system
    {
        private HealthResult result;

        [SetUp]
        public void SetUp()
        {
            var service = new HealthCheckService();
            result = service.CheckHealth();
        }

        [Test]
        public void Should_return_an_overall_health_of_Good()
        {
            result.State.Should().Be(HealthState.Good);
        }
    }
}