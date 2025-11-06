namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Information about a workflow.
/// </summary>
/// <param name="Name">Name of the workflow.</param>
/// <param name="Description">Description of the workflow.</param>
/// <param name="Steps">Steps in the workflow.</param>
/// <param name="IsEnabled">Whether the workflow is enabled.</param>
public record WorkflowInfo(
    string Name,
    string Description,
    IEnumerable<WorkflowStep> Steps,
    bool IsEnabled
);