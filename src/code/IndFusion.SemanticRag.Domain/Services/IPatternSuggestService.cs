using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;
using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Service for analyzing code patterns and suggesting improvements.
/// </summary>
public interface IPatternSuggestService
{
    /// <summary>
    /// Suggests patterns based on the provided code context.
    /// </summary>
    /// <param name="codeContext">The code context to analyze.</param>
    /// <param name="options">Options for pattern suggestion.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern suggestions.</returns>
    Task<Result<IReadOnlyList<PatternSuggestion>>> SuggestPatternsAsync(
        string codeContext, 
        PatternSuggestionOptions options, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes a specific pattern in the provided code.
    /// </summary>
    /// <param name="code">The code to analyze.</param>
    /// <param name="patternType">The type of pattern to analyze for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the pattern analysis.</returns>
    Task<Result<PatternAnalysis>> AnalyzePatternAsync(
        string code, 
        string patternType, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds pattern violations in the provided code.
    /// </summary>
    /// <param name="code">The code to analyze.</param>
    /// <param name="filePath">Optional file path for context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern violations.</returns>
    Task<Result<IReadOnlyList<PatternViolation>>> FindViolationsAsync(
        string code,
        string? filePath = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pattern definitions for a specific category.
    /// </summary>
    /// <param name="category">The pattern category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern definitions.</returns>
    Task<Result<IReadOnlyList<PatternDefinition>>> GetPatternDefinitionsAsync(
        string category,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available pattern categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern categories.</returns>
    Task<Result<IReadOnlyList<string>>> GetPatternCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a pattern definition.
    /// </summary>
    /// <param name="patternDefinition">The pattern definition to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating whether the pattern is valid.</returns>
    Task<Result> ValidatePatternDefinitionAsync(
        PatternDefinition patternDefinition,
        CancellationToken cancellationToken = default);
}
