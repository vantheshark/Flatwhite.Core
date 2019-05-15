using Flatwhite.Core.Tests.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Flatwhite.Core.Tests
{
	public class TestFilterOnProxyWithoutTarget
	{
		[Fact]
		public async Task Test_ex_thrown_from_filters()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.UseFlatwhiteFilters();
			serviceCollection.AddSingleton(Mock.Of<ILogger>());
			serviceCollection.AddProxyWithoutTarget<IProductService>(ServiceLifetime.Singleton);

			var sp = serviceCollection.BuildServiceProvider();
			
			var proxy = sp.GetRequiredService<IProductService>();
			

			var ex = Assert.Throws<Exception>(() => proxy.Delete(1));
			Assert.Equal($"{nameof(BadMethodFilterAttribute)}.{nameof(BadMethodFilterAttribute.OnMethodExecuting)}", ex.Message);
			ex = await Assert.ThrowsAsync<Exception>(() => proxy.DeleteAsync(1));
			Assert.Equal($"{nameof(BadMethodFilterAttribute)}.{nameof(BadMethodFilterAttribute.OnMethodExecutingAsync)}", ex.Message);
		}

		[Fact]
		public async Task Test_catching_ex_thrown_from_filters()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.UseFlatwhiteFilters();
			serviceCollection.AddProxyWithoutTarget<IProductService>(ServiceLifetime.Singleton);

			var sp = serviceCollection.BuildServiceProvider();
			var proxy = sp.GetRequiredService<IProductService>();

			// Exceptions are handled 
			proxy.DeleteBySku(Guid.NewGuid());
			await proxy.DeleteBySkuAsync(Guid.NewGuid());
		}
	}
}
