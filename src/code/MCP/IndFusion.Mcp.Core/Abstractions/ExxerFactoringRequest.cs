namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Describes a request to execute a specific refactoring tool against a solution.
/// </summary>
/// <param name="ToolName">The tool identifier to execute (e.g., "extract-method").</param>
/// <param name="SolutionPath">Absolute path to the target solution file (.sln).</param>
/// <param name="Parameters">Arbitrary parameters required by the selected tool.</param>
public record ExxerFactoringRequest(
    string ToolName,
    string SolutionPath,
    Dictionary<string, object> Parameters
);