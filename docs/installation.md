## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNet/releases)
- [Known Issues](known-issues.md)

## Installation

### Preconditions
You need your own Nitro project as a precondition of this installation manual.
Please follow the beautiful guide of Nitro: [Link](https://github.com/namics/generator-nitro/)

### Step 1 - Create a ASP.NET MVC application
Create a ASP.NET MVC solution on your local machine with Visual Studio and compile the solution.

### Step 2 - Install NitroNet
There are several ways to install NitroNet. The easiest way is to use NitroNet together with Unity or CastleWindsor.

Please choose between variant
* **A** with Unity or CastleWindsor
* **B** with another IoC Framework.

#### (A) With Unity or CastleWindsor

##### NuGet Package installation

Execute following the line in your NuGet Package Manager to install NitroNet for Sitecore with your preferred IoC framework:

**Unity**

`PM >` `Install-Package NitroNet.UnityModules`

Optionally, we recommend to install the [Unity.Mvc](https://www.nuget.org/packages/Unity.Mvc/) which is a lightweight Unity bootstrapper for MVC applications:

`PM >` `Install-Package Unity.Mvc`

**CastleWindsor**:

`PM >` `Install-Package NitroNet.CastleWindsorModules`


##### Extend your Global.asax(.cs)
To activate NitroNet it's important to add/register the new view engine in your application. You can do this, with these lines of code ([Gist](https://gist.github.com/hombreDelPez/40320c444a6ac4ba39d0040eaf25fdcb)):

```csharp
protected void Application_Start()
{
	AreaRegistration.RegisterAllAreas();
	FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
	RouteConfig.RegisterRoutes(RouteTable.Routes);
	BundleConfig.RegisterBundles(BundleTable.Bundles);

	ViewEngines.Engines.Add(DependencyResolver.Current.GetService<NitroNetViewEngine>());
}
```

##### Register the IoC containers
###### Unity
To activate NitroNet with Unity, please add these lines to */App_Start/UnityConfig.cs* in your application ([Gist](https://gist.github.com/hombreDelPez/81ad0980c560f8c0e5acca9bef1280b1)):

```csharp
public static void RegisterTypes(IUnityContainer container)
{
	var rootPath = HostingEnvironment.MapPath("~/");
	var basePath = PathInfo.Combine(PathInfo.Create(rootPath), PathInfo.Create(ConfigurationManager.AppSettings["NitroNet.BasePath"])).ToString();

	new DefaultUnityModule(basePath).Configure(container);
}
```

###### CastleWindsor
To activate NitroNet with CastleWindsor, please add these lines to your application:

```csharp
public static void RegisterTypes(IWindsorContainer container)
{
	var rootPath = HostingEnvironment.MapPath("~/");
	var basePath = PathInfo.Combine(PathInfo.Create(rootPath), PathInfo.Create(ConfigurationManager.AppSettings["NitroNet.BasePath"])).ToString();

	new DefaultCastleWindsorModule(basePath).Configure(container);
}
```

#### (B) With another IoC Framework
You don't like Unity and you design your application with an other IoC framework? No Problem.
In this case, you can install NitroNet only with our base package:

`PM >` `Install-Package NitroNet`

##### Extend your Global.asax(.cs)
*Please extend your Global.asax(.cs) in the same way as in scenario A*

##### Register NitroNet with your own IoC framework
Actually, we only made a Unity and CastleWindsor integration with NitroNet. But it's easy to use another IoC Framework.
Please follow our Unity sample as a template for you ([Gist](https://gist.github.com/daniiiol/036be44e535768fac2df5eec0aff9180)):

###### DefaultUnityModule.cs

```csharp
using Microsoft.Practices.Unity;
using NitroNet;
using NitroNet.Mvc;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.Cache;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.IO;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.ViewEngines;
using System.Web;
using Veil.Compiler;
using Veil.Helper;

namespace NitroNet.UnityModules
{
  public class DefaultUnityModule : IUnityModule
  {
    private readonly string _basePath;

    public DefaultUnityModule(string basePath)
    {
      this._basePath = basePath;
    }

    public void Configure(IUnityContainer container)
    {
      this.RegisterConfiguration(container);
      this.RegisterApplication(container);
    }

    protected virtual void RegisterConfiguration(IUnityContainer container)
    {
      INitroNetConfig nitroNetConfig = ConfigurationLoader.LoadNitroConfiguration(this._basePath);
      UnityContainerExtensions.RegisterInstance<INitroNetConfig>(container, nitroNetConfig);
      UnityContainerExtensions.RegisterInstance<IFileSystem>(container, (IFileSystem) new FileSystem(this._basePath, nitroNetConfig));
    }

    protected virtual void RegisterApplication(IUnityContainer container)
    {
      UnityContainerExtensions.RegisterInstance<AsyncLocal<HttpContext>>(container, new AsyncLocal<HttpContext>(), (LifetimeManager) new ContainerControlledLifetimeManager());
      UnityContainerExtensions.RegisterType<IHelperHandlerFactory, DefaultRenderingHelperHandlerFactory>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
      UnityContainerExtensions.RegisterType<IMemberLocator, MemberLocatorFromNamingRule>(container);
      UnityContainerExtensions.RegisterType<INamingRule, NamingRule>(container);
      UnityContainerExtensions.RegisterType<IModelTypeProvider, DefaultModelTypeProvider>(container);
      UnityContainerExtensions.RegisterType<IViewEngine, VeilViewEngine>(container);
      UnityContainerExtensions.RegisterType<ICacheProvider, MemoryCacheProvider>(container);
      UnityContainerExtensions.RegisterType<IComponentRepository, DefaultComponentRepository>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
      UnityContainerExtensions.RegisterType<ITemplateRepository, NitroTemplateRepository>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
      UnityContainerExtensions.RegisterType<INitroTemplateHandlerFactory, MvcNitroTemplateHandlerFactory>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
    }
  }
}
```