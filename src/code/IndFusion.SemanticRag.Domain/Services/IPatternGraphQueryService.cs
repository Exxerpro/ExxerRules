using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;
using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Service for querying pattern-specific graph operations.
/// </summary>
public interface IPatternGraphQueryService
{
    /// <summary>
    /// Queries the pattern graph with a specific query.
    /// </summary>
    /// <param name="query">The pattern graph query to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the pattern graph results.</returns>
    Task<Result<PatternGraphResult>> QueryPatternGraphAsync(
        PatternGraphQuery query, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds pattern relationships starting from a specific pattern.
    /// </summary>
    /// <param name="patternId">The ID of the pattern to start from.</param>
    /// <param name="maxDepth">Maximum depth to traverse.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern relationships.</returns>
    Task<Result<IReadOnlyList<PatternRelationship>>> FindPatternRelationshipsAsync(
        string patternId, 
        int maxDepth = 3, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds patterns that are similar to the specified pattern.
    /// </summary>
    /// <param name="patternId">The ID of the pattern to find similarities for.</param>
    /// <param name="similarityThreshold">Minimum similarity threshold.</param>
    /// <param name="maxResults">Maximum number of results to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing similar patterns.</returns>
    Task<Result<IReadOnlyList<PatternSimilarity>>> FindSimilarPatternsAsync(
        string patternId,
        float similarityThreshold = 0.7f,
        int maxResults = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pattern usage statistics across the codebase.
    /// </summary>
    /// <param name="patternId">The ID of the pattern to get statistics for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern usage statistics.</returns>
    Task<Result<PatternUsageStatistics>> GetPatternUsageStatisticsAsync(
        string patternId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds patterns that violate best practices.
    /// </summary>
    /// <param name="category">Optional pattern category to filter by.</param>
    /// <param name="severity">Minimum severity level.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing anti-pattern violations.</returns>
    Task<Result<IReadOnlyList<AntiPatternViolation>>> FindAntiPatternsAsync(
        string? category = null,
        PatternSeverity severity = PatternSeverity.Warning,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pattern evolution history for a specific pattern.
    /// </summary>
    /// <param name="patternId">The ID of the pattern to get history for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern evolution data.</returns>
    Task<Result<IReadOnlyList<PatternEvolution>>> GetPatternEvolutionAsync(
        string patternId,
        CancellationToken cancellationToken = default);
}
