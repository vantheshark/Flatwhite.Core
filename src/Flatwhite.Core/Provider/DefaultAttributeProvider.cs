using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flatwhite.Core.Provider
{
	/// <summary>
	/// Default attribute provider
	/// </summary>
	public class DefaultAttributeProvider : IAttributeProvider
    {
	    private readonly IServiceProvider _serviceProvider;

		/// <summary>
		/// Initialize an instance of <see cref="DefaultAttributeProvider"/>
		/// </summary>
		/// <param name="serviceProvider"></param>
	    public DefaultAttributeProvider(IServiceProvider serviceProvider)
	    {
		    _serviceProvider = serviceProvider;
	    }

        /// <summary>
        /// Get all attributes decorated on method or declarative type
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="invocationContext"></param>
        /// <returns></returns>
        public IEnumerable<Attribute> GetAttributes(MethodInfo methodInfo, IDictionary<string, object> invocationContext)
        {
            if (methodInfo == null)
            {
                return new Attribute[0];
            }

            if (!Global.Cache.AttributeCache.ContainsKey(methodInfo))
            {
                var attributes = methodInfo.GetCustomAttributes(typeof (Attribute), true).OfType<Attribute>().ToList();

                if (methodInfo.DeclaringType != null)
                {
                    attributes.AddRange(methodInfo.DeclaringType.GetCustomAttributes(typeof (Attribute), true).OfType<Attribute>());
                }

	            for (var i = 0; i < attributes.Count; i++)
	            {
					if (attributes[i] is ServiceFilterAttribute serviceFilterAttribute)
					{
						attributes[i] = serviceFilterAttribute.CreateInstance(_serviceProvider);
					}

		            if (attributes[i] is TypeFilterAttribute typeFilterAttribute)
		            {
			            attributes[i] = typeFilterAttribute.CreateInstance(_serviceProvider);
		            }
	            }

                Global.Cache.AttributeCache[methodInfo] = attributes;
            }

            return Global.Cache.AttributeCache[methodInfo];
        }
    }
}