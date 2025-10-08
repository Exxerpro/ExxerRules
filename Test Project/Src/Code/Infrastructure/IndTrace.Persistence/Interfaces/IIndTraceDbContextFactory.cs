using Microsoft.EntityFrameworkCore;

namespace IndTrace.Persistence.Interfaces;

/// <summary>
/// Provides methods for creating IndTrace database context instances.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Ensure interface is open for extension but closed for modification (OCP - SOLID). Consider using default interface methods or extension methods for future-proofing.
public interface IIndTraceDbContextFactory : IAsyncDisposable
{
    /// <summary>
    /// Creates a new Entity Framework DbContext instance.
    /// </summary>
    /// <returns>A new DbContext instance.</returns>
    DbContext CreateEfDbContext();

    /// <summary>
    /// Asynchronously creates a new IndTrace database context instance.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A task representing the asynchronous operation, with a new IIndTraceDbContext instance.</returns>
    Task<IIndTraceDbContext> CreateDbContextAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Synchronously creates a new IndTrace database context instance.
    /// </summary>
    /// <returns>A task representing the synchronous operation, with a new IIndTraceDbContext instance.</returns>
    IIndTraceDbContext CreateDbContext();
}
