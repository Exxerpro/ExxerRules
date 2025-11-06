using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Represents the result of pattern enforcement.
/// </summary>
public record EnforcementResult
{
    /// <summary>
    /// Whether enforcement was successful.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Number of violations found.
    /// </summary>
    public required int ViolationsFound { get; init; }

    /// <summary>
    /// Number of violations fixed.
    /// </summary>
    public required int ViolationsFixed { get; init; }

    /// <summary>
    /// List of remaining violations.
    /// </summary>
    public required IReadOnlyList<PatternViolation> RemainingViolations { get; init; }

    /// <summary>
    /// Time taken for enforcement in milliseconds.
    /// </summary>
    public required long ElapsedMilliseconds { get; init; }
}