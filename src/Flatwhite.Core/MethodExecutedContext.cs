using System.Collections.Generic;

namespace Flatwhite.Core
{
	/// <summary>
	/// Contains information for the executed action.
	/// </summary>
	public class MethodExecutedContext : MethodInvocationContext
    {
        internal MethodExecutedContext(MethodExecutingContext executingContext)
        {
            Invocation = executingContext.Invocation;
            MethodInfo = executingContext.MethodInfo;
            InvocationContext = new Dictionary<string, object>(executingContext.InvocationContext);
            Result = executingContext.Result;
        }
    }
}