using IndTrace.Application.Repository;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Interfaces;
using IndTrace.Persistence.Interfaces;
using IndTrace.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Integration.Tests.Infrastructure;

/// <summary>
/// Extension methods for registering services as keyed services for integration tests.
/// Uses the DbProfiles.Allowed keys to support multi-database testing.
/// </summary>
public static class KeyedServiceRegistrationExtensions
{
    /// <summary>
    /// Registers all repository types as keyed services for the specified database profile.
    /// </summary>
    public static IServiceCollection AddKeyedRepositoriesForProfile(
        this IServiceCollection services,
        string profile,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        // Register each repository type with the profile key
        services.AddKeyedRepository<IRepository<BarCode>, Repository<BarCode>, BarCode>(profile, lifetime);
        services.AddKeyedRepository<IRepository<ConfigApp>, Repository<ConfigApp>, ConfigApp>(profile, lifetime);
        services.AddKeyedRepository<IRepository<Customer>, Repository<Customer>, Customer>(profile, lifetime);
        services.AddKeyedRepository<IRepository<Cycle>, Repository<Cycle>, Cycle>(profile, lifetime);
        services.AddKeyedRepository<IRepository<DistinctRegister>, Repository<DistinctRegister>, DistinctRegister>(profile, lifetime);
        services.AddKeyedRepository<IRepository<Line>, Repository<Line>, Line>(profile, lifetime);
        services.AddKeyedRepository<IRepository<Machine>, Repository<Machine>, Machine>(profile, lifetime);
        services.AddKeyedRepository<IRepository<MachinePlc>, Repository<MachinePlc>, MachinePlc>(profile, lifetime);
        services.AddKeyedRepository<IRepository<MasterLabel>, Repository<MasterLabel>, MasterLabel>(profile, lifetime);
        services.AddKeyedRepository<IRepository<Plc>, Repository<Plc>, Plc>(profile, lifetime);
        services.AddKeyedRepository<IRepository<Product>, Repository<Product>, Product>(profile, lifetime);
        services.AddKeyedRepository<IRepository<Recipe>, Repository<Recipe>, Recipe>(profile, lifetime);
        services.AddKeyedRepository<IRepository<Register>, Repository<Register>, Register>(profile, lifetime);
        services.AddKeyedRepository<IRepository<Rule>, Repository<Rule>, Rule>(profile, lifetime);
        services.AddKeyedRepository<IRepository<Shift>, Repository<Shift>, Shift>(profile, lifetime);
        services.AddKeyedRepository<IRepository<TaskGatewayRequest>, Repository<TaskGatewayRequest>, TaskGatewayRequest>(profile, lifetime);
        services.AddKeyedRepository<IRepository<TaskGatewayResponse>, Repository<TaskGatewayResponse>, TaskGatewayResponse>(profile, lifetime);
        services.AddKeyedRepository<IRepository<Variable>, Repository<Variable>, Variable>(profile, lifetime);
        services.AddKeyedRepository<IRepository<VariablesGroup>, Repository<VariablesGroup>, VariablesGroup>(profile, lifetime);
        services.AddKeyedRepository<IRepository<WorkFlow>, Repository<WorkFlow>, WorkFlow>(profile, lifetime);

        return services;
    }

    /// <summary>
    /// Registers a single repository as a keyed service.
    /// </summary>
    private static IServiceCollection AddKeyedRepository<TInterface, TImplementation, TEntity>(
        this IServiceCollection services,
        string profile,
        ServiceLifetime lifetime)
        where TInterface : class, IRepository<TEntity>
        where TImplementation : class, TInterface
        where TEntity : class
    {
        var factory = (IServiceProvider sp, object? key) =>
        {
            var dbContextFactory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(profile);
            var logger = sp.GetRequiredService<ILogger<TImplementation>>();
            return ActivatorUtilities.CreateInstance<TImplementation>(sp, dbContextFactory, logger);
        };

        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddKeyedSingleton<TInterface>(profile, factory);
                break;
            case ServiceLifetime.Scoped:
                services.AddKeyedScoped<TInterface>(profile, factory);
                break;
            case ServiceLifetime.Transient:
                services.AddKeyedTransient<TInterface>(profile, factory);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }

        return services;
    }

    /// <summary>
    /// Registers read-only repositories as keyed services for the specified database profile.
    /// </summary>
    public static IServiceCollection AddKeyedReadOnlyRepositoriesForProfile(
        this IServiceCollection services,
        string profile,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        // Add specific read-only repository registrations here when needed
        // Example:
        // services.AddKeyedReadOnlyRepository<IBarCodeReadOnlyRepository, BarCodeReadOnlyRepository, BarCode>(profile, lifetime);

        return services;
    }

    /// <summary>
    /// Registers extended/specialized repositories as keyed services.
    /// </summary>
    public static IServiceCollection AddKeyedExtendedRepositoriesForProfile(
        this IServiceCollection services,
        string profile,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        // Add specific extended repository registrations here when needed
        // These are repositories with custom methods beyond basic CRUD

        return services;
    }

    /// <summary>
    /// Registers application services that depend on repositories as keyed services.
    /// </summary>
    public static IServiceCollection AddKeyedApplicationServicesForProfile(
        this IServiceCollection services,
        string profile)
    {
        // Register application services that need keyed repositories
        // Example:
        // services.AddKeyedScoped<IBarCodeService>(profile, (sp, _) =>
        // {
        //     var repository = sp.GetRequiredKeyedService<IRepository<BarCode>>(profile);
        //     return new BarCodeService(repository);
        // });

        return services;
    }
}
