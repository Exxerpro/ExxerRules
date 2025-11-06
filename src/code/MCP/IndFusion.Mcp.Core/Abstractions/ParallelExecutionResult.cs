namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of parallel service execution.
/// </summary>
/// <param name="Success">Whether the parallel execution succeeded.</param>
/// <param name="ExecutionName">Name of the parallel execution.</param>
/// <param name="ServiceResults">Results from individual services.</param>
/// <param name="AggregatedResult">Aggregated result from all services.</param>
/// <param name="ExecutionTimeMs">Time taken for parallel execution.</param>
/// <param name="ErrorDetails">Error details if execution failed.</param>
public record ParallelExecutionResult(
    bool Success,
    string ExecutionName,
    IEnumerable<ParallelServiceStepResult> ServiceResults,
    object AggregatedResult,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);