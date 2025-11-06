namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Caching settings for external services.
/// </summary>
/// <param name="Enabled">Whether caching is enabled.</param>
/// <param name="CacheDuration">Duration to cache results.</param>
/// <param name="CacheKeyStrategy">Strategy for generating cache keys.</param>
public record CachingSettings(
    bool Enabled,
    TimeSpan CacheDuration,
    string CacheKeyStrategy
);