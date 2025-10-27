using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;
using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port for knowledge graph operations in the semantic RAG system.
/// </summary>
public interface IKnowledgeGraphPort
{
    /// <summary>
    /// Creates a new node in the knowledge graph.
    /// </summary>
    /// <param name="node">The node to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> CreateNodeAsync(KnowledgeNode node, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates multiple nodes in the knowledge graph.
    /// </summary>
    /// <param name="nodes">The nodes to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> CreateNodesAsync(IReadOnlyList<KnowledgeNode> nodes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a relationship between two nodes.
    /// </summary>
    /// <param name="relationship">The relationship to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> CreateRelationshipAsync(KnowledgeRelationship relationship, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates multiple relationships in the knowledge graph.
    /// </summary>
    /// <param name="relationships">The relationships to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> CreateRelationshipsAsync(IReadOnlyList<KnowledgeRelationship> relationships, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a node by its ID.
    /// </summary>
    /// <param name="nodeId">The node ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the node or null if not found.</returns>
    Task<Result<KnowledgeNode?>> GetNodeAsync(string nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves nodes by label.
    /// </summary>
    /// <param name="label">The node label.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the nodes.</returns>
    Task<Result<IReadOnlyList<KnowledgeNode>>> GetNodesByLabelAsync(
        string label, 
        int limit = 100, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves relationships for a specific node.
    /// </summary>
    /// <param name="nodeId">The node ID.</param>
    /// <param name="relationshipType">Optional relationship type filter.</param>
    /// <param name="direction">The direction of relationships to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the relationships.</returns>
    Task<Result<IReadOnlyList<KnowledgeRelationship>>> GetNodeRelationshipsAsync(
        string nodeId,
        string? relationshipType = null,
        RelationshipDirection direction = RelationshipDirection.Both,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for nodes using a Cypher query.
    /// </summary>
    /// <param name="cypherQuery">The Cypher query to execute.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the query results.</returns>
    Task<Result<IReadOnlyList<KnowledgeNode>>> QueryNodesAsync(
        string cypherQuery,
        IReadOnlyDictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for relationships using a Cypher query.
    /// </summary>
    /// <param name="cypherQuery">The Cypher query to execute.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the query results.</returns>
    Task<Result<IReadOnlyList<KnowledgeRelationship>>> QueryRelationshipsAsync(
        string cypherQuery,
        IReadOnlyDictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a node in the knowledge graph.
    /// </summary>
    /// <param name="node">The updated node.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> UpdateNodeAsync(KnowledgeNode node, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a node and all its relationships.
    /// </summary>
    /// <param name="nodeId">The node ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> DeleteNodeAsync(string nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a relationship by its ID.
    /// </summary>
    /// <param name="relationshipId">The relationship ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> DeleteRelationshipAsync(string relationshipId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of nodes in the graph.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the count.</returns>
    Task<Result<int>> GetNodeCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of relationships in the graph.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the count.</returns>
    Task<Result<int>> GetRelationshipCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all nodes and relationships from the graph.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> ClearAllAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents the direction of relationships in graph queries.
/// </summary>
public enum RelationshipDirection
{
    /// <summary>
    /// Outgoing relationships (from the node).
    /// </summary>
    Outgoing,

    /// <summary>
    /// Incoming relationships (to the node).
    /// </summary>
    Incoming,

    /// <summary>
    /// Both incoming and outgoing relationships.
    /// </summary>
    Both
}
