using IndTrace.Application.Repository;
using IndTrace.Persistence.Interfaces;
using IndTrace.Persistence.Repositories;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IndTrace.Dependencies.Repositories;

/// <summary>
/// Provides extension methods for registering repository services with the dependency injection container.
/// </summary>
public static class RepositoryRegistration
{
    /// <summary>
    /// Registers a repository implementation for the specified interface with the given service lifetime.
    /// Intended for repositories that support both read and write operations.
    /// </summary>
    /// <typeparam name="TInterface">The repository interface type. Must implement <see cref="IRepository{T}"/>.</typeparam>
    /// <typeparam name="TImplementation">The concrete implementation type of the repository.</typeparam>
    /// <typeparam name="T">The entity type for the repository.</typeparam>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <param name="lifetime">The desired service lifetime (Singleton, Scoped, or Transient).</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddRepository<TInterface, TImplementation, T>
        (this IServiceCollection services, ServiceLifetime lifetime)
        where TInterface : class, IRepository<T>
        where TImplementation : class, TInterface
        where T : class
    {
        // If no key provided, register as before (unkeyed)

        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton<TInterface, TImplementation>();

                break;

            case ServiceLifetime.Scoped:
                services.AddScoped<TInterface, TImplementation>();
                break;

            case ServiceLifetime.Transient:
                services.AddTransient<TInterface, TImplementation>();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }
        return services;
    }

    /// <summary>
    /// Registers a read-only repository implementation for the specified interface with the given service lifetime.
    /// Intended for repositories that only support read operations.
    /// </summary>
    /// <typeparam name="TInterface">The read-only repository interface type. Must implement <see cref="IReadOnlyRepository{T}"/>.</typeparam>
    /// <typeparam name="TImplementation">The concrete implementation type of the read-only repository.</typeparam>
    /// <typeparam name="T">The entity type for the repository.</typeparam>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <param name="lifetime">The desired service lifetime (Singleton, Scoped, or Transient).</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddReadOnlyRepository<TInterface, TImplementation, T>
        (this IServiceCollection services, ServiceLifetime lifetime)
        where TInterface : class, IReadOnlyRepository<T>
        where TImplementation : class, TInterface
        where T : class
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton<TInterface, TImplementation>();
                break;

            case ServiceLifetime.Scoped:
                services.AddScoped<TInterface, TImplementation>();
                break;

            case ServiceLifetime.Transient:
                services.AddTransient<TInterface, TImplementation>();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }
        return services;
    }
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate repository registration logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
