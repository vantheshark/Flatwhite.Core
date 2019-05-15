using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Flatwhite.Core
{
	/// <summary>
	/// Represents the base class for all method-filter attributes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ExceptionFilterAttribute : Attribute
    {
        /// <summary>
        /// Occurs when there is an exception
        /// </summary>
        /// <param name="exceptionContext"></param>
        [DebuggerStepThrough]
        public virtual void OnException(MethodExceptionContext exceptionContext)
        {
        }

        /// <summary>
        /// Occurs when there is an exception
        /// </summary>
        /// <param name="exceptionContext"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public virtual Task OnExceptionAsync(MethodExceptionContext exceptionContext)
        {
			OnException(exceptionContext);
            return Task.CompletedTask;
        }
    }
}