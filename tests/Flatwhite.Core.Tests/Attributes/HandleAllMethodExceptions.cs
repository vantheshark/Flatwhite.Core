using System;
using System.Threading.Tasks;

namespace Flatwhite.Core.Tests.Attributes
{
	public class HandleAllMethodExceptions : ExceptionFilterAttribute
	{
		public override void OnException(MethodExceptionContext exceptionContext)
		{
			exceptionContext.Handled = true;
			exceptionContext.InvocationContext[$"{nameof(HandleAllMethodExceptions)}.{nameof(OnException)}"] = DateTime.UtcNow;
		}

		public override Task OnExceptionAsync(MethodExceptionContext exceptionContext)
		{
			exceptionContext.Handled = true;
			exceptionContext.InvocationContext[$"{nameof(HandleAllMethodExceptions)}.{nameof(OnExceptionAsync)}"] = DateTime.UtcNow;
			return Task.CompletedTask;
		}
	}
}