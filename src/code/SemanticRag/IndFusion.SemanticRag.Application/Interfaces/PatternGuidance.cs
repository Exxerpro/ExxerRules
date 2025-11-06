using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Represents pattern guidance for a development context.
/// </summary>
public record PatternGuidance
{
    /// <summary>
    /// Development context.
    /// </summary>
    public required string Context { get; init; }

    /// <summary>
    /// Recommended patterns for this context.
    /// </summary>
    public required IReadOnlyList<PatternDefinition> RecommendedPatterns { get; init; }

    /// <summary>
    /// Patterns to avoid in this context.
    /// </summary>
    public required IReadOnlyList<PatternDefinition> AvoidPatterns { get; init; }

    /// <summary>
    /// Best practices for this context.
    /// </summary>
    public required IReadOnlyList<string> BestPractices { get; init; }

    /// <summary>
    /// Common pitfalls to avoid.
    /// </summary>
    public required IReadOnlyList<string> CommonPitfalls { get; init; }
}