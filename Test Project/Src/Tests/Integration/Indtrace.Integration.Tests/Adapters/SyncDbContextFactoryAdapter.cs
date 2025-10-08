using IndTrace.Persistence.DBContext;
using IndTrace.Persistence.Interfaces;
using MicrosoftEntityFrameworkDbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace Integration.Tests.Adapters;

/// <summary>
/// Adapts an async-only DbContext factory to support synchronous disposal for test DI scopes.
/// </summary>
internal sealed class SyncDbContextFactoryAdapter : IIndTraceDbContextFactory, IDisposable
{
    private readonly IndTraceDbContextFactory _inner;

    public SyncDbContextFactoryAdapter(IndTraceDbContextFactory inner)
    {
        _inner = inner;
    }

    public MicrosoftEntityFrameworkDbContext CreateEfDbContext() => _inner.CreateEfDbContext();

    public Task<IIndTraceDbContext> CreateDbContextAsync(CancellationToken cancellationToken)
        => _inner.CreateDbContextAsync(cancellationToken);

    public IIndTraceDbContext CreateDbContext() => _inner.CreateDbContext();

    public void Dispose()
    {
        // Bridge async disposal to sync to satisfy ServiceProviderEngineScope.Dispose()
        _inner.DisposeAsync().AsTask().GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _inner.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
