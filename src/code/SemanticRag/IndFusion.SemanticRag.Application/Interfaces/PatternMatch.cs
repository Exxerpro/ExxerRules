using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Represents a pattern match result.
/// </summary>
public record PatternMatch
{
    /// <summary>
    /// The matched pattern.
    /// </summary>
    public required PatternDefinition Pattern { get; init; }

    /// <summary>
    /// Similarity score between 0 and 1.
    /// </summary>
    public required float Similarity { get; init; }

    /// <summary>
    /// Confidence score for the match.
    /// </summary>
    public required float Confidence { get; init; }

    /// <summary>
    /// Context where the pattern was matched.
    /// </summary>
    public required string Context { get; init; }
}