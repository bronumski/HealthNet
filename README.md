# HealthNet (Documentation still in progress)
.Net health check endpoint


HealthNet provides a simple way of exposing a health check endpoint to any .net application

## Creating System Checkers

There are two ways to create a system checker. Either by implementing the `ISystemChecker` interface or by inheriting from the `SystemCheckerBase`.
The interface gives you full flexability where as the base class simplifies the impementation.

### ISystemChecker

```csharp
public class TestSystemChecker : ISystemChecker
{
  public SystemCheckResult CheckSystem()
  {
    if (true == true)
    {
      return this.CreateCriticalResult("Some error");
    }
    return this.CreateGoodResult();
  }

  //Tells the health check service that this checker is intrusive and should be skipped unless
  //doing a thorough exam
  public bool IsIntrusive { get { return false; } }
  
  //The Name to be returned in the health check result
  public string SystemName { get { return "Test Checker"; } }
  
  //Provides information in the health check as to whether this is a vital system or not
  public bool IsVital { get { return true; } }
}
```

### SystemCheckerBase

The system checker base class takes care of the basics. It defaults to an unobtrosive vital system checker, this can be overriden.
The check is performed in the `PerformCheck method, if the system is in bad health an exception should be thrown otherwise there is no need to do anything.

```csharp
public class TestSystemChecker : SystemCheckerBase
{
  protected override void PerformCheck()
  {
    if (true == true)
    {
      throw new Exception("Some error");
    }
  }
}
```

## Wiring it up

### Owin

* Install the latest [`HealthNet.Owin`](https://www.nuget.org/packages/HealthNet.Owin/) package.
* Create a custom `HealthNetConfiguration` class
* Provide a function to the middleware to resolve an enumeration of `ISystemChecker`

```csharp
public void Configuration(IAppBuilder app)
{
  ...
  app.Use<HealthNetMiddleware>(new CustomHealthNetConfiguration(), () => container.ResolveAll(typeof(ISystemChecker)));
  ...
}
```

### WebApi 2

### NancyFx

[![Build status](https://ci.appveyor.com/api/projects/status/05xrcyeej88itj1b?svg=true)](https://ci.appveyor.com/project/bronumski/healthnet)
[![NuGet Status](http://img.shields.io/nuget/v/HealthNet.Core.svg?style=flat)](https://www.nuget.org/packages/HealthNet.Core/) 
