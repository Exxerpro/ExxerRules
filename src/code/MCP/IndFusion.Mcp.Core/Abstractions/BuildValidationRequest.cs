namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for build validation operations.
/// </summary>
/// <param name="SolutionPath">Path to the solution file.</param>
/// <param name="TransformedFiles">Files that have been transformed.</param>
/// <param name="ValidationOptions">Options for validation.</param>
/// <param name="RunAnalyzers">Whether to run analyzers for validation.</param>
/// <param name="BuildValidation">Whether to perform build validation.</param>
/// <param name="CheckForNewIssues">Whether to check for new issues.</param>
public record BuildValidationRequest(
    string SolutionPath,
    IEnumerable<TransformedFile> TransformedFiles,
    TransformationValidationOptions ValidationOptions,
    bool RunAnalyzers = true,
    bool BuildValidation = true,
    bool CheckForNewIssues = true
);