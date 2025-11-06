namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for service chain execution.
/// </summary>
/// <param name="ChainName">Name of the service chain.</param>
/// <param name="Services">Services in the chain.</param>
/// <param name="DataFlow">Data flow between services.</param>
/// <param name="ValidationSteps">Validation steps between services.</param>
/// <param name="TimeoutMs">Timeout for the entire chain.</param>
public record ServiceChainRequest(
    string ChainName,
    IEnumerable<ServiceChainStep> Services,
    IEnumerable<DataFlowStep> DataFlow,
    IEnumerable<ValidationStep> ValidationSteps,
    int TimeoutMs = 30000
);