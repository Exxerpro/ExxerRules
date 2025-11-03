using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port for knowledge graph service operations in the Semantic RAG system.
/// This defines the contract for high-level knowledge graph operations.
/// </summary>
public interface IKnowledgeGraphServicePort
{
    /// <summary>
    /// Creates a new knowledge entity in the graph.
    /// </summary>
    /// <param name="entity">Entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> CreateEntityAsync(
        KnowledgeEntity entity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates multiple knowledge entities in batch.
    /// </summary>
    /// <param name="entities">Entities to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> CreateEntitiesAsync(
        IReadOnlyList<KnowledgeEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new knowledge relationship in the graph.
    /// </summary>
    /// <param name="relationship">Relationship to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> CreateRelationshipAsync(
        KnowledgeRelationship relationship,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates multiple knowledge relationships in batch.
    /// </summary>
    /// <param name="relationships">Relationships to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> CreateRelationshipsAsync(
        IReadOnlyList<KnowledgeRelationship> relationships,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a knowledge entity by ID.
    /// </summary>
    /// <param name="entityId">ID of the entity to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the entity or failure if not found.</returns>
    Task<Result<KnowledgeEntity>> GetEntityAsync(
        string entityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves multiple knowledge entities by IDs.
    /// </summary>
    /// <param name="entityIds">IDs of the entities to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the entities.</returns>
    Task<Result<IReadOnlyList<KnowledgeEntity>>> GetEntitiesAsync(
        IReadOnlyList<string> entityIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a knowledge relationship by ID.
    /// </summary>
    /// <param name="relationshipId">ID of the relationship to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the relationship or failure if not found.</returns>
    Task<Result<KnowledgeRelationship>> GetRelationshipAsync(
        string relationshipId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves multiple knowledge relationships by IDs.
    /// </summary>
    /// <param name="relationshipIds">IDs of the relationships to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the relationships.</returns>
    Task<Result<IReadOnlyList<KnowledgeRelationship>>> GetRelationshipsAsync(
        IReadOnlyList<string> relationshipIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for entities by type and properties.
    /// </summary>
    /// <param name="entityType">Type of entities to search for.</param>
    /// <param name="properties">Properties to filter by.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the matching entities.</returns>
    Task<Result<IReadOnlyList<KnowledgeEntity>>> SearchEntitiesAsync(
        string entityType,
        IReadOnlyDictionary<string, object>? properties = null,
        int limit = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for relationships by type and properties.
    /// </summary>
    /// <param name="relationshipType">Type of relationships to search for.</param>
    /// <param name="properties">Properties to filter by.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the matching relationships.</returns>
    Task<Result<IReadOnlyList<KnowledgeRelationship>>> SearchRelationshipsAsync(
        string relationshipType,
        IReadOnlyDictionary<string, object>? properties = null,
        int limit = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities connected to a specific entity.
    /// </summary>
    /// <param name="entityId">ID of the entity to find connections for.</param>
    /// <param name="relationshipTypes">Types of relationships to follow.</param>
    /// <param name="maxDepth">Maximum depth to traverse.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the connected entities.</returns>
    Task<Result<IReadOnlyList<KnowledgeEntity>>> FindConnectedEntitiesAsync(
        string entityId,
        IReadOnlyList<string>? relationshipTypes = null,
        int maxDepth = 2,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds paths between two entities.
    /// </summary>
    /// <param name="fromEntityId">ID of the source entity.</param>
    /// <param name="toEntityId">ID of the target entity.</param>
    /// <param name="maxPathLength">Maximum path length to search.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the paths between entities.</returns>
    Task<Result<IReadOnlyList<GraphPath>>> FindPathsAsync(
        string fromEntityId,
        string toEntityId,
        int maxPathLength = 5,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing knowledge entity.
    /// </summary>
    /// <param name="entity">Updated entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> UpdateEntityAsync(
        KnowledgeEntity entity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing knowledge relationship.
    /// </summary>
    /// <param name="relationship">Updated relationship.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> UpdateRelationshipAsync(
        KnowledgeRelationship relationship,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a knowledge entity by ID.
    /// </summary>
    /// <param name="entityId">ID of the entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> DeleteEntityAsync(
        string entityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a knowledge relationship by ID.
    /// </summary>
    /// <param name="relationshipId">ID of the relationship to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> DeleteRelationshipAsync(
        string relationshipId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics about the knowledge graph.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing graph statistics.</returns>
    Task<Result<KnowledgeGraphStatistics>> GetStatisticsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all entities and relationships from the graph.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> ClearAsync(
        CancellationToken cancellationToken = default);
}
