using System;

namespace Flatwhite.Core
{
	/// <summary>
	/// Decorate on a method to ignore all method filter
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class NoInterceptAttribute : Attribute
    {
    }
}