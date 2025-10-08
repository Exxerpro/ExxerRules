// Src/Tests/Integration/Indtrace.Integration.Tests/Utilities/KeyedPersistenceRegistrationTest.cs
using IndTrace.Persistence.Interfaces;
using IndTrace.Dependencies.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using IndTrace.Persistence.DBContext;

namespace Integration.Tests.Utilities;

/// <summary>
/// Test-only helper to register keyed persistence (DbContext factory and repositories)
/// from allowed connection string profiles defined in <see cref="DbProfiles"/>.
/// </summary>
public static class KeyedPersistenceRegistrationTest
{
    /// <summary>
    /// Registers keyed <see cref="IIndTraceDbContextFactory"/> and repositories for all configured
    /// connection string profiles present in <see cref="DbProfiles.Allowed"/>.
    /// </summary>
    /// <param name="services">The DI container.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="logger">Logger for diagnostics.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddKeyedPersistenceFromConfigTest(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        var csSection = configuration.GetSection("ConnectionStrings");
        foreach (var child in csSection.GetChildren())
        {
            var key = child.Key;
            var conn = child.Value;
            if (string.IsNullOrWhiteSpace(conn))
            {
                continue;
            }

            if (!DbProfiles.Allowed.Contains(key))
            {
                continue;
            }

            // EXPLICIT: Register KEYED pooled DbContext factory for this specific connection
            services.AddKeyedSingleton<IDbContextFactory<IndTraceDbContext>>(key, (sp, _) =>
            {
                var services = new ServiceCollection();
                services.AddPooledDbContextFactory<IndTraceDbContext>(options =>
                    options.UseSqlServer(conn, actions =>
                    {
                        actions.MigrationsAssembly(typeof(IndTraceDbContext).Assembly.FullName)
                               .EnableRetryOnFailure(maxRetryCount: 4, maxRetryDelay: TimeSpan.FromSeconds(2),
                                   errorNumbersToAdd: []);
                    })
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .ConfigureWarnings(warnings =>
                    {
                        warnings.Default(WarningBehavior.Log)
                            .Log(CoreEventId.SaveChangesCompleted,
                                 CoreEventId.FirstWithoutOrderByAndFilterWarning,
                                 CoreEventId.RowLimitingOperationWithoutOrderByWarning);
                    }),
                    poolSize: 128); // EXPLICIT pool size per key

                var provider = services.BuildServiceProvider();
                return provider.GetRequiredService<IDbContextFactory<IndTraceDbContext>>();
            });

            // EXPLICIT: Bridge EF Core pooled factory to YOUR clean interface
            services.AddKeyedScoped<IIndTraceDbContextFactory>(key, (sp, _) =>
            {
                var pooledFactory = sp.GetRequiredKeyedService<IDbContextFactory<IndTraceDbContext>>(key);
                return new Integration.Tests.Adapters.PooledKeyedDbContextFactory(pooledFactory, conn);
            });

            // Repositories are not registered here to avoid unkeyed DI; tests resolve only what they need
        }

        return services;
    }

    // Repositories intentionally not registered in this helper; tests resolve only required services
}
