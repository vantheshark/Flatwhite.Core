using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Flatwhite.Core
{
    /// <summary>
    /// Represents the base class for all method-filter attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class MethodFilterAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the order in which the action filters are executed.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="methodExecutingContext"></param>
        [DebuggerStepThrough]
        public virtual void OnMethodExecuting(MethodExecutingContext methodExecutingContext)
        {
        }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="methodExecutingContext"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public virtual Task OnMethodExecutingAsync(MethodExecutingContext methodExecutingContext)
        {
            OnMethodExecuting(methodExecutingContext);
	        return Task.CompletedTask;
        }


        /// <summary>
        ///  Occurs after the action method is invoked.
        /// </summary>
        /// <param name="methodExecutedContext"></param>
        [DebuggerStepThrough]
        public virtual void OnMethodExecuted(MethodExecutedContext methodExecutedContext)
        {
        }

        /// <summary>
        ///  Occurs after the action method is invoked.
        /// </summary>
        /// <param name="methodExecutedContext"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public virtual Task OnMethodExecutedAsync(MethodExecutedContext methodExecutedContext)
        {
            OnMethodExecuted(methodExecutedContext);
			return Task.CompletedTask;
        }
    }
}