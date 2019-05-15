using System.Collections.Generic;

namespace Flatwhite.Core.Provider
{
    /// <summary>
    /// A provider to resolve a context which is a key/value dictionary. This could be a HttpRequestContext or Thread context
    /// </summary>
    public interface IContextProvider
    {
        /// <summary>
        /// Get context
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetContext();
    }

	internal class EmptyContextProvider : IContextProvider
	{
		public IDictionary<string, object> GetContext()
		{
			return new Dictionary<string, object>();
		}
	}
}
