namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for pattern analysis operations.
/// </summary>
/// <param name="ProjectPath">Path to the project to analyze.</param>
/// <param name="PatternType">Type of patterns to analyze for.</param>
/// <param name="Scope">Analysis scope (file, directory, project).</param>
/// <param name="IncludeMetrics">Whether to include pattern metrics.</param>
/// <param name="GenerateReport">Whether to generate a detailed report.</param>
public record PatternAnalysisRequest(
    string ProjectPath,
    string PatternType,
    string Scope = "project",
    bool IncludeMetrics = true,
    bool GenerateReport = false
);