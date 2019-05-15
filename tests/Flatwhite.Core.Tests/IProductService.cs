using Flatwhite.Core.Tests.Attributes;
using System;
using System.Threading.Tasks;

namespace Flatwhite.Core.Tests
{
	/// <summary>
	/// This is the interface without any implementation
	/// </summary>
	public interface IProductService
	{
		[BadMethodFilter]
		void Delete(int id);

		[BadMethodFilter]
		Task DeleteAsync(int id);

		[BadMethodFilter]
		[HandleAllMethodExceptions]
		void DeleteBySku(Guid sku);

		[BadMethodFilter]
		[HandleAllMethodExceptions]
		Task DeleteBySkuAsync(Guid sku);
	}
}
