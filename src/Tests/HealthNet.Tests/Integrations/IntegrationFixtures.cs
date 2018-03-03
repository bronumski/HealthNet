using System;
using System.Collections.Generic;
using System.Net.Http;
using HealthNet.Integrations.Runners;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using NSubstitute;
using NUnit.Framework;

namespace HealthNet.Integrations
{
  [TestFixture(typeof(NancyFixturesRunner))]
  [TestFixture(typeof(OwinFixturesRunner))]
  [TestFixture(typeof(WebApiFixturesRunner))]
  [TestFixture(typeof(AspNetCoreFixturesRunner))]
  abstract class IntegrationFixtures<TFixtureRunner> where TFixtureRunner : IFixtureRunner, new()
  {
    [OneTimeSetUp]
    public void SetUp()
    {
      IFixtureRunner runner = new TFixtureRunner();
      using (var server = new TestServer(new WebHostBuilder()
        .Configure(app => runner.Configure(app, GetConfiguration(), CreateCheckers()).Run(async context =>
        {
          context.Response.ContentType = "text/plain";
          await context.Response.WriteAsync("Hello World");
        }))))
      {
        Response = server.CreateClient().GetAsync(Path).Result;

        RawContent = Response.Content.ReadAsStringAsync().Result;
      }

      Console.WriteLine(RawContent);
    }

    protected virtual IHealthNetConfiguration GetConfiguration()
    {
      return new TestHealthNetConfiguration();
    }

    protected virtual string Path => $"/api/healthcheck{(IsIntrusive ? "?intrusive=true" : string.Empty)}";

    protected virtual bool IsIntrusive => false;

    protected HttpResponseMessage Response { get; private set; }

    protected string RawContent { get; private set; }

    protected virtual IEnumerable<ISystemChecker> CreateCheckers()
    {
      var systemChecker = Substitute.For<ISystemChecker>();
      systemChecker.CheckSystem().Returns(new SystemCheckResult());
      yield return systemChecker;
    }
  }
}