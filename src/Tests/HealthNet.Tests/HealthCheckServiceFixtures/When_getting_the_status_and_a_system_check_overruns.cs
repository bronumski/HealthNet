using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace HealthNet.HealthCheckServiceFixtures
{
    class When_getting_the_status_and_a_system_check_overruns
    {
        private HealthResult Result;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var hangingChecker = Substitute.For<ISystemChecker>();
            hangingChecker.SystemName.Returns("Hanging checker");
            hangingChecker.CheckSystem().Returns(x =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                return (SystemCheckResult)null;
            });

            var healthyChecker = Substitute.For<ISystemChecker>();
            healthyChecker.CheckSystem().Returns(x => new SystemCheckResult { SystemName = "Healthy checker", Health = HealthState.Good });
            
            var healthNetConfiguration = Substitute.For<IHealthNetConfiguration>();
            healthNetConfiguration.DefaultSystemCheckTimeout.Returns(TimeSpan.FromSeconds(1));

            var service = new HealthCheckService(healthNetConfiguration, Substitute.For<IVersionProvider>(), new[] { hangingChecker, healthyChecker });

            var task = Task<HealthResult>.Factory.StartNew(() => service.CheckHealth());
            if (Task.WaitAll(new Task[] {task}, TimeSpan.FromSeconds(5)))
            {
                Result = task.Result;
            }
            else
            {
                throw new TimeoutException();
            }
        }

        [Test]
        public void Overall_health_is_Serious()
        {
            Result.Health.Should().Be(HealthState.Serious);
        }

        [Test]
        public void Healthy_system_checker_result_is_returned()
        {
            Result.SystemStates.Single(x => x.SystemName == "Healthy checker").Health.Should().Be(HealthState.Good);
        }

        [Test]
        public void Hanging_system_checker_result_is_returned()
        {
            Result.SystemStates.Single(x => x.SystemName == "Hanging checker").Health.Should().Be(HealthState.Serious);
        }
    }
}