<img alt="Flatwhite logo" src="http://oi64.tinypic.com/15hkikp.jpg" title="Flatwhite" width="100px" height="100px"/>

# Flatwhite.Core

[![Latest version](https://img.shields.io/nuget/v/Flatwhite.Core.svg)](https://www.nuget.org/packages/Flatwhite.Core/) [![NuGet Downloads](https://img.shields.io/nuget/dt/Flatwhite.Core.svg)](https://www.nuget.org/packages/Flatwhite.Core/) [![Build Status](https://api.travis-ci.org/vanthoainguyen/Flatwhite.Core.svg)](https://travis-ci.org/vanthoainguyen/Flatwhite.Core) [![Build status](https://ci.appveyor.com/api/projects/status/yw4jbtxymftja5dx?svg=true)](https://ci.appveyor.com/project/vanthoainguyen/flatwhite-core) [![Test status](https://img.shields.io/appveyor/tests/vanthoainguyen/flatwhite-core.svg)](https://ci.appveyor.com/project/vanthoainguyen/flatwhite-core/branch/master) [![License WTFPL](https://img.shields.io/badge/licence-WTFPL-green.svg)](http://sam.zoy.org/wtfpl/COPYING)


## What is Flatwhite.Core?

Port from old Flatwhite package (.NET 4.5.2) to support only .NET CORE 2.1.x and doesn't require any extra packages

## When to use Flatwhite.Core?

In your .NET core app, you have a need to intercept method calls so you possibly have 2 quick options:
- Use Autofac.Extras and call EnableInterfaceInterceptor() on type registrations then create/register custom IInterceptor.
- Or use Flatwhite, implement an MethodFilterAttribute and decorate on the methods on your interfaces which you want to intercept.

Flatwhite works for any methods

## How to use Flatwhite.Core?

#### First, register the components
```C#
[HandleAllMethodExceptions]
interface IOrderService
{
	[FilterAttributeOnInterfaceMethod]
	Order GetById(int id);	

	[FilterAttributeOnInterfaceMethod]
	Task<Order> GetByIdAsync(int id);	
}

var serviceCollection = new ServiceCollection();
serviceCollection.UseFlatwhiteFilters();
serviceCollection.RegisterWithMethodFilters<IOrderService, OrderService>(ServiceLifetime.Singleton);
```

#### For additional logic before/after calling methods
Flatwhite is inspired by ASP.NET MVC ActionFilterAttribute, so it works quite similar. The base filter attribute has following methods. So simply implement your filter class and do whatever you want.

```C#
public abstract class MethodFilterAttribute : Attribute
{
    public virtual void OnMethodExecuting(MethodExecutingContext methodExecutingContext);    
    public virtual Task OnMethodExecutingAsync(MethodExecutingContext methodExecutingContext);   
    public virtual void OnMethodExecuted(MethodExecutedContext methodExecutedContext);    
    public virtual Task OnMethodExecutedAsync(MethodExecutedContext methodExecutedContext);    
}
```

If you decorate the filter on async methods, only _OnMethodExecutingAsync_ and _OnMethodExecutedAsync_ are called. During the filters are being executed, if the Result value is set to the MethodExecutingContext, the remaining filters will be ignored. However, all filters will be called _OnMethodExecutedAsync_ or _OnMethodExecuted_ unless there is an exception during invocation


#### For error handling
Similar to MethodFilterAttribute, you can implement *ExceptionFilterAttribute* to provide custom error handling logic. If the property _MethodExceptionContext.Handled_ is true, all remaining *ExceptionFilter* will be ignored.

```C#
public abstract class ExceptionFilterAttribute : Attribute
{    
    public virtual void OnException(MethodExceptionContext exceptionContext);    
    public virtual Task OnExceptionAsync(MethodExceptionContext exceptionContext);       
}
```

#### Dependency injection on your filter
You can either use *ServiceFilterAttribute* or *TypeFilterAttribute* which work similar to .NET MVC Core

```C#

public class FilterAttributeOnClassMethod : MethodFilterAttribute
{
	private readonly ILogger _logger;

	public FilterAttributeOnClassMethod(ILogger logger)
	{
		_logger = logger;
	}
}

public class OrderService : IOrderService
{
	[ServiceFilter(typeof(FilterAttributeOnClassMethod))]
	public Order GetById(int id)
	{
		if (id <= 0) throw new ArgumentException("invalid order id");
		
		return new Order {Id = id, TotalAmount = id * DateTime.Now.Ticks};
	}
	
	[TypeFilter(typeof(FilterAttributeOnClassMethod))]
	public async Task<Order> GetByIdAsync(int id)
	{
		await Task.Delay(10);
		return GetById(id);
	}
}


var serviceCollection = new ServiceCollection();
serviceCollection.UseFlatwhiteFilters();
serviceCollection.RegisterWithMethodFilters<IOrderService, OrderService>(ServiceLifetime.Singleton);
// You must register the filter so ServiceProvider can resolve it later
serviceCollection.AddSingleton<FilterAttributeOnClassMethod>();
```


#### Create proxy without target

You can create an interface, have some method filters on its method and dont even need to implement the interface

```C#
public class FireProductDeletedEvent : MethodFilterAttribute
{	
}

public class HandleAllException : ExceptionFilterAttribute
{	
}


/// <summary>
/// This is the interface without any implementation, you can implement the main code in FireProductDeletedEvent and exception handling in HandleAllException
/// </summary>
[HandleAllException]
public interface IProductNotificationService
{	
	[FireProductDeletedEvent]
	Task FireProductEvent(Guid sku);
}

var serviceCollection = new ServiceCollection();
serviceCollection.UseFlatwhiteFilters();
serviceCollection.AddProxyWithoutTarget<IProductNotificationService>(ServiceLifetime.Singleton);
```

## LICENCE
[![License WTFPL](https://img.shields.io/badge/licence-WTFPL-green.svg)](http://sam.zoy.org/wtfpl/COPYING) ![Troll](http://i40.tinypic.com/2m4vl2x.jpg) 

