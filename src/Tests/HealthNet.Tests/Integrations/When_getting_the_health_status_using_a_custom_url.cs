using HealthNet.Integrations.Runners;

namespace HealthNet.Integrations
{
  class When_getting_the_health_status_using_a_custom_url<TFixtureRunner>
    : HealthCheckResponseFixturesBase<TFixtureRunner> where TFixtureRunner : IFixtureRunner, new()
  {
    protected override string Path => "/foo/bar";

    protected override IHealthNetConfiguration GetConfiguration()
    {
      return new TestHealthNetConfiguration(Path);
    }
  }
}