using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Flatwhite.Core.Provider;

namespace Flatwhite.Core
{
    /// <summary>
    /// The default method interceptors that resolve MVC/WebAPI style action filters to execute before and after the invocation
    /// </summary>
    public class MethodInterceptor<T> : DispatchProxy where T : class
    {
        private static readonly MethodInfo HandleAsyncWithTypeMethod = typeof (MethodInterceptor<T>).GetMethod(nameof(HandleAsyncWithType), BindingFlags.Instance | BindingFlags.NonPublic);
	    private T _decorated;
        private IAttributeProvider _attributeProvider;
        private IContextProvider _contextProvider;

		/// <summary>
		/// Create a proxy with provided target
		/// </summary>
		/// <param name="decorated"></param>
		/// <param name="contextProvider"></param>
		/// <param name="attributeProvider"></param>
		/// <returns></returns>
	    public static T Create(T decorated, IContextProvider contextProvider, IAttributeProvider attributeProvider)
	    {
		    object proxy = Create<T, MethodInterceptor<T>>();
		    ((MethodInterceptor<T>) proxy).SetParameters(decorated, contextProvider, attributeProvider);

		    return (T) proxy;
	    }

	    private void SetParameters(T decorated, IContextProvider contextProvider, IAttributeProvider attributeProvider)
	    {
		    _decorated = decorated;
		    _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
		    _attributeProvider = attributeProvider ?? throw new ArgumentNullException(nameof(attributeProvider));
	    }

        /// <summary>
        /// Intercept the invocation
        /// </summary>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
	        var methodParameterTypes = targetMethod.GetParameters().Select(p => p.ParameterType).ToArray();
	        var classMethodInfo = _decorated != null
		        ? _decorated.GetType().GetMethod(targetMethod.Name, methodParameterTypes)
				: targetMethod;

			var invocation = new Invocation
			{
				Arguments = args,
				GenericArguments = targetMethod.IsGenericMethod ? targetMethod.GetGenericArguments() : new Type[0],
				InvocationTarget = _decorated,
				Method = targetMethod,
				Proxy = this,
				MethodInvocationTarget = classMethodInfo,
				TargetType = _decorated != null ? _decorated.GetType() : typeof(T)
			};

            var methodExecutingContext = new MethodExecutingContext
            {
                InvocationContext = _contextProvider.GetContext(),
                MethodInfo = targetMethod,
                Invocation = invocation
            };

            var attributes = GetInvocationMethodFilterAttributes(invocation, methodExecutingContext.InvocationContext);
            if (attributes.Any(a => a is NoInterceptAttribute))
            {
				invocation.Proceed();
				return invocation.ReturnValue;
            }

            var methodFilterAttributes = attributes.OfType<MethodFilterAttribute>().OrderBy(x => x.Order).ToList();
	        var exceptionFilterAttributes = attributes.OfType<ExceptionFilterAttribute>().ToList();

            var isAsync = typeof (Task).IsAssignableFrom(invocation.Method.ReturnType);

            if (isAsync)
            {
                if (invocation.Method.ReturnType.IsGenericType && invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof (Task<>))
                {
                    var taskResultType = invocation.Method.ReturnType.GetGenericArguments()[0];
                    var mInfo = HandleAsyncWithTypeMethod.MakeGenericMethod(taskResultType);
					if (_decorated != null)
					{
		                methodFilterAttributes.Add(new InvocationAttribute(invocation, taskResultType));
					}
                    invocation.ReturnValue = mInfo.Invoke(this, new object[] { methodFilterAttributes, exceptionFilterAttributes, methodExecutingContext });
                }
                else
                {
	                if (_decorated != null)
	                {
						methodFilterAttributes.Add(new InvocationAttribute(invocation));
					}
                    invocation.ReturnValue = HandleAsync(methodFilterAttributes, exceptionFilterAttributes, methodExecutingContext);
                }
            }
            else
            {
	            if (_decorated != null)
	            {
					methodFilterAttributes.Add(new InvocationAttribute(invocation));
				}
                HandleSync(methodFilterAttributes, exceptionFilterAttributes, methodExecutingContext);
            }
			return invocation.ReturnValue;
        }

        private void HandleSync(IReadOnlyList<MethodFilterAttribute> filterAttributes, IReadOnlyList<ExceptionFilterAttribute> exceptionFilterAttributes, MethodExecutingContext methodExecutingContext)
        {
            foreach (var f in filterAttributes)
            {
                try
                {
                    if (methodExecutingContext.Result == null) f.OnMethodExecuting(methodExecutingContext);
                }
                catch (Exception ex)
                {
	                var exContext = new MethodExceptionContext(ex, methodExecutingContext);
					HandleException(exceptionFilterAttributes, exContext);
	                if (!exContext.Handled)
	                {
						throw;
	                }
                }
            }

            var reversedFilterAttributes = filterAttributes.Reverse();
            var methodExecutedContext = new MethodExecutedContext(methodExecutingContext);

            foreach (var f in reversedFilterAttributes)
            {
                try
                {
                    f.OnMethodExecuted(methodExecutedContext);
                }
                catch (Exception ex)
                {
	                var exContext = new MethodExceptionContext(ex, methodExecutedContext);
	                HandleException(exceptionFilterAttributes, exContext);
	                if (!exContext.Handled)
	                {
		                throw;
	                }
                }
            }
        }
        
