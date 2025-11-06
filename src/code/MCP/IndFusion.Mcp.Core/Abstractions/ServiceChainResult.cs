namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of service chain execution.
/// </summary>
/// <param name="Success">Whether the chain succeeded.</param>
/// <param name="ChainName">Name of the executed chain.</param>
/// <param name="ServiceResults">Results from individual services.</param>
/// <param name="ChainResult">Final result of the chain.</param>
/// <param name="ExecutionTimeMs">Time taken for the chain.</param>
/// <param name="ErrorDetails">Error details if chain failed.</param>
public record ServiceChainResult(
    bool Success,
    string ChainName,
    IEnumerable<ServiceChainStepResult> ServiceResults,
    object ChainResult,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);