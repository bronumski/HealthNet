using System;
using System.Collections.Generic;
using System.Net.Http;
using HealthNet.Integrations.Runners;
using Microsoft.Owin.Testing;
using NSubstitute;
using NUnit.Framework;
using Owin;

namespace HealthNet.Integrations
{
    [TestFixture(typeof(NancyFixturesRunner))]
    [TestFixture(typeof(OwinFixturesRunner))]
    [TestFixture(typeof(WebApiFixturesRunner))]
    abstract class IntegrationFixtures<TFixtureRunner> where TFixtureRunner : IFixtureRunner, new()
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            IFixtureRunner runner = new TFixtureRunner();
            using (var server = TestServer.Create(app => runner.Configure(app, GetConfiguration(), CreateCheckers()).Run(context =>
                {
                    context.Response.ContentType = "text/plain";
                    return context.Response.WriteAsync("Hello World");
                })))
            {
                Response = server.HttpClient.GetAsync(Path).Result;

                RawContent = Response.Content.ReadAsStringAsync().Result;
            }

            Console.WriteLine(RawContent);
        }

        protected virtual IHealthNetConfiguration GetConfiguration()
        {
            return new TestHealthNetConfiguration();
        }

        protected virtual string Path { get { return "/api/healthcheck" + (IsIntrusive ? "?intrusive=true" : string.Empty); } }

        protected virtual bool IsIntrusive { get { return false; } }

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