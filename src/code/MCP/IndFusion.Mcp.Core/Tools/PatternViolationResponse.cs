using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Response model for pattern violation operations.
/// </summary>
/// <param name="Violations">The pattern violations found.</param>
/// <param name="TotalViolations">Total number of violations.</param>
/// <param name="FilePath">File path where violations were found.</param>
/// <param name="AnalysisTimeMs">Time taken for analysis in milliseconds.</param>
public readonly record struct PatternViolationResponse(
    IReadOnlyList<PatternViolation> Violations,
    int TotalViolations,
    string? FilePath,
    long AnalysisTimeMs);