using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Service for semantic pattern analysis and enforcement.
/// </summary>
public interface ISemanticPatternEngine
{
    /// <summary>
    /// Analyzes code for semantic pattern violations.
    /// </summary>
    /// <param name="code">Code to analyze.</param>
    /// <param name="context">Development context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of pattern violations.</returns>
    Task<IReadOnlyList<PatternViolation>> AnalyzeCodeAsync(
        string code, 
        string context, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes a project for semantic pattern violations.
    /// </summary>
    /// <param name="projectPath">Path to the project.</param>
    /// <param name="patternTypes">Types of patterns to analyze for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of pattern violations.</returns>
    Task<IReadOnlyList<PatternViolation>> AnalyzeProjectAsync(
        string projectPath, 
        string[]? patternTypes = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Suggests pattern alternatives for the given violation.
    /// </summary>
    /// <param name="violation">The pattern violation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of pattern suggestions.</returns>
    Task<IReadOnlyList<PatternSuggestion>> SuggestAlternativesAsync(
        PatternViolation violation, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes code consistency across a project.
    /// </summary>
    /// <param name="projectPath">Path to the project.</param>
    /// <param name="patternFamily">Pattern family to analyze.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Consistency report.</returns>
    Task<ConsistencyReport> AnalyzeConsistencyAsync(
        string projectPath, 
        string patternFamily = "all", 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Enforces architectural patterns in a project.
    /// </summary>
    /// <param name="projectPath">Path to the project.</param>
    /// <param name="patternTypes">Types of patterns to enforce.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Enforcement result.</returns>
    Task<EnforcementResult> EnforcePatternsAsync(
        string projectPath, 
        string[] patternTypes, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pattern guidance for a development context.
    /// </summary>
    /// <param name="context">Development context.</param>
    /// <param name="patternTypes">Types of patterns to get guidance for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Pattern guidance.</returns>
    Task<PatternGuidance> GetPatternGuidanceAsync(
        string context, 
        string[]? patternTypes = null, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a consistency report for a project.
/// </summary>
public record ConsistencyReport
{
    /// <summary>
    /// Overall consistency score (0-1).
    /// </summary>
    public required float ConsistencyScore { get; init; }

    /// <summary>
    /// List of inconsistencies found.
    /// </summary>
    public required IReadOnlyList<Inconsistency> Inconsistencies { get; init; }

    /// <summary>
    /// Pattern family that was analyzed.
    /// </summary>
    public required string PatternFamily { get; init; }

    /// <summary>
    /// Number of files analyzed.
    /// </summary>
    public required int FilesAnalyzed { get; init; }

    /// <summary>
    /// Time taken for analysis in milliseconds.
    /// </summary>
    public required long ElapsedMilliseconds { get; init; }
}

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
