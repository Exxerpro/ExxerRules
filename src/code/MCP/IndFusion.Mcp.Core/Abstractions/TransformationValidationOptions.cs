namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Options for transformation validation.
/// </summary>
/// <param name="RunAnalyzers">Whether to run analyzers.</param>
/// <param name="BuildValidation">Whether to perform build validation.</param>
/// <param name="CheckForNewIssues">Whether to check for new issues.</param>
/// <param name="TimeoutMs">Timeout for validation in milliseconds.</param>
/// <param name="SeverityThreshold">Minimum severity threshold for issues.</param>
public record TransformationValidationOptions(
    bool RunAnalyzers = true,
    bool BuildValidation = true,
    bool CheckForNewIssues = true,
    int TimeoutMs = 30000,
    string SeverityThreshold = "Warning"
);