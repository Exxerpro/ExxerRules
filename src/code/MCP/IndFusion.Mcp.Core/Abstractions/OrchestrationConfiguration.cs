namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Configuration for orchestration.
/// </summary>
/// <param name="AvailableWorkflows">Available workflows.</param>
/// <param name="ServiceChains">Available service chains.</param>
/// <param name="ParallelExecutionSettings">Settings for parallel execution.</param>
/// <param name="ErrorHandlingStrategies">Available error handling strategies.</param>
/// <param name="Version">Version of the configuration.</param>
/// <param name="LastUpdated">When the configuration was last updated.</param>
public record OrchestrationConfiguration(
    IEnumerable<WorkflowInfo> AvailableWorkflows,
    IEnumerable<ServiceChainInfo> ServiceChains,
    ParallelExecutionSettings ParallelExecutionSettings,
    IEnumerable<ErrorHandlingStrategy> ErrorHandlingStrategies,
    string Version,
    DateTime LastUpdated
);