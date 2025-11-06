using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;
using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Core service for semantic RAG operations that abstracts infrastructure details.
/// </summary>
public interface ISemanticRagService
{
    /// <summary>
    /// Performs semantic search across documents and knowledge graph.
    /// </summary>
    /// <param name="query">The semantic search query.</param>
    /// <param name="config">Configuration for the search operation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the search results.</returns>
    Task<Result<IReadOnlyList<SemanticSearchResult>>> SearchAsync(
        SemanticSearchQuery query,
        SemanticRagConfig config,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves semantic context for a given query.
    /// </summary>
    /// <param name="query">The query to get context for.</param>
    /// <param name="config">Configuration for the context retrieval.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the semantic context.</returns>
    Task<Result<SemanticContext>> GetContextAsync(
        string query,
        SemanticRagConfig config,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Indexes a document for semantic search.
    /// </summary>
    /// <param name="document">The document to index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> IndexDocumentAsync(
        SemanticDocument document,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Indexes multiple documents in batch.
    /// </summary>
    /// <param name="documents">The documents to index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> IndexDocumentsAsync(
        IReadOnlyList<SemanticDocument> documents,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a knowledge entity to the semantic graph.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> AddEntityAsync(
        KnowledgeEntity entity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a relationship between knowledge entities.
    /// </summary>
    /// <param name="relationship">The relationship to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> CreateRelationshipAsync(
        KnowledgeRelationship relationship,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds similar entities based on semantic similarity.
    /// </summary>
    /// <param name="entity">The entity to find similarities for.</param>
    /// <param name="limit">Maximum number of similar entities to return.</param>
    /// <param name="threshold">Similarity threshold.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing similar entities.</returns>
    Task<Result<IReadOnlyList<KnowledgeEntity>>> FindSimilarEntitiesAsync(
        KnowledgeEntity entity,
        int limit = 5,
        float threshold = 0.7f,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics about the semantic RAG system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing system statistics.</returns>
    Task<Result<SemanticRagStats>> GetStatsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all indexed data (documents and entities).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> ClearAllAsync(CancellationToken cancellationToken = default);
}