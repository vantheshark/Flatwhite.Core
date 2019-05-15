using System;
using System.Reflection;

namespace Flatwhite.Core
{
    /// <summary>
    /// Copy from Castle.Core.Invocation
    /// </summary>
    public interface IInvocation
    {
        object[] Arguments { get; }

        Type[] GenericArguments { get; }

        object InvocationTarget { get; }

        MethodInfo Method { get; }

        MethodInfo MethodInvocationTarget { get; }

        object Proxy { get; }

        object ReturnValue { get; set; }

        Type TargetType { get; }

	    void Proceed();
    }

	internal class Invocation : IInvocation
	{
		public object[] Arguments { get; set;}
		public Type[] GenericArguments { get; set;}
		public object InvocationTarget { get; set;}
		public MethodInfo Method { get; set;}
		public MethodInfo MethodInvocationTarget { get; set;}
		public object Proxy { get; set;}
		public object ReturnValue { get; set; }
		public Type TargetType { get; set;}

		public void Proceed()
		{
			ReturnValue = Method.Invoke(InvocationTarget, Arguments);
		}
	}
}