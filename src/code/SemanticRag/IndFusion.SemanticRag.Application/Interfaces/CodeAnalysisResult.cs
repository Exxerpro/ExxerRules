using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Represents the result of code analysis.
/// </summary>
public record CodeAnalysisResult
{
    /// <summary>
    /// List of pattern violations found.
    /// </summary>
    public required IReadOnlyList<PatternViolation> Violations { get; init; }

    /// <summary>
    /// List of pattern suggestions.
    /// </summary>
    public required IReadOnlyList<PatternSuggestion> Suggestions { get; init; }

    /// <summary>
    /// Overall compliance score (0-1).
    /// </summary>
    public required float ComplianceScore { get; init; }

    /// <summary>
    /// Time taken for analysis in milliseconds.
    /// </summary>
    public required long ElapsedMilliseconds { get; init; }

    /// <summary>
    /// Number of files analyzed.
    /// </summary>
    public required int FilesAnalyzed { get; init; }

    /// <summary>
    /// Total lines of code analyzed.
    /// </summary>
    public required int LinesOfCode { get; init; }
}