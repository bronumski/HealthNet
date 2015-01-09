using System;
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
    class When_getting_the_health_status_of_an_application_as_json
    {
        private HttpResponseMessage response;
        private string rawResult;
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
            server = TestServer.Create(app => app.UseHealthNet(CreateCheckers));

            var client = server.HttpClient;

            response = client.GetAsync("/api/healthcheck").Result;

            rawResult = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine(rawResult);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Safely.Dispose(server);
        }

        [Test]
        public void Should_return_status_Ok()
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void Should_return_camlecase_json()
        {
            rawResult.Should().Contain("\"checkupDate\":");
        }

        [Test]
        public void Should_return_non_formatted_json()
        {
            rawResult.Should().NotContain("\n").And.NotContain("\r").And.NotContain("\t");
        }

        [Test]
        public void Should_not_return_json_with_null_values()
        {
            rawResult.Should().NotContain("null");
        }

        [Test]
        public void Should_return_enum_string_values()
        {
            rawResult.Should().Contain("\"health\":\"Good\"");
        }
    }
}