# HealthNet (Documentation still in progress)
.Net health check endpoint


HealthNet provides a simple way of exposing a health check endpoint to any .net application

## Configuration

A custom configuration object is required to get the hosting path and also determin the application version number.
The path is exposed by the the Path property on the `IHealthNetConfiguration` interface. The version is done by looking at the file version of the Assembly where the configuration object is defined. Implementing can either by done by implementing the `IHealthNetConfiguration` interface directly or by inheriting from `HealthNetConfiguration`. The later will default the path to `\api\healthcheck`.

```csharp
public class CustomHealthCheckConfiguration : IHealthNetConfiguration
{
  public sttring Path { get { return "\foo\bar"; } }
}

public class CustmHealthCheckConfiguration : HealthNetConfiguration {}
```


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

  //Tells the health check service that this checker is intrusive and should be skipped
  //unless doing a thorough exam
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
  app.Use<HealthNetMiddleware>(
    new CustomHealthNetConfiguration(),
    () => container.ResolveAll(typeof(ISystemChecker)));
  ...
}
```

### WebApi 2

* Install the latest [`HealthNet.WebApi`](https://www.nuget.org/packages/HealthNet.WebApi/) package.
* Create a custom `HealthNetConfiguration` class
* Add the custom `HealthNetConfiguration` class and any implementation of `ISystemChecker` to your IoC of choice

Example using autofac

```csharp
protected void Application_Start()
{
  ...
  var builder = new ContainerBuilder();
  ...
  builder.RegisterApiControllers(typeof(HealthCheckController).Assembly);
  builder.RegisterType<MyCustomHealthCheckConfiguration>().As<IHealthNetConfiguration>();
  builder.RegisterAssemblyTypes(GetType().Assembly)
    .Where(t => t.GetInterfaces().Any(x => x == typeof(ISystemChecker)))
    .As<ISystemChecker>();
  ...
  var container = builder.Build();
  ...
}
```

### NancyFx

* Install the latest [`HealthNet.Nancy`](https://www.nuget.org/packages/HealthNet.Nancy/) package.
* Create a custom `HealthNetConfiguration` class
* Add the custom `HealthNetConfiguration` class and any implementation of `ISystemChecker` to your IoC of choice

Example using Nancy's built in Tiny IoC:
 
```csharp
//If not using the IoC auto wire up
protected override void ConfigureApplicationContainer(TinyIoCContainer container)
{
  ...
  container.Register<IHealthNetConfiguration, MyCustomHealthCheckConfiguration>().AsSingleton();
  container.RegisterMultiple<ISystemChecker>(
    new[] {typeof (MyCustomSystemChecker),
    typeof (MyOtherCustinSystemChecker)});
  ...
}
```

### Manually

If you so desire you can invoke the `HealthCheckServiceDirectly`. This will give you more control over the path and how the version is derived.

```csharp
var healthCheckService = new HealthCheckService(
  new CustomVersionProvider(),
  new [] { new CustomSystemChecker() });
```

[![Build status](https://ci.appveyor.com/api/projects/status/05xrcyeej88itj1b?svg=true)](https://ci.appveyor.com/project/bronumski/healthnet)
[![NuGet Status](http://img.shields.io/nuget/v/HealthNet.Core.svg?style=flat)](https://www.nuget.org/packages/HealthNet.Core/) 
