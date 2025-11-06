namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of a workflow step.
/// </summary>
/// <param name="StepName">Name of the step.</param>
/// <param name="Success">Whether the step succeeded.</param>
/// <param name="Result">Result from the step.</param>
/// <param name="ExecutionTimeMs">Time taken for the step.</param>
/// <param name="ErrorDetails">Error details if step failed.</param>
public record WorkflowStepResult(
    string StepName,
    bool Success,
    object Result,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);