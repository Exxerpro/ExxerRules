namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// A step in a workflow.
/// </summary>
/// <param name="StepName">Name of the step.</param>
/// <param name="ServiceName">Service to execute in this step.</param>
/// <param name="InputMapping">Mapping of inputs for this step.</param>
/// <param name="OutputMapping">Mapping of outputs from this step.</param>
/// <param name="Dependencies">Dependencies on other steps.</param>
/// <param name="TimeoutMs">Timeout for this step.</param>
public record WorkflowStep(
    string StepName,
    string ServiceName,
    Dictionary<string, object> InputMapping,
    Dictionary<string, object> OutputMapping,
    IEnumerable<string> Dependencies,
    int TimeoutMs = 30000
);