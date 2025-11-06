namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Performance settings for composition.
/// </summary>
/// <param name="MaxExecutionTimeMs">Maximum execution time.</param>
/// <param name="MaxMemoryUsage">Maximum memory usage.</param>
/// <param name="TimeoutMs">Timeout for individual services.</param>
/// <param name="RetryAttempts">Number of retry attempts.</param>
public record CompositionPerformanceSettings(
    long MaxExecutionTimeMs,
    long MaxMemoryUsage,
    int TimeoutMs,
    int RetryAttempts
);