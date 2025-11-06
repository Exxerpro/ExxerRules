namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// A step in parallel service execution.
/// </summary>
/// <param name="ServiceName">Name of the service.</param>
/// <param name="InputData">Input data for the service.</param>
/// <param name="Priority">Priority of this service.</param>
/// <param name="TimeoutMs">Timeout for this service.</param>
public record ParallelServiceStep(
    string ServiceName,
    Dictionary<string, object> InputData,
    int Priority = 0,
    int TimeoutMs = 30000
);