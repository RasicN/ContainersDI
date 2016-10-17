There are 3 Total Nuget packages in this Repo.
# Containers - Core application
This is a tool to simplify dependency injection and IoC's.  
Containers acomplishes:
- Dependency Injection for WCF and WebAPI applications down to one line
- A consistent syntax for an inversion of control tool
- An IoC wrapper that removes a lot of functionality to focus on the most basic needs

## Configs
Available config settings:
- `new Config<InterfaceType, ImplementationType>()` Basic binding
- `new Config<InterfaceType>(new ImplementationType())` Basic binding of instantiated object
- `new Config<InterfaceType, ImplementationType>().AsSingleton()` Basic singleton binding
- `new Config<InterfaceType>(new ImplementationType())` Basic instantiated singleton binding
- `new SelfConfig(typeof(ImplementationType))` Required to assign value to constructor arguments (useful if you have two implemenations of the same InterfaceType

### Config extension methods
- `config.AsSingleton()` Makes a singleton instance
- `config.WithName("UniqueName")` Assigns a name to the binding, this defaults to the InterfaceType

### SelfConfig extension methods
Supports all of the above config extension methods as well as:
- `selfConfig.ResolveConstructorArgumentUsing<ImplementationType>("{parameterName}")`  Assigns an ImplemenationType to a parameter

*Right now Containers uses Ninject version 3.2.2, however it is wrapped up into the package .dll so it is not considered a dependency from Nuget's perspective.  The idea is that the IoC tool doesn't really matter and it can be easily swapped out if needed.

# WebAPI
Create a `ContainerConfig.cs` or `Install-Package Containers.WebAPI`.

Populate the Configs with the desired bindings
```csharp
public static ICollection<IConfig> Configs => new List<IConfig>
{
    // Fill in configs here
    // new Config<IType, Type>();
    // new Config<IType>(new Type());
};
```

In the `Global.asax``Application_Start()` add the following line at the end:
```csharp
protected void Application_Start()
{  
  // ...
  // ...
  new WebApiContainer(ContainerConfig.Configs).SetDependencyResolver();
}
```

# WCF Service
Create a `ContainerConfig.cs` that inherits `ServiceHostFactory` or `Install-Package Containers.WCF`.

Populate the Configs with the desired bindings
```csharp
public class ContainerConfig : ServiceHostFactory
{
  public static ICollection<IConfig> Configs => new List<IConfig>
  {
      // Fill in configs here
      // new Config<IType, Type>();
      // new Config<IType>(new Type());
  };

  protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddress)
  {
      return new WcfContainer(Configs).CreateServiceHost(serviceType, baseAddress);
  }
}
```

Edit your `.svc` **not `.svc.cs`** (you can do this by right clicking on it and selecting "Open With").

Add a Factory that points to `ContainerConfig`
```csharp
<% ... Factory="ProjectName.ContainerConfig" %>
```
