using IndTrace.Persistence.DBContext;
using IndTrace.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MicrosoftEntityFrameworkDbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace Integration.Tests.Adapters;

/// <summary>
/// Pooled version of KeyedDbContextFactory for improved performance.
/// Uses EF Core's built-in pooling to avoid the "154 Context Creation Monster" issue.
/// </summary>
internal sealed class PooledKeyedDbContextFactory : IIndTraceDbContextFactory, IDisposable
{
    private readonly IDbContextFactory<IndTraceDbContext> _pooledFactory;
    private readonly string _connectionString;

    public PooledKeyedDbContextFactory(IDbContextFactory<IndTraceDbContext> pooledFactory, string connectionString)
    {
        _pooledFactory = pooledFactory;
        _connectionString = connectionString;
    }

    public MicrosoftEntityFrameworkDbContext CreateEfDbContext()
    {
        return _pooledFactory.CreateDbContext();
    }

    public IIndTraceDbContext CreateDbContext()
    {
        return _pooledFactory.CreateDbContext();
    }

    public async Task<IIndTraceDbContext> CreateDbContextAsync(CancellationToken cancellationToken)
    {
        return await _pooledFactory.CreateDbContextAsync(cancellationToken);
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
