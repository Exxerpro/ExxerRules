namespace IndTrace.Application.Repository;

/// <summary>
/// Cache service abstraction for repository pattern.
/// Designed to support Redis-like operations for future flexibility.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a value from cache or creates it using the provided factory.
    /// Redis-like GET with automatic SET if not found.
    /// </summary>
    /// <typeparam name="T">The type of value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">Factory to create the value if not in cache.</param>
    /// <param name="expiration">Optional expiration time.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached or newly created value.</returns>
    Task<T?> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Removes a value from cache.
    /// Redis-like DEL operation.
    /// </summary>
    /// <param name="key">The cache key to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the key was removed, false if not found.</returns>
    Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all values matching a pattern.
    /// Redis-like DEL with pattern matching.
    /// </summary>
    /// <param name="pattern">The pattern to match keys.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of keys removed.</returns>
    Task<int> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a value in cache with optional expiration.
    /// Redis-like SET operation.
    /// </summary>
    /// <typeparam name="T">The type of value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">Optional expiration time.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Gets a value from cache.
    /// Redis-like GET operation.
    /// </summary>
    /// <typeparam name="T">The type of value to retrieve.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached value or null if not found.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Checks if a key exists in cache.
    /// Redis-like EXISTS operation.
    /// </summary>
    /// <param name="key">The cache key to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
