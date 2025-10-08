namespace IndTrace.Dependencies.Interceptors;

/// <summary>
/// Provides extension methods for registering services with dynamic proxy interception in the dependency injection container.
/// </summary>
public static class InterceptorServiceCollectionExtensions
{
    private static readonly ProxyGenerator ProxyGenerator = new();

    /// <summary>
    /// Registers a scoped service with an interceptor for the specified interface and implementation.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to register.</typeparam>
    /// <typeparam name="TImplementation">The implementation type to register.</typeparam>
    /// <typeparam name="TInterceptor">The interceptor type to use.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate interceptor service collection extension logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    public static IServiceCollection AddInterceptedScoped<TInterface, TImplementation, TInterceptor>(
        this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TInterceptor : class, IInterceptor
    {
        services.AddScoped<TImplementation>();
        services.AddScoped<TInterceptor>();
        services.AddScoped<TInterface>(provider =>
        {
            var implementation = provider.GetRequiredService<TImplementation>();
            var interceptor = provider.GetRequiredService<TInterceptor>();

            return ProxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, interceptor);
        });

        return services;
    }

    /// <summary>
    /// Registers a singleton service with an interceptor for the specified interface and implementation.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to register.</typeparam>
    /// <typeparam name="TImplementation">The implementation type to register.</typeparam>
    /// <typeparam name="TInterceptor">The interceptor type to use.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    //TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated extension or registration logic. Refactor for maintainability if necessary.
    public static IServiceCollection AddInterceptedSingleton<TInterface, TImplementation, TInterceptor>(
        this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TInterceptor : class, IInterceptor
    {
        services.AddSingleton<TImplementation>();
        services.AddSingleton<TInterceptor>();
        services.AddSingleton<TInterface>(provider =>
        {
            var implementation = provider.GetRequiredService<TImplementation>();
            var interceptor = provider.GetRequiredService<TInterceptor>();

            return ProxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, interceptor);
        });

        return services;
    }

    /// <summary>
    /// Registers a transient service with an interceptor for the specified interface and implementation.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to register.</typeparam>
    /// <typeparam name="TImplementation">The implementation type to register.</typeparam>
    /// <typeparam name="TInterceptor">The interceptor type to use.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    //TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency registration operations, consider optimizing service registration and memory usage.
    public static IServiceCollection AddInterceptedTransient<TInterface, TImplementation, TInterceptor>(
        this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TInterceptor : class, IInterceptor
    {
        services.AddTransient<TImplementation>();
        services.AddTransient<TInterceptor>();
        services.AddTransient<TInterface>(provider =>
        {
            var implementation = provider.GetRequiredService<TImplementation>();
            var interceptor = provider.GetRequiredService<TInterceptor>();

            return ProxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, interceptor);
        });

        return services;
    }
}