        private void HandleException(IReadOnlyList<ExceptionFilterAttribute> exceptionFilterAttributes, MethodExceptionContext exceptionContext)
        {
            foreach (var f in exceptionFilterAttributes)
            {
                try
                {
                    if (!exceptionContext.Handled)
                    {
                        f.OnException(exceptionContext);
                    }
                }
                catch (Exception ex)
                {
                    throw new AggregateException(ex.Message, ex, exceptionContext.Exception);
                }
            }
        }

	    /// <summary>
	    /// This will be called via Reflection
	    /// </summary>
	    /// <typeparam name="TResult"></typeparam>
	    /// <param name="filterAttributes"></param>
	    /// <param name="exceptionFilterAttributes"></param>
	    /// <param name="methodExecutingContext"></param>
	    /// <returns></returns>
	    private async Task<TResult> HandleAsyncWithType<TResult>(IReadOnlyList<MethodFilterAttribute> filterAttributes, IReadOnlyList<ExceptionFilterAttribute> exceptionFilterAttributes, MethodExecutingContext methodExecutingContext)
        {
	        foreach (var f in filterAttributes)
	        {
		        try
		        {
			        if (methodExecutingContext.Result == null) await f.OnMethodExecutingAsync(methodExecutingContext).ConfigureAwait(false);
		        }
		        catch (Exception ex)
		        {
			        var exContext = new MethodExceptionContext(ex, methodExecutingContext);
			        await HandleExceptionAsync(exceptionFilterAttributes, exContext);
			        if (!exContext.Handled)
			        {
				        throw;
			        }
		        }
	        }

	        var reversedFilterAttributes = filterAttributes.Reverse();
	        var methodExecutedContext = new MethodExecutedContext(methodExecutingContext);

	        foreach (var f in reversedFilterAttributes)
	        {
		        try
		        {
			        await f.OnMethodExecutedAsync(methodExecutedContext).ConfigureAwait(false);
		        }
		        catch (Exception ex)
		        {
			        var exContext = new MethodExceptionContext(ex, methodExecutedContext);
			        await HandleExceptionAsync(exceptionFilterAttributes, exContext);
			        if (!exContext.Handled)
			        {
				        throw;
			        }
		        }
	        }
			
	        if (methodExecutedContext.Result != null && methodExecutedContext.Result is TResult result)
	        {
		        return result;
	        }
			return default(TResult);
        }

        private async Task HandleAsync(IReadOnlyList<MethodFilterAttribute> filterAttributes, IReadOnlyList<ExceptionFilterAttribute> exceptionFilterAttributes, MethodExecutingContext methodExecutingContext)
        {
	        foreach (var f in filterAttributes)
	        {
		        try
		        {
			        if (methodExecutingContext.Result == null) await f.OnMethodExecutingAsync(methodExecutingContext).ConfigureAwait(false);
		        }
		        catch (Exception ex)
		        {
			        var exContext = new MethodExceptionContext(ex, methodExecutingContext);
			        await HandleExceptionAsync(exceptionFilterAttributes, exContext);
			        if (!exContext.Handled)
			        {
				        throw;
			        }
		        }
	        }

	        var reversedFilterAttributes = filterAttributes.Reverse();
	        var methodExecutedContext = new MethodExecutedContext(methodExecutingContext);

	        foreach (var f in reversedFilterAttributes)
	        {
		        try
		        {
			        await f.OnMethodExecutedAsync(methodExecutedContext).ConfigureAwait(false);
		        }
		        catch (Exception ex)
		        {
			        var exContext = new MethodExceptionContext(ex, methodExecutedContext);
			        await HandleExceptionAsync(exceptionFilterAttributes, exContext);
			        if (!exContext.Handled)
			        {
				        throw;
			        }
		        }
	        }
        }

        private async Task HandleExceptionAsync(IReadOnlyList<ExceptionFilterAttribute> exceptionFilterAttributes, MethodExceptionContext exceptionContext)
        {
            foreach (var f in exceptionFilterAttributes)
            {
                try
                {
                    if (!exceptionContext.Handled)
                    {
                        await f.OnExceptionAsync(exceptionContext).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    throw new AggregateException(ex.Message, ex, exceptionContext.Exception);
                }
            }
        }

        private List<Attribute> GetInvocationMethodFilterAttributes(IInvocation invocation, IDictionary<string, object> invocationContext)
        {
            var attributes = _attributeProvider.GetAttributes(invocation.Method, invocationContext).ToList();
            if (invocation.Method != invocation.MethodInvocationTarget)
            {
                attributes.AddRange(_attributeProvider.GetAttributes(invocation.MethodInvocationTarget, invocationContext));
            }
            return attributes;
        }
    }
}
