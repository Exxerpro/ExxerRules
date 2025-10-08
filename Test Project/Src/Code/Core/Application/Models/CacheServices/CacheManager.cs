// <copyright file="CacheManager.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.CacheServices;

/// <summary>
/// Represents the CacheManager.
/// </summary>
public class CacheManager<T>(TimeSpan cacheDuration)
{
    private T? cachedData;
    private DateTime? cacheTimestamp;
    private readonly SemaphoreSlim semaphore = new(1, 1);

    /// <summary>
    /// Gets the cached data if valid, or refreshes it using the provided function.
    /// </summary>
    /// <param name="refreshFunc">The function to refresh the data if the cache is invalid or expired.</param>
    /// <param name="forceRefresh">Whether to force a refresh of the data, even if the cache is valid.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <param name="logger">An optional logger for logging cache operations.</param>
    /// <returns>The cached or refreshed data.</returns>
    public async Task<T?> GetOrRefreshAsync(
        Func<Task<T>> refreshFunc,
        bool forceRefresh = false,
        CancellationToken cancellationToken = default,
        ILogger? logger = default)
    {
        await this.semaphore.WaitAsync(cancellationToken);
        try
        {
            if (this.cachedData != null && !forceRefresh && this.cacheTimestamp.HasValue && DateTime.UtcNow - this.cacheTimestamp.Value <= cacheDuration)
            {
                if (logger is not null)
                {
                    logger.LogInformation("Returning cached data");
                }

                return this.cachedData;
            }

            this.cachedData = await refreshFunc();
            this.cacheTimestamp = DateTime.UtcNow;

            if (logger is not null)
            {
                logger.LogInformation("Returning refreshed data");
            }

            return this.cachedData;
        }
        finally
        {
            this.semaphore.Release();
        }
    }

    /// <summary>
    /// Invalidates the current cache, clearing the cached data and timestamp.
    /// </summary>
    public void InvalidateCache()
    {
        this.cachedData = default;
        this.cacheTimestamp = null;
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate cache keys and values defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    // TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated cache management or eviction logic. Refactor for maintainability if necessary.
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency cache operations, consider optimizing data structures or eviction policies.
}
