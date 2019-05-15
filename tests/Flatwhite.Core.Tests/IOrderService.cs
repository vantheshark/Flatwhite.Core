using Flatwhite.Core.Tests.Attributes;
using System;
using System.Threading.Tasks;

namespace Flatwhite.Core.Tests
{
	public class Order
	{
		public long Id {get;set;}
		public decimal TotalAmount {get;set;}
	}

	[HandleAllMethodExceptions]
	interface IOrderService
	{
		[FilterAttributeOnInterfaceMethod]
		Order GetById(int id);
		
		void Delete(int id);

		[FilterAttributeOnInterfaceMethod]
		Task<Order> GetByIdAsync(int id);
		
		Task DeleteAsync(int id);
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

		public void Delete(int id)
		{
			if (id <= 0) throw new ArgumentException("invalid order id");
			if (id > 1000) throw new InvalidOperationException("id not found");
		}

		public async Task DeleteAsync(int id)
		{
			await Task.Delay(10);
			Delete(id);
		}
	}
}
