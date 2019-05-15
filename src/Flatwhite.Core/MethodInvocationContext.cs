using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Flatwhite.Core
{
	/// <summary>
	/// Contains basic information about a method action
	/// </summary>
	public abstract class MethodInvocationContext
	{
		/// <summary>
		/// The MethodInfo of the executing method
		/// </summary>
		public MethodInfo MethodInfo { get; internal set; }
		
		/// <summary>
		/// The invocation context of the executing method
		/// </summary>
		public IDictionary<string, object> InvocationContext { get; internal set; }

		/// <summary>
		/// The result of the method being filtered. Set value to stop the chain
		/// </summary>
		public object Result {get;set;}

		/// <summary>
		/// The Invocation data
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IInvocation Invocation { get; internal set; }
	}
}