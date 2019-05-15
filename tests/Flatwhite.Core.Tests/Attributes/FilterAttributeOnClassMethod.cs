using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Flatwhite.Core.Tests.Attributes
{
	public class FilterAttributeOnClassMethod : MethodFilterAttribute
	{
		private readonly ILogger _logger;

		public FilterAttributeOnClassMethod(ILogger logger)
		{
			_logger = logger;
		}

		public override void OnMethodExecuted(MethodExecutedContext methodExecutedContext)
		{
			methodExecutedContext.InvocationContext[$"{nameof(FilterAttributeOnClassMethod)}.{nameof(OnMethodExecuted)}"] = DateTime.UtcNow;
		}

		public override Task OnMethodExecutedAsync(MethodExecutedContext methodExecutedContext)
		{
			methodExecutedContext.InvocationContext[$"{nameof(FilterAttributeOnClassMethod)}.{nameof(OnMethodExecutedAsync)}"] = DateTime.UtcNow;
			return Task.CompletedTask;
		}

		public override void OnMethodExecuting(MethodExecutingContext methodExecutingContext)
		{
			methodExecutingContext.InvocationContext[$"{nameof(FilterAttributeOnClassMethod)}.{nameof(OnMethodExecutedAsync)}"] = DateTime.UtcNow;
		}

		public override Task OnMethodExecutingAsync(MethodExecutingContext methodExecutingContext)
		{
			methodExecutingContext.InvocationContext[$"{nameof(FilterAttributeOnClassMethod)}.{nameof(OnMethodExecutingAsync)}"] = DateTime.UtcNow;
			return Task.CompletedTask;
		}
	}
}