namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Performance metrics for service composition.
/// </summary>
/// <param name="TotalExecutionTimeMs">Total execution time.</param>
/// <param name="ServiceExecutionTimes">Execution times for individual services.</param>
/// <param name="OverheadTimeMs">Overhead time for composition.</param>
/// <param name="MemoryUsage">Memory usage during composition.</param>
public record CompositionPerformanceMetrics(
    long TotalExecutionTimeMs,
    Dictionary<string, long> ServiceExecutionTimes,
    long OverheadTimeMs,
    long MemoryUsage
);