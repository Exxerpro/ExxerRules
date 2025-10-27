using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Service for managing pattern knowledge base operations.
/// </summary>
public interface IPatternKnowledgeBase
{
    /// <summary>
    /// Finds patterns similar to the given embeddings.
    /// </summary>
    /// <param name="embeddings">Code embeddings to match against.</param>
    /// <param name="context">Development context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of similar patterns.</returns>
    Task<IReadOnlyList<PatternMatch>> FindSimilarPatternsAsync(
        float[] embeddings, 
        string context, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new pattern to the knowledge base.
    /// </summary>
    /// <param name="pattern">Pattern definition.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task AddPatternAsync(
        PatternDefinition pattern, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing pattern in the knowledge base.
    /// </summary>
    /// <param name="patternId">Pattern identifier.</param>
    /// <param name="pattern">Updated pattern definition.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task UpdatePatternAsync(
        string patternId, 
        PatternDefinition pattern, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a pattern from the knowledge base.
    /// </summary>
    /// <param name="patternId">Pattern identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task RemovePatternAsync(
        string patternId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all patterns in the knowledge base.
    /// </summary>
    /// <param name="category">Optional category filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of patterns.</returns>
    Task<IReadOnlyList<PatternDefinition>> GetAllPatternsAsync(
        string? category = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific pattern by ID.
    /// </summary>
    /// <param name="patternId">Pattern identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Pattern definition or null if not found.</returns>
    Task<PatternDefinition?> GetPatternAsync(
        string patternId, 
        CancellationToken cancellationToken = default);
}

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
