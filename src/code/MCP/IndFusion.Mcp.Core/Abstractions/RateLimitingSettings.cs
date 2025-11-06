namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Rate limiting settings for external services.
/// </summary>
/// <param name="RequestsPerMinute">Maximum requests per minute.</param>
/// <param name="BurstLimit">Burst limit for requests.</param>
/// <param name="BackoffStrategy">Strategy for backoff on rate limit.</param>
public record RateLimitingSettings(
    int RequestsPerMinute,
    int BurstLimit,
    string BackoffStrategy
);