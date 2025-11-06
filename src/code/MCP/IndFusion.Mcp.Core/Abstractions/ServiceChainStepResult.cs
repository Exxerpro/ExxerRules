namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of a service chain step.
/// </summary>
/// <param name="ServiceName">Name of the service.</param>
/// <param name="Success">Whether the service succeeded.</param>
/// <param name="Result">Result from the service.</param>
/// <param name="ExecutionTimeMs">Time taken for the service.</param>
/// <param name="ErrorDetails">Error details if service failed.</param>
public record ServiceChainStepResult(
    string ServiceName,
    bool Success,
    object Result,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);