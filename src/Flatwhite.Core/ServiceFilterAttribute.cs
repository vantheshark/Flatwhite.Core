using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Flatwhite.Core
{
	/// <summary>
	/// https://github.com/aspnet/Mvc/blob/65af12f1f575fdaee893e0232416df29bb83d7fa/src/Microsoft.AspNetCore.Mvc.Core/ServiceFilterAttribute.cs
    /// A filter that finds another filter in an <see cref="IServiceProvider"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Similar to the <see cref="TypeFilterAttribute"/> in that both use constructor injection. Use
    /// <see cref="TypeFilterAttribute"/> instead if the filter is not itself a service.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    [DebuggerDisplay("ServiceFilter: Type={ServiceType} Order={Order}")]
    public class ServiceFilterAttribute : Attribute, IFilterFactory
    {
        /// <summary>
        /// Instantiates a new <see cref="ServiceFilterAttribute"/> instance.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of filter to find.</param>
        public ServiceFilterAttribute(Type type)
        {
			ServiceType = type ?? throw new ArgumentNullException(nameof(type));
        }

	    /// <summary>
	    /// Gets or sets the order in which the action filters are executed.
	    /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets the <see cref="Type"/> of filter to find.
        /// </summary>
        public Type ServiceType { get; }

        /// <inheritdoc />
        public MethodFilterAttribute CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var filter = (MethodFilterAttribute)serviceProvider.GetRequiredService(ServiceType);
            if (filter is IFilterFactory filterFactory)
            {
                // Unwrap filter factories
                filter = filterFactory.CreateInstance(serviceProvider);
            }

			filter.Order = Order;
            return filter;
        }
    }
}
