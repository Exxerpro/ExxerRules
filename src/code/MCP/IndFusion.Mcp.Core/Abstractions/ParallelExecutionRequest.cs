namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for parallel service execution.
/// </summary>
/// <param name="ExecutionName">Name of the parallel execution.</param>
/// <param name="Services">Services to execute in parallel.</param>
/// <param name="CoordinationStrategy">Strategy for coordinating parallel execution.</param>
/// <param name="ResultAggregation">Strategy for aggregating results.</param>
/// <param name="TimeoutMs">Timeout for parallel execution.</param>
public record ParallelExecutionRequest(
    string ExecutionName,
    IEnumerable<ParallelServiceStep> Services,
    string CoordinationStrategy,
    string ResultAggregation,
    int TimeoutMs = 30000
);