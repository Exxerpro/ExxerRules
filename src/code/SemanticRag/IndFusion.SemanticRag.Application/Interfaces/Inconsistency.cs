using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Represents an inconsistency found in the code.
/// </summary>
public record Inconsistency
{
    /// <summary>
    /// Description of the inconsistency.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Severity of the inconsistency.
    /// </summary>
    public required PatternSeverity Severity { get; init; }

    /// <summary>
    /// File path where the inconsistency was found.
    /// </summary>
    public required string FilePath { get; init; }

    /// <summary>
    /// Line number where the inconsistency was found.
    /// </summary>
    public required int LineNumber { get; init; }

    /// <summary>
    /// Suggested fix for the inconsistency.
    /// </summary>
    public string? SuggestedFix { get; init; }
}