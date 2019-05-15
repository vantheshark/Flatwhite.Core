using System;
using System.Threading.Tasks;

namespace Flatwhite.Core.Tests.Attributes
{
	public class FilterAttributeOnInterfaceMethod : MethodFilterAttribute
	{
		public override void OnMethodExecuted(MethodExecutedContext methodExecutedContext)
		{
			methodExecutedContext.InvocationContext[$"{nameof(FilterAttributeOnInterfaceMethod)}.{nameof(OnMethodExecuted)}"] = DateTime.UtcNow;
		}

		public override Task OnMethodExecutedAsync(MethodExecutedContext methodExecutedContext)
		{
			methodExecutedContext.InvocationContext[$"{nameof(FilterAttributeOnInterfaceMethod)}.{nameof(OnMethodExecutedAsync)}"] = DateTime.UtcNow;
			return Task.CompletedTask;
		}

		public override void OnMethodExecuting(MethodExecutingContext methodExecutingContext)
		{
			methodExecutingContext.InvocationContext[$"{nameof(FilterAttributeOnInterfaceMethod)}.{nameof(OnMethodExecutedAsync)}"] = DateTime.UtcNow;
		}

		public override Task OnMethodExecutingAsync(MethodExecutingContext methodExecutingContext)
		{
			methodExecutingContext.InvocationContext[$"{nameof(FilterAttributeOnInterfaceMethod)}.{nameof(OnMethodExecutingAsync)}"] = DateTime.UtcNow;
			return Task.CompletedTask;
		}
	}
}