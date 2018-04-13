using System;

namespace HealthNet
{
  public interface IHealthNetConfiguration
  {
    string Path { get; }

    TimeSpan DefaultSystemCheckTimeout { get; }
  }
}