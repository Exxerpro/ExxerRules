namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Settings for parallel execution.
/// </summary>
/// <param name="MaxConcurrency">Maximum number of concurrent services.</param>
/// <param name="CoordinationStrategy">Strategy for coordinating parallel execution.</param>
/// <param name="ResultAggregationStrategy">Strategy for aggregating results.</param>
public record ParallelExecutionSettings(
    int MaxConcurrency,
    string CoordinationStrategy,
    string ResultAggregationStrategy
);