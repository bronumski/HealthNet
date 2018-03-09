# HealthNet
**.Net health check endpoint**

[![Build status](https://ci.appveyor.com/api/projects/status/05xrcyeej88itj1b?svg=true)](https://ci.appveyor.com/project/bronumski/healthnet)
[![NuGet Status](http://img.shields.io/nuget/v/HealthNet.Core.svg?style=flat)](https://www.nuget.org/packages/HealthNet.Core/) 

HealthNet provides a simple way of exposing a health check endpoint to any .net application.
It provides a mechanism to query your application and view the overall state and sub states of your application.
HealthNet is modelled similarly to how we would view a health check of the human body.

The medical system uses a set of [Medical States](http://en.wikipedia.org/wiki/Medical_state) to describe the state of a  patient, as such our application can either be in a Good, Serious, Critical or Undetermined state.
Also the human body is made up of a number of [Systems](http://en.wikipedia.org/wiki/List_of_systems_of_the_human_body), we can apply that same principal to our applications.
Applications are dependent on a number of components either internally or remote, we can apply the same set of states mentioned previously to each of these systems.
These systems can either be vital, such as the brain or a database without which the body or application would not work.
Alternativly vision is not so important but it makes life easier, the same could be said of an email system.
So our overall state of the application can determined by the state of its systems and how vital they are.
Additionally some checks to see how a system is performing can be intrusive and others not so, think colonoscopy.
In terms of our applications an intrusive check may be time consuming or degrade performance.

## Creating System Checkers

There are two ways to create a system checker. Either by implementing the `ISystemChecker` interface or by inheriting from the `SystemCheckerBase`.
The interface gives you full flexibility whereas the base class simplifies the implementation.

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

The system checker base class takes care of the basics. It defaults to an unobtrusive vital system checker, this can be overriden.
The check is performed in the `PerformCheck` method, if the system is in bad health an exception should be thrown otherwise there is no need to do anything.
The base class also derives the name of the system from the name of the class, if the class name has "SystemChecker" as suffix then it will be removed.

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

## Configuration

A custom configuration object is required to get the hosting path and also determine the application version number.
The path is exposed by the the Path property on the `IHealthNetConfiguration` interface. The version is done by looking at the file version of the Assembly where the configuration object is defined. Implementing can either by done by implementing the `IHealthNetConfiguration` interface directly or by inheriting from `HealthNetConfiguration`. The later will default the path to `\api\healthcheck`.

```csharp
public class CustomHealthCheckConfiguration : IHealthNetConfiguration
{
  public sttring Path { get { return "\foo\bar"; } }
}

public class CustmHealthCheckConfiguration : HealthNetConfiguration {}
```

## Wiring it up

### AspNet Core

* Install the latest [`HealthNet.Owin`](https://www.nuget.org/packages/HealthNet.AspNetCore/) package.
`dotnet add package HealthNet.AspNetCore`
`Install-Package HealthNet.AspNetCore`
* Create a custom `HealthNetConfiguration` class

```csharp
public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    ...
    services.AddHealthNet(new AuthHealthCheckConfiguration());
    services.AddTransient<ISystemChecker, FooSystemChecker>();
    ...
  }

  public void Configure(IApplicationBuilder app)
  {
    ...
    app.UseHealthNetMiddleware(); //Expect rename to app.UseHealthNet();
    ...
  }
}
```

### Owin

* Install the latest [`HealthNet.Owin`](https://www.nuget.org/packages/HealthNet.Owin/) package.
`dotnet add package HealthNet.Owin`
`Install-Package HealthNet.Owin`
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
`dotnet add package HealthNet.WebApi`
`Install-Package HealthNet.WebApi`
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
`dotnet add package HealthNet.Nancy`
`Install-Package HealthNet.Nancy`
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

## Calling the health check

There are two modes to calling the health check, the basic call will call the checker and run all the non-intrusive System Checks or for a more thorough investigation an intrusive check can be done.

```
$ curl http://host/api/healthcheck
HTTP/1.1 200 OK
Content-Type: application/json;charset=utf-8
```
```json
{
    "host": "host1",
    "checkupDate": "2015-06-03T15:08:35.8104214Z",
    "health": "Good",
    "systemStates": [
        {
            "health": "Good",
            "isVital": true,
            "systemName": "Non Intrusive Health Check",
            "timeTaken": "00:00:00.5014515"
        },
        {
            "health": "Undetermined",
            "isVital": true,
            "message": "Intrusive check skipped",
            "systemName": "Intrusive Health Check",
            "timeTaken": "00:00:00.000"
        }
    ],
    "systemVersion": "1.2.3.4",
    "timeTaken": "00:00:00.5088545"
}
```

```
$ curl http://host/api/healthcheck?intrusive=true

HTTP/1.1 200 OK
Content-Type: application/json;charset=utf-8
```
```json
{
    "host": "host2",
    "checkupDate": "2015-06-03T15:08:35.8104214Z",
    "health": "Good",
    "systemStates": [
        {
            "health": "Good",
            "isVital": true,
            "systemName": "Non Intrusive Health Check",
            "timeTaken": "00:00:00.5014515"
        },
        {
            "health": "Good",
            "isVital": true,
            "systemName": "Intrusive Health Check",
            "timeTaken": "00:00:00.000"
        }
    ],
    "systemVersion": "1.2.3.4",
    "timeTaken": "00:00:00.5088545"
}
```

## Dashboard

In order to make full use of the health checks it pays to have a Dashboard to show the current state of your systems. You will probably want to create your own one but in the short term
you can use the healthnet dashboard javascript and stylesheet bundled with the repo.

![ACME DashBoard](https://github.com/bronumski/HealthNet/blob/master/AcmeDashboard.PNG)

```html
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>ACME Dashboard</title>
</head>
<body>
<link rel="stylesheet" type="text/css" href="https://cdn.rawgit.com/bronumski/HealthNet/release/1.1.0.46/src/DashBoard/HealthNetDashboard.css">

<script type="application/javascript" src="https://cdn.rawgit.com/bronumski/HealthNet/release/1.1.0.46/src/DashBoard/HealthNetDashboard.js"></script>

<div id="healthCheckDashboard"></div>

<script type="application/javascript">
    var dashboard = new HealthNetDashboard("healthCheckDashboard",
            [
                HealthNetDashboard.createEnvironment("Production", [
                    { endpointName: "Service A", endpointUrl: "http://servera.production.com/healthcheck" },
                    { endpointName: "Service B", endpointUrl: "http://serverb.production.com/healthcheck" },
                    { endpointName: "Service C", endpointUrl: "http://serverc.production.com/healthcheck" }
                ]),
                HealthNetDashboard.createEnvironment("Staging", [
                    { endpointName: "Service A", endpointUrl: "http://servera.staging.com/healthcheck" },
                    { endpointName: "Service B", endpointUrl: "http://serverb.staging.com/healthcheck" },
                    { endpointName: "Service C", endpointUrl: "http://serverc.staging.com/healthcheck" }
                ]),
                HealthNetDashboard.createEnvironment("Development", [
                    { endpointName: "Service A", endpointUrl: "http://servera.development.com/healthcheck" },
                    { endpointName: "Service B", endpointUrl: "http://serverb.development.com/healthcheck" },
                    { endpointName: "Service C", endpointUrl: "http://serverc.development.com/healthcheck" }
                ])
            ]);
    dashboard.checkHealth();
</script>
</body>
</html>
```

[![Build status](https://ci.appveyor.com/api/projects/status/05xrcyeej88itj1b?svg=true)](https://ci.appveyor.com/project/bronumski/healthnet)
[![NuGet Status](http://img.shields.io/nuget/v/HealthNet.Core.svg?style=flat)](https://www.nuget.org/packages/HealthNet.Core/) 