using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port for semantic pattern matching operations.
/// </summary>
public interface IPatternMatchingPort
{
    /// <summary>
    /// Matches patterns against the provided text content.
    /// </summary>
    /// <param name="content">The text content to match against.</param>
    /// <param name="patterns">The patterns to match against the content.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result containing the pattern matches or an error.</returns>
    Task<Result<IReadOnlyList<PatternMatch>>> MatchPatternsAsync(
        string content, 
        IReadOnlyList<SemanticPattern> patterns, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new semantic pattern for matching.
    /// </summary>
    /// <param name="pattern">The semantic pattern to register.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> RegisterPatternAsync(
        SemanticPattern pattern, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing semantic pattern.
    /// </summary>
    /// <param name="pattern">The updated semantic pattern.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> UpdatePatternAsync(
        SemanticPattern pattern, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a semantic pattern by its ID.
    /// </summary>
    /// <param name="patternId">The ID of the pattern to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> DeletePatternAsync(
        string patternId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all registered patterns.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result containing all patterns or an error.</returns>
    Task<Result<IReadOnlyList<SemanticPattern>>> GetAllPatternsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves patterns by category.
    /// </summary>
    /// <param name="category">The category to filter by.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result containing filtered patterns or an error.</returns>
    Task<Result<IReadOnlyList<SemanticPattern>>> GetPatternsByCategoryAsync(
        string category, 
        CancellationToken cancellationToken = default);
}