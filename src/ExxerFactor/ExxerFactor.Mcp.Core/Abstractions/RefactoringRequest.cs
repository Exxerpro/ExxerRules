namespace ExxerFactor.Mcp.Core.Abstractions;

/// <summary>
/// Describes a request to execute a specific refactoring tool against a solution.
/// </summary>
public record ExxerFactoringRequest(
    /// <summary>The tool identifier to execute (e.g., "extract-method").</summary>
    string ToolName,
    /// <summary>Absolute path to the target solution file (.sln).</summary>
    string SolutionPath,
    /// <summary>Arbitrary parameters required by the selected tool.</summary>
    Dictionary<string, object> Parameters
);