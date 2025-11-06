namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// A step in external service integration.
/// </summary>
/// <param name="StepName">Name of the step.</param>
/// <param name="StepType">Type of the step.</param>
/// <param name="Description">Description of the step.</param>
/// <param name="RequiredParameters">Parameters required for the step.</param>
public record IntegrationStep(
    string StepName,
    string StepType,
    string Description,
    IEnumerable<string> RequiredParameters
);