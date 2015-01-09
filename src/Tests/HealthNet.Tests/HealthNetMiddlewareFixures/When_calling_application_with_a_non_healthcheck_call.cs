using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Owin.Testing;
using NCeption;
using NUnit.Framework;
using Owin;

namespace HealthNet.HealthNetMiddlewareFixures
{
    class When_calling_application_with_a_non_healthcheck_call
    {
        private HttpResponseMessage response;
        private TestServer server;

        [TestFixtureSetUp]
        public void SetUp()
        {
            server = TestServer.Create(app => app
                .UseHealthNet(() => new ISystemChecker[0])
                .Run(context =>
                {
                    context.Response.ContentType = "text/plain";
                    return context.Response.WriteAsync("Hello World");
                }));

            var client = server.HttpClient;

            response = client.GetAsync("/foo/bar").Result;
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Safely.Dispose(server);
        }

        [Test]
        public void Should_return_status_of_OK()
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}