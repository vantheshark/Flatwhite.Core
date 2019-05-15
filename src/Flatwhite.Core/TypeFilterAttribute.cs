using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Flatwhite.Core
{
	/// <summary>
	/// https://github.com/aspnet/Mvc/blob/65af12f1f575fdaee893e0232416df29bb83d7fa/src/Microsoft.AspNetCore.Mvc.Core/TypeFilterAttribute.cs
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    [DebuggerDisplay("TypeFilter: Type={ImplementationType} Order={Order}")]
    public class TypeFilterAttribute : Attribute, IFilterFactory
    {
	    /// <summary>
	    /// Gets or sets the order in which the action filters are executed.
	    /// </summary>
	    public int Order { get; set; }

        private ObjectFactory _factory;

        /// <summary>
        /// Instantiates a new <see cref="TypeFilterAttribute"/> instance.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of filter to create.</param>
        public TypeFilterAttribute(Type type)
        {
			ImplementationType = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <summary>
        /// Gets or sets the non-service arguments to pass to the <see cref="ImplementationType"/> constructor.
        /// </summary>
        /// <remarks>
        /// Service arguments are found in the dependency injection container i.e. this filter supports constructor
        /// injection in addition to passing the given <see cref="Arguments"/>.
        /// </remarks>
        public object[] Arguments { get; set; }

        /// <summary>
        /// Gets the <see cref="Type"/> of filter to create.
        /// </summary>
        public Type ImplementationType { get; }
        
        /// <summary>
		/// <inheritdoc cref="IFilterFactory"/>
		/// </summary>
		/// <param name="serviceProvider"></param>
		/// <returns></returns>
        public MethodFilterAttribute CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (_factory == null)
            {
                var argumentTypes = Arguments?.Select(a => a.GetType())?.ToArray();
                _factory = ActivatorUtilities.CreateFactory(ImplementationType, argumentTypes ?? Type.EmptyTypes);
            }

            var filter = (MethodFilterAttribute)_factory(serviceProvider, Arguments);
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
