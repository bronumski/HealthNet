using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Owin.Testing;
using NCeption;
using NSubstitute;
using NUnit.Framework;

namespace HealthNet.HealthNetMiddlewareFixures
{
    class When_getting_the_health_status_using_a_custom_url
    {
        private HttpResponseMessage response;
        private TestServer server;

        private IEnumerable<ISystemChecker> CreateCheckers()
        {
            var systemChecker = Substitute.For<ISystemChecker>();
            systemChecker.CheckSystem().Returns(new SystemCheckResult());

            yield return systemChecker;
        }

        [TestFixtureSetUp]
        public void SetUp()
        {
            server = TestServer.Create(app => app.UseHealthNet("/foo/bar", CreateCheckers));

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