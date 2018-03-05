using FluentAssertions;
using HealthNet.Integrations.Runners;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace HealthNet.Integrations
{
  class When_getting_the_health_status_using_a_custom_version_provider<TFixtureRunner>
    : HealthCheckResponseFixturesBase <TFixtureRunner> where TFixtureRunner : IFixtureRunner, new()
  {
    protected override void ConfigureDependencies(IServiceCollection services)
    {
      services.AddTransient(x =>
      {
        var versionProvider = Substitute.For<IVersionProvider>();
        versionProvider.GetSystemVersion().Returns("9.8");
        return versionProvider;
      });
    }

    [Test]
    public void Should_return_version_number_from_custom_version_number_provider()
    {
      RawContent.Should().Contain("\"systemVersion\":\"9.8\"");
    }
  }
}