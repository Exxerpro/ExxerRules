using IndTrace.Application.Repository;
using Microsoft.Extensions.Logging;
using ZiggyCreatures.Caching.Fusion;

namespace IndTrace.Persistence.Caching;

/// <summary>
/// FusionCache implementation of ICacheService.
/// Provides Redis-like caching operations using FusionCache.
/// </summary>
public class FusionCacheService : ICacheService
{
    private readonly IFusionCache _fusionCache;
    private readonly ILogger<FusionCacheService> _logger;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromHours(1);

    /// <summary>
    /// Initializes a new instance of the <see cref="FusionCacheService"/> class.
    /// </summary>
    /// <param name="fusionCache">The FusionCache instance.</param>
    /// <param name="logger">The logger.</param>
    public FusionCacheService(IFusionCache fusionCache, ILogger<FusionCacheService> logger)
    {
        _fusionCache = fusionCache ?? throw new ArgumentNullException(nameof(fusionCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogError("Cache key cannot be null or empty");
            return null;
        }

        if (factory is null)
        {
            _logger.LogError("Factory function cannot be null");
            return null;
        }

        try
        {
            return await _fusionCache.GetOrSetAsync<T?>(
                key,
                async (ctx, ct) => await factory(ct).ConfigureAwait(false),
                options => options
                    .SetDuration(expiration ?? _defaultExpiration)
                    .SetFailSafe(true),
                //.SetFactorySoftTimeout(TimeSpan.FromMilliseconds(100))
                //  .SetFactoryHardTimeout(TimeSpan.FromSeconds(2)),
                token: cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetOrSetAsync for key {Key}", key);
            // With fail-safe enabled, this should rarely happen
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogError("Cache key cannot be null or empty");
            return false;
        }

        try
        {
            await _fusionCache.RemoveAsync(key, token: cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing key {Key}", key);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<int> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            _logger.LogError("Pattern cannot be null or empty");
            return 0;
        }

        // FusionCache doesn't have pattern-based removal out of the box
        // This would require custom implementation or key tracking
        _logger.LogWarning("RemoveByPatternAsync not implemented for pattern {Pattern}", pattern);
        await Task.CompletedTask;
        return 0;
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogError("Cache key cannot be null or empty");
            return;
        }

        if (value is null)
        {
            _logger.LogWarning("Attempting to cache null value for key {Key}", key);
            return;
        }

        try
        {
            await _fusionCache.SetAsync(
                key,
                value,
                options => options.SetDuration(expiration ?? _defaultExpiration),
                token: cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value for key {Key}", key);
        }
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogError("Cache key cannot be null or empty");
            return null;
        }

        try
        {
            var result = await _fusionCache.TryGetAsync<T>(key, token: cancellationToken).ConfigureAwait(false);
            return result.HasValue ? result.Value : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting value for key {Key}", key);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogError("Cache key cannot be null or empty");
            return false;
        }

        try
        {
            var result = await _fusionCache.TryGetAsync<object>(key, token: cancellationToken).ConfigureAwait(false);
            return result.HasValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence for key {Key}", key);
            return false;
        }
    }
}
