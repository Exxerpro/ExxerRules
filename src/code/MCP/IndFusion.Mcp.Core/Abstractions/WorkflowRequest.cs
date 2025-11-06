namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for workflow orchestration.
/// </summary>
/// <param name="WorkflowName">Name of the workflow to execute.</param>
/// <param name="Steps">Steps in the workflow.</param>
/// <param name="Context">Context for the workflow.</param>
/// <param name="ParallelExecution">Whether steps can be executed in parallel.</param>
/// <param name="ErrorHandling">Error handling strategy.</param>
public record WorkflowRequest(
    string WorkflowName,
    IEnumerable<WorkflowStep> Steps,
    Dictionary<string, object> Context,
    bool ParallelExecution = false,
    string ErrorHandling = "StopOnError"
);