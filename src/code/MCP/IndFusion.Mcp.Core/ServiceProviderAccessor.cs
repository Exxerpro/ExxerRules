using Microsoft.Extensions.DependencyInjection;

namespace IndFusion.Mcp.Core;

/// <summary>
/// Provides access to the service provider for MCP tools.
/// This is a simple implementation that allows MCP tools to access services.
/// </summary>
public static class ServiceProviderAccessor
{
    private static IServiceProvider? _serviceProvider;

    /// <summary>
    /// Gets or sets the service provider.
    /// </summary>
    public static IServiceProvider? ServiceProvider
    {
        get => _serviceProvider;
        set => _serviceProvider = value;
    }

    /// <summary>
    /// Gets a service of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of service to get.</typeparam>
    /// <returns>The service instance, or null if not found.</returns>
    public static T? GetService<T>() where T : class
    {
        return _serviceProvider?.GetService<T>();
    }

    /// <summary>
    /// Gets a required service of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of service to get.</typeparam>
    /// <returns>The service instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the service is not found.</exception>
    public static T GetRequiredService<T>() where T : class
    {
        return _serviceProvider?.GetRequiredService<T>() 
            ?? throw new InvalidOperationException($"Service of type {typeof(T)} is not registered.");
    }
}
