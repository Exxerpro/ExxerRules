namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of workflow orchestration.
/// </summary>
/// <param name="Success">Whether the workflow succeeded.</param>
/// <param name="WorkflowName">Name of the executed workflow.</param>
/// <param name="StepResults">Results from individual steps.</param>
/// <param name="FinalResult">Final result of the workflow.</param>
/// <param name="ExecutionTimeMs">Time taken for the workflow.</param>
/// <param name="ErrorDetails">Error details if workflow failed.</param>
public record WorkflowOrchestrationResult(
    bool Success,
    string WorkflowName,
    IEnumerable<WorkflowStepResult> StepResults,
    object FinalResult,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);