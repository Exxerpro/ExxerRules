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