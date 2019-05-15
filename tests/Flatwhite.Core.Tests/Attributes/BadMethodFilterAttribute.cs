using System;
using System.Threading.Tasks;

namespace Flatwhite.Core.Tests.Attributes
{
	public class BadMethodFilterAttribute : MethodFilterAttribute
	{
		public override void OnMethodExecuted(MethodExecutedContext methodExecutedContext)
		{
			throw new Exception($"{nameof(BadMethodFilterAttribute)}.{nameof(OnMethodExecuted)}");
		}

		public override Task OnMethodExecutedAsync(MethodExecutedContext actionExecutedContext)
		{
			throw new Exception($"{nameof(BadMethodFilterAttribute)}.{nameof(OnMethodExecutedAsync)}");
		}

		public override void OnMethodExecuting(MethodExecutingContext methodExecutingContext)
		{
			throw new Exception($"{nameof(BadMethodFilterAttribute)}.{nameof(OnMethodExecuting)}");
		}

		public override Task OnMethodExecutingAsync(MethodExecutingContext actionContext)
		{
			throw new Exception($"{nameof(BadMethodFilterAttribute)}.{nameof(OnMethodExecutingAsync)}");
		}
	}
}