using System;
using System.Collections.Generic;

namespace Flatwhite.Core
{
	/// <summary>
	/// Represents an exception and the contextual data associated with it when exception was caught.
	/// </summary>
	public class MethodExceptionContext : MethodInvocationContext
    {
        /// <summary>
        /// Initializes the exception context with current <see cref="MethodInvocationContext" />
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        public MethodExceptionContext(Exception exception, MethodInvocationContext context)
        {
            Invocation = context.Invocation;
            MethodInfo = context.MethodInfo;
	        InvocationContext = new Dictionary<string, object>(context.InvocationContext);
			Result = context.Result;
	        Exception = exception;
        }

        /// <summary>
        /// The result of the method
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Indicate whether the exception was handled
        /// </summary>
        public bool Handled { get; set; }
    }
}