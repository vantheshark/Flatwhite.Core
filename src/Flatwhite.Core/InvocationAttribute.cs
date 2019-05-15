using System;
using System.Threading.Tasks;

namespace Flatwhite.Core
{
    internal class InvocationAttribute : MethodFilterAttribute
    {
        private readonly IInvocation _invocation;
        private readonly Type _taskGenericReturnType;

        public InvocationAttribute(IInvocation invocation, Type taskGenericReturnType = null)
        {
            _invocation = invocation;
            _taskGenericReturnType = taskGenericReturnType;
        }

        public override void OnMethodExecuting(MethodExecutingContext methodExecutingContext)
        {
            _invocation.Proceed();
			methodExecutingContext.Result = _invocation.ReturnValue;
        }

        public override async Task OnMethodExecutingAsync(MethodExecutingContext actionContext)
        {
            _invocation.Proceed();

	        if (_invocation.ReturnValue is Task taskResult)
            {
                if (_taskGenericReturnType != null)
                {
                    actionContext.Result = await taskResult.TryGetTaskResult();
                }
                else
                {
                    await taskResult;
                }
            }
            else
				actionContext.Result = _invocation.ReturnValue;
        }
    }
}