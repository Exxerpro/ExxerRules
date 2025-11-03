using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;
using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Service for executing graph queries and retrieving nodes and relationships.
/// </summary>
public interface IGraphQueryService
{
    /// <summary>
    /// Executes a graph query and returns the results.
    /// </summary>
    /// <param name="query">The graph query to execute.</param>
    /// <param name="parameters">Optional parameters for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the query results.</returns>
    Task<Result<GraphQueryResult>> ExecuteQueryAsync(
        string query, 
        IReadOnlyDictionary<string, object>? parameters = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves nodes of a specific type with optional filters.
    /// </summary>
    /// <param name="nodeType">The type of nodes to retrieve.</param>
    /// <param name="filters">Optional filters to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the matching nodes.</returns>
    Task<Result<IReadOnlyList<GraphNode>>> GetNodesAsync(
        string nodeType, 
        IReadOnlyDictionary<string, object>? filters = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves relationships of a specific type with optional filters.
    /// </summary>
    /// <param name="relationshipType">The type of relationships to retrieve.</param>
    /// <param name="filters">Optional filters to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the matching relationships.</returns>
    Task<Result<IReadOnlyList<GraphRelationship>>> GetRelationshipsAsync(
        string relationshipType, 
        IReadOnlyDictionary<string, object>? filters = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a traversal query starting from a specific node.
    /// </summary>
    /// <param name="startNodeId">The ID of the starting node.</param>
    /// <param name="maxDepth">Maximum traversal depth.</param>
    /// <param name="relationshipTypes">Optional relationship types to follow.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the traversal results.</returns>
    Task<Result<GraphTraversalResult>> TraverseAsync(
        string startNodeId,
        int maxDepth = 3,
        IReadOnlyList<string>? relationshipTypes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds the shortest path between two nodes.
    /// </summary>
    /// <param name="startNodeId">The ID of the starting node.</param>
    /// <param name="endNodeId">The ID of the ending node.</param>
    /// <param name="maxDepth">Maximum path length.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the shortest path.</returns>
    Task<Result<GraphPath?>> FindShortestPathAsync(
        string startNodeId,
        string endNodeId,
        int maxDepth = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics about the graph structure.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing graph statistics.</returns>
    Task<Result<GraphStatistics>> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
