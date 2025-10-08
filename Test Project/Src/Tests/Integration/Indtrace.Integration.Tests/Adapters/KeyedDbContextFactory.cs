using IndTrace.Persistence.DBContext;
using IndTrace.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using MicrosoftEntityFrameworkDbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace Integration.Tests.Adapters;

internal sealed class KeyedDbContextFactory : IIndTraceDbContextFactory, IDisposable
{
    private readonly string _connectionString;

    public KeyedDbContextFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public MicrosoftEntityFrameworkDbContext CreateEfDbContext()
    {
        var options = CreateOptions();
        return new IndTraceDbContext(options);
    }

    public IIndTraceDbContext CreateDbContext()
    {
        var options = CreateOptions();
        return new IndTraceDbContext(options);
    }

    public Task<IIndTraceDbContext> CreateDbContextAsync(CancellationToken cancellationToken)
    {
        IIndTraceDbContext ctx = new IndTraceDbContext(CreateOptions());
        return Task.FromResult(ctx);
    }

    private DbContextOptions<IndTraceDbContext> CreateOptions()
    {
        var builder = new DbContextOptionsBuilder<IndTraceDbContext>();
        builder.UseSqlServer(_connectionString, actions =>
        {
            actions.MigrationsAssembly(typeof(IndTraceDbContext).Assembly.FullName)
                   .EnableRetryOnFailure(maxRetryCount: 4, maxRetryDelay: TimeSpan.FromSeconds(2), errorNumbersToAdd: Array.Empty<int>());
        })
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();
        return builder.Options;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        // Bridge async disposal to sync path for DI container disposal
        DisposeAsync().GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }
}
