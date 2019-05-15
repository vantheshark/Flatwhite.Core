using System;

namespace Flatwhite.Core
{
	/// <summary>
	/// Copied from MVC Core
	/// </summary>
	public interface IFilterFactory
	{
		/// <summary>
		/// Creates an instance of the <see cref="MethodFilterAttribute"/> filter.
		/// </summary>
		/// <param name="serviceProvider">The request <see cref="IServiceProvider"/>.</param>
		/// <returns>An instance of the executable filter.</returns>
		MethodFilterAttribute CreateInstance(IServiceProvider serviceProvider);
	}
}