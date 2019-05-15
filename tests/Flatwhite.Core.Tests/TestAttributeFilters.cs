using System;
using System.Threading.Tasks;
using Flatwhite.Core.Tests.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Flatwhite.Core.Tests
{
	public class TestAttributeFilters
	{
		[Fact]
		public async Task Test_interceptors()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton(Mock.Of<ILogger>());
			serviceCollection.UseFlatwhiteFilters();
			serviceCollection.RegisterWithMethodFilters<IOrderService, OrderService>(ServiceLifetime.Singleton);
			serviceCollection.AddSingleton<FilterAttributeOnClassMethod>();

			var sp = serviceCollection.BuildServiceProvider();


			var instance = sp.GetRequiredService<OrderService>();
			var proxy = sp.GetRequiredService<IOrderService>();
			

			// All exceptions are handled if calling proxy
			Assert.Null(proxy.GetById(0));
			Assert.Null(await proxy.GetByIdAsync(0));
			Assert.Equal(1, proxy.GetById(1).Id);
			Assert.Equal(1, (await proxy.GetByIdAsync(1)).Id);

			proxy.Delete(0);
			proxy.Delete(1);
			proxy.Delete(2000);
			await proxy.DeleteAsync(0);
			await proxy.DeleteAsync(1);
			await proxy.DeleteAsync(2000);

			// Exception is not handled by default if calling instance
			Assert.Throws<ArgumentException>(() => instance.GetById(0));
			await Assert.ThrowsAsync<ArgumentException>(() => instance.GetByIdAsync(0));
			Assert.Equal(1, instance.GetById(1).Id);
			Assert.Equal(1, (await instance.GetByIdAsync(1)).Id);

			Assert.Throws<ArgumentException>(() => instance.Delete(0));
			instance.Delete(1);
			Assert.Throws<InvalidOperationException>(() => instance.Delete(2000));
			await Assert.ThrowsAsync<ArgumentException>(() => instance.DeleteAsync(0));
			await instance.DeleteAsync(1);
			await Assert.ThrowsAsync<InvalidOperationException>(() => instance.DeleteAsync(2000));
			
		}
	}
}
