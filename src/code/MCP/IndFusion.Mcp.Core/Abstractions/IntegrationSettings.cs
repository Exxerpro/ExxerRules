namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Settings for external service integration.
/// </summary>
/// <param name="Authentication">Authentication settings.</param>
/// <param name="TimeoutMs">Timeout for external service calls.</param>
/// <param name="RetryAttempts">Number of retry attempts.</param>
/// <param name="RateLimiting">Rate limiting settings.</param>
/// <param name="Caching">Caching settings.</param>
public record IntegrationSettings(
    AuthenticationSettings Authentication,
    RateLimitingSettings RateLimiting,
    CachingSettings Caching,
    int TimeoutMs = 30000,
    int RetryAttempts = 3
);