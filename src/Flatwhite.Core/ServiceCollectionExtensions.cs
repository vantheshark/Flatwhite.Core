using Flatwhite.Core.Provider;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Flatwhite.Core
{
	/// <summary>
	/// Provide ServiceCollection extension methods to support Flatwhite module registrations and method filter attributes
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Register Flatwhite.Core components
		/// </summary>
		/// <param name="services"></param>
		/// <param name="attributeProvider"></param>
		/// <param name="contextProvider"></param>
		public static IServiceCollection UseFlatwhiteFilters(this IServiceCollection services, IAttributeProvider attributeProvider = null, IContextProvider contextProvider = null)
		{
			services.AddSingleton(contextProvider ?? new EmptyContextProvider());
			services.AddSingleton(sp => attributeProvider ?? new DefaultAttributeProvider(sp));
			return services;
		}

		/// <summary>
		/// Register a proxy with attribute interceptors
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TImpl"></typeparam>
		/// <param name="services"></param>
		/// <param name="lifetime"></param>
		public static void RegisterWithMethodFilters<T, TImpl>(this IServiceCollection services, ServiceLifetime lifetime) where TImpl : class, T where T : class 
		{
			services.AddSingleton<TImpl>();
			RegisterWithMethodFilters<T, TImpl>(services, sp => sp.GetRequiredService<TImpl>(), lifetime);
		}

		/// <summary>
		/// Register a proxy with attribute interceptors
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TImpl"></typeparam>
		/// <param name="services"></param>
		/// <param name="target"></param>
		/// <param name="lifetime"></param>
		public static void RegisterWithMethodFilters<T, TImpl>(this IServiceCollection services, TImpl target, ServiceLifetime lifetime) where TImpl : class, T where T : class 
		{
			RegisterWithMethodFilters<T, TImpl>(services, sp => target, lifetime);
		}

		/// <summary>
		/// Register a proxy with attribute interceptors
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TImpl"></typeparam>
		/// <param name="services"></param>
		/// <param name="targetResolver"></param>
		/// <param name="lifetime"></param>
		public static void RegisterWithMethodFilters<T, TImpl>(this IServiceCollection services, Func<IServiceProvider, TImpl> targetResolver, ServiceLifetime lifetime) where TImpl : class, T where T : class 
		{
			Func<IServiceProvider, T> implementationFactory = serviceProvider => MethodInterceptor<T>.Create(
				targetResolver(serviceProvider), 
				serviceProvider.GetRequiredService<IContextProvider>(), 
				serviceProvider.GetRequiredService<IAttributeProvider>());
			var serviceDescriptor = new ServiceDescriptor(typeof(T), implementationFactory, lifetime);
			services.Add(serviceDescriptor);
		}

		/// <summary>
		/// Register proxy without target
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="services"></param>
		/// <param name="lifetime"></param>
		public static void AddProxyWithoutTarget<T>(this IServiceCollection services, ServiceLifetime lifetime) where T : class
		{
			Func<IServiceProvider, T> implementationFactory = serviceProvider => MethodInterceptor<T>.Create(
				null, 
				serviceProvider.GetRequiredService<IContextProvider>(), 
				serviceProvider.GetRequiredService<IAttributeProvider>());
			var serviceDescriptor = new ServiceDescriptor(typeof(T), implementationFactory, lifetime);
			services.Add(serviceDescriptor);
		}
	}
}
