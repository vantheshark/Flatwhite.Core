using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Flatwhite.Core 
{
    internal class MethodInfoCache
    {
        /// <summary>
        /// </summary>
        public MethodInfoCache()
        {
            var primitiveTypes = new List<Type>
            {
                typeof (short),
                typeof (ushort),
                typeof (int),
                typeof (uint),
                typeof (long),
                typeof (ulong),

                typeof (bool),
                typeof (char),
                typeof (byte),
                typeof (string),

                typeof (decimal),
                typeof (float),
                typeof (double),

                typeof (DateTime),
                typeof (Guid),

            };
            var nullable = typeof(Nullable<>);
        }

        public readonly IDictionary<MethodInfo, List<Attribute>> AttributeCache = new ConcurrentDictionary<MethodInfo, List<Attribute>>();
        public readonly IDictionary<MethodInfo, bool> InterceptableCache = new ConcurrentDictionary<MethodInfo, bool>();
    }
}
