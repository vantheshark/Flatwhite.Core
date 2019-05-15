using Flatwhite.Core.Provider;

namespace Flatwhite.Core
{
	/// <summary>
	/// Global config
	/// </summary>
	public static class Global
    {   
        /// <summary>
        /// Internal cache for Flatwhite objects
        /// </summary>
        internal static MethodInfoCache Cache { get; set; }

        static Global()
        {
            Init();
        }

        internal static void Init()
        {
            Cache = new MethodInfoCache();
        }
    }
}