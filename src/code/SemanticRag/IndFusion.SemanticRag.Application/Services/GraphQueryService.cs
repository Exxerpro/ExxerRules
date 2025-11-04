using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Domain.Services;
using IndQuestResults;
using Microsoft.Extensions.Logging;

namespace IndFusion.SemanticRag.Application.Services;

/// <summary>
/// Service for executing graph queries and retrieving nodes and relationships.
/// </summary>
public class GraphQueryService : IGraphQueryService
{
    private readonly IKnowledgeGraphPort _knowledgeGraphPort;
    private readonly ILogger<GraphQueryService> _logger;

    /// <summary>
    /// Initializes a new instance of the GraphQueryService class.
    /// </summary>
    /// <param name="knowledgeGraphPort">The knowledge graph port for graph operations.</param>
    /// <param name="logger">The logger for this service.</param>
    public GraphQueryService(IKnowledgeGraphPort knowledgeGraphPort, ILogger<GraphQueryService> logger)
    {
        _knowledgeGraphPort = knowledgeGraphPort ?? throw new ArgumentNullException(nameof(knowledgeGraphPort));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executes a graph query and returns the results.
    /// </summary>
    /// <param name="query">The graph query to execute.</param>
    /// <param name="parameters">Optional parameters for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the query results.</returns>
    public async Task<Result<GraphQueryResult>> ExecuteQueryAsync(
        string query, 
        IReadOnlyDictionary<string, object>? parameters = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(query))
            {
                _logger.LogWarning("Query cannot be null or empty");
                return Result<GraphQueryResult>.WithFailure("Query cannot be null or empty");
            }

            _logger.LogDebug("Executing graph query: {Query}", query);

            var startTime = DateTimeOffset.UtcNow;
            var graphQuery = new GraphQuery(query, parameters);
            var validationResult = graphQuery.Validate();
            
            if (validationResult.IsFailure)
            {
                _logger.LogWarning("Query validation failed: {Error}", validationResult.Error);
                return Result<GraphQueryResult>.WithFailure(validationResult.Error!);
            }

            // Execute the query using the knowledge graph port
            var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(query, parameters, cancellationToken).ConfigureAwait(false);
            var relationshipsResult = await _knowledgeGraphPort.QueryRelationshipsAsync(query, parameters, cancellationToken).ConfigureAwait(false);

            var executionTime = DateTimeOffset.UtcNow - startTime;
            var executionTimeMs = (long)executionTime.TotalMilliseconds;

            if (nodesResult.IsFailure && relationshipsResult.IsFailure)
            {
                _logger.LogError("Both node and relationship queries failed");
                return Result<GraphQueryResult>.WithFailure("Query execution failed");
            }

            // Convert results to GraphQueryResult format
            var records = new List<GraphRecord>();
            var recordsAffected = 0;

            if (nodesResult.IsSuccess && nodesResult.Value != null)
            {
                foreach (var node in nodesResult.Value)
                {
                    var values = new List<object> { node.Id, node.Label, node.Properties };
                    var keys = new List<string> { "id", "type", "properties" };
                    records.Add(new GraphRecord(values, keys));
                    recordsAffected++;
                }
            }

            if (relationshipsResult.IsSuccess && relationshipsResult.Value != null)
            {
                foreach (var relationship in relationshipsResult.Value)
                {
                    var values = new List<object> { relationship.Id, relationship.RelationshipType, relationship.FromNodeId, relationship.ToNodeId, relationship.Properties };
                    var keys = new List<string> { "id", "type", "startNodeId", "endNodeId", "properties" };
                    records.Add(new GraphRecord(values, keys));
                    recordsAffected++;
                }
            }

            var result = new GraphQueryResult(records, executionTimeMs, recordsAffected, true);
            
            _logger.LogDebug("Query executed successfully in {ExecutionTimeMs}ms, {RecordsAffected} records affected", 
                executionTimeMs, recordsAffected);

            return Result<GraphQueryResult>.Success(result);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Query execution was cancelled");
            return Result<GraphQueryResult>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing graph query: {Query}", query);
            return Result<GraphQueryResult>.WithFailure($"Error executing query: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves nodes of a specific type with optional filters.
    /// </summary>
    /// <param name="nodeType">The type of nodes to retrieve.</param>
    /// <param name="filters">Optional filters to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the matching nodes.</returns>
    public async Task<Result<IReadOnlyList<GraphNode>>> GetNodesAsync(
        string nodeType, 
        IReadOnlyDictionary<string, object>? filters = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(nodeType))
            {
                _logger.LogWarning("Node type cannot be null or empty");
                return Result<IReadOnlyList<GraphNode>>.WithFailure("Node type cannot be null or empty");
            }

            _logger.LogDebug("Retrieving nodes of type: {NodeType}", nodeType);

            var cypherQuery = $"MATCH (n:{nodeType}) RETURN n";
            var parameters = filters ?? new Dictionary<string, object>();

            var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(cypherQuery, parameters, cancellationToken).ConfigureAwait(false);

            if (nodesResult.IsFailure)
            {
                _logger.LogError("Failed to retrieve nodes: {Error}", nodesResult.Error);
                return Result<IReadOnlyList<GraphNode>>.WithFailure(nodesResult.Error!);
            }

            var graphNodes = (nodesResult.Value ?? []).Select(knowledgeNode => new GraphNode(
                knowledgeNode.Id,
                knowledgeNode.Label,
                knowledgeNode.Properties,
                [knowledgeNode.Label]
            )).ToList();

            _logger.LogDebug("Retrieved {Count} nodes of type {NodeType}", graphNodes.Count, nodeType);

            return Result<IReadOnlyList<GraphNode>>.Success(graphNodes);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Node retrieval was cancelled");
            return Result<IReadOnlyList<GraphNode>>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving nodes of type {NodeType}", nodeType);
            return Result<IReadOnlyList<GraphNode>>.WithFailure($"Error retrieving nodes: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves relationships of a specific type with optional filters.
    /// </summary>
    /// <param name="relationshipType">The type of relationships to retrieve.</param>
    /// <param name="filters">Optional filters to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the matching relationships.</returns>
    public async Task<Result<IReadOnlyList<GraphRelationship>>> GetRelationshipsAsync(
        string relationshipType, 
        IReadOnlyDictionary<string, object>? filters = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(relationshipType))
            {
                _logger.LogWarning("Relationship type cannot be null or empty");
                return Result<IReadOnlyList<GraphRelationship>>.WithFailure("Relationship type cannot be null or empty");
            }

            _logger.LogDebug("Retrieving relationships of type: {RelationshipType}", relationshipType);

            var cypherQuery = $"MATCH ()-[r:{relationshipType}]->() RETURN r";
            var parameters = filters ?? new Dictionary<string, object>();

            var relationshipsResult = await _knowledgeGraphPort.QueryRelationshipsAsync(cypherQuery, parameters, cancellationToken).ConfigureAwait(false);

            if (relationshipsResult.IsFailure)
            {
                _logger.LogError("Failed to retrieve relationships: {Error}", relationshipsResult.Error);
                return Result<IReadOnlyList<GraphRelationship>>.WithFailure(relationshipsResult.Error!);
            }

            var graphRelationships = (relationshipsResult.Value ?? []).Select(rel => new GraphRelationship(
                rel.Id,
                rel.RelationshipType,
                rel.FromNodeId,
                rel.ToNodeId,
                rel.Properties
            )).ToList();

            _logger.LogDebug("Retrieved {Count} relationships of type {RelationshipType}", graphRelationships.Count, relationshipType);

            return Result<IReadOnlyList<GraphRelationship>>.Success(graphRelationships);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Relationship retrieval was cancelled");
            return Result<IReadOnlyList<GraphRelationship>>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving relationships of type {RelationshipType}", relationshipType);
            return Result<IReadOnlyList<GraphRelationship>>.WithFailure($"Error retrieving relationships: {ex.Message}");
        }
    }

    /// <summary>
    /// Executes a traversal query starting from a specific node.
    /// </summary>
    /// <param name="startNodeId">The ID of the starting node.</param>
    /// <param name="maxDepth">Maximum traversal depth.</param>
    /// <param name="relationshipTypes">Optional relationship types to follow.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the traversal results.</returns>
    public async Task<Result<GraphTraversalResult>> TraverseAsync(
        string startNodeId,
        int maxDepth = 3,
        IReadOnlyList<string>? relationshipTypes = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(startNodeId))
            {
                _logger.LogWarning("Start node ID cannot be null or empty");
                return Result<GraphTraversalResult>.WithFailure("Start node ID cannot be null or empty");
            }

            if (maxDepth < 0)
            {
                _logger.LogWarning("Max depth cannot be negative");
                return Result<GraphTraversalResult>.WithFailure("Max depth cannot be negative");
            }

            _logger.LogDebug("Starting traversal from node {StartNodeId} with max depth {MaxDepth}", startNodeId, maxDepth);

            var relationshipFilter = relationshipTypes != null && relationshipTypes.Count > 0
                ? string.Join("|", relationshipTypes)
                : "*";

            var cypherQuery = $"MATCH path = (start)-[r:{relationshipFilter}*1..{maxDepth}]->(end) " +
                             $"WHERE start.id = $startNodeId " +
                             $"RETURN path, length(path) as depth";

            var parameters = new Dictionary<string, object> { ["startNodeId"] = startNodeId };

            var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(cypherQuery, parameters, cancellationToken).ConfigureAwait(false);
            var relationshipsResult = await _knowledgeGraphPort.QueryRelationshipsAsync(cypherQuery, parameters, cancellationToken).ConfigureAwait(false);

            if (nodesResult.IsFailure && relationshipsResult.IsFailure)
            {
                _logger.LogError("Traversal query failed");
                return Result<GraphTraversalResult>.WithFailure("Traversal query failed");
            }

            var visitedNodes = new List<GraphNode>();
            var traversedRelationships = new List<GraphRelationship>();
            var paths = new List<GraphPath>();
            var maxDepthReached = 0;

            if (nodesResult.IsSuccess && nodesResult.Value != null)
            {
                foreach (var node in nodesResult.Value)
                {
                    visitedNodes.Add(new GraphNode(node.Id, node.Label, node.Properties, [node.Label]));
                }
            }

            if (relationshipsResult.IsSuccess && relationshipsResult.Value != null)
            {
                foreach (var relationship in relationshipsResult.Value)
                {
                    traversedRelationships.Add(new GraphRelationship(
                        relationship.Id,
                        relationship.RelationshipType,
                        relationship.FromNodeId,
                        relationship.ToNodeId,
                        relationship.Properties
                    ));
                }
            }

            var result = new GraphTraversalResult(
                visitedNodes,
                traversedRelationships,
                paths,
                maxDepthReached,
                visitedNodes.Count,
                traversedRelationships.Count
            );

            _logger.LogDebug("Traversal completed: {NodesVisited} nodes, {RelationshipsTraversed} relationships", 
                visitedNodes.Count, traversedRelationships.Count);

            return Result<GraphTraversalResult>.Success(result);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Traversal was cancelled");
            return Result<GraphTraversalResult>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during traversal from node {StartNodeId}", startNodeId);
            return Result<GraphTraversalResult>.WithFailure($"Error during traversal: {ex.Message}");
        }
    }

    /// <summary>
    /// Finds the shortest path between two nodes.
    /// </summary>
    /// <param name="startNodeId">The ID of the starting node.</param>
    /// <param name="endNodeId">The ID of the ending node.</param>
    /// <param name="maxDepth">Maximum path length.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the shortest path.</returns>
    public async Task<Result<GraphPath?>> FindShortestPathAsync(
        string startNodeId,
        string endNodeId,
        int maxDepth = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(startNodeId))
            {
                _logger.LogWarning("Start node ID cannot be null or empty");
                return Result<GraphPath?>.WithFailure("Start node ID cannot be null or empty");
            }

            if (string.IsNullOrWhiteSpace(endNodeId))
            {
                _logger.LogWarning("End node ID cannot be null or empty");
                return Result<GraphPath?>.WithFailure("End node ID cannot be null or empty");
            }

            if (maxDepth < 0)
            {
                _logger.LogWarning("Max depth cannot be negative");
                return Result<GraphPath?>.WithFailure("Max depth cannot be negative");
            }

            _logger.LogDebug("Finding shortest path from {StartNodeId} to {EndNodeId}", startNodeId, endNodeId);

            var cypherQuery = $"MATCH path = shortestPath((start)-[*1..{maxDepth}]->(end)) " +
                             $"WHERE start.id = $startNodeId AND end.id = $endNodeId " +
                             $"RETURN path";

            var parameters = new Dictionary<string, object>
            {
                ["startNodeId"] = startNodeId,
                ["endNodeId"] = endNodeId
            };

            var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(cypherQuery, parameters, cancellationToken).ConfigureAwait(false);
            var relationshipsResult = await _knowledgeGraphPort.QueryRelationshipsAsync(cypherQuery, parameters, cancellationToken).ConfigureAwait(false);

            // No path exists if:
            // 1. Both queries failed, OR
            // 2. Nodes query failed, OR
            // 3. Nodes query succeeded but returned no results (empty or null)
            if (nodesResult.IsFailure || 
                (nodesResult.IsSuccess && (nodesResult.Value == null || nodesResult.Value.Count == 0)))
            {
                _logger.LogDebug("No path found from {StartNodeId} to {EndNodeId}", startNodeId, endNodeId);
                return Result<GraphPath?>.Success(null);
            }

            if (nodesResult.IsSuccess && nodesResult.Value != null && nodesResult.Value.Count > 0)
            {
                var pathNodes = nodesResult.Value.Select(node => new GraphNode(
                    node.Id,
                    node.Label,
                    node.Properties,
                    [node.Label]
                )).ToList();

                var pathRelationships = relationshipsResult.IsSuccess && relationshipsResult.Value != null
                    ? relationshipsResult.Value.Select(rel => new GraphRelationship(
                        rel.Id,
                        rel.RelationshipType,
                        rel.FromNodeId,
                        rel.ToNodeId,
                        rel.Properties
                    )).ToList()
                    : [];

                var path = new GraphPath(pathNodes, pathRelationships, pathRelationships.Count);
                
                _logger.LogDebug("Found path with {Length} relationships", path.Length);
                return Result<GraphPath?>.Success(path);
            }

            _logger.LogDebug("No path found from {StartNodeId} to {EndNodeId}", startNodeId, endNodeId);
            return Result<GraphPath?>.Success(null);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Path finding was cancelled");
            return Result<GraphPath?>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding shortest path from {StartNodeId} to {EndNodeId}", startNodeId, endNodeId);
            return Result<GraphPath?>.WithFailure($"Error finding shortest path: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets statistics about the graph structure.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing graph statistics.</returns>
    public async Task<Result<GraphStatistics>> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug("Retrieving graph statistics");

            var nodeCountResult = await _knowledgeGraphPort.GetNodeCountAsync(cancellationToken).ConfigureAwait(false);
            var relationshipCountResult = await _knowledgeGraphPort.GetRelationshipCountAsync(cancellationToken).ConfigureAwait(false);

            if (nodeCountResult.IsFailure)
            {
                _logger.LogError("Failed to get node count: {Error}", nodeCountResult.Error);
                return Result<GraphStatistics>.WithFailure(nodeCountResult.Error!);
            }

            if (relationshipCountResult.IsFailure)
            {
                _logger.LogError("Failed to get relationship count: {Error}", relationshipCountResult.Error);
                return Result<GraphStatistics>.WithFailure(relationshipCountResult.Error!);
            }

            // Get node types distribution
            var nodeTypesQuery = "MATCH (n) RETURN labels(n) as labels, count(n) as count";
            var nodeTypesResult = await _knowledgeGraphPort.QueryNodesAsync(nodeTypesQuery, null, cancellationToken).ConfigureAwait(false);

            var nodeTypes = new Dictionary<string, int>();
            if (nodeTypesResult.IsSuccess)
            {
                // This would need to be implemented based on the actual graph structure
                nodeTypes["Unknown"] = nodeCountResult.Value;
            }

            // Get relationship types distribution
            var relationshipTypesQuery = "MATCH ()-[r]->() RETURN type(r) as type, count(r) as count";
            var relationshipTypesResult = await _knowledgeGraphPort.QueryRelationshipsAsync(relationshipTypesQuery, null, cancellationToken).ConfigureAwait(false);

            var relationshipTypes = new Dictionary<string, int>();
            if (relationshipTypesResult.IsSuccess)
            {
                // This would need to be implemented based on the actual graph structure
                relationshipTypes["Unknown"] = relationshipCountResult.Value;
            }

            var averageDegree = nodeCountResult.Value > 0 
                ? (double)relationshipCountResult.Value / nodeCountResult.Value 
                : 0.0;

            var statistics = new GraphStatistics(
                TotalNodes: nodeCountResult.Value,
                TotalRelationships: relationshipCountResult.Value,
                NodeTypes: nodeTypes,
                RelationshipTypes: relationshipTypes,
                AverageDegree: averageDegree,
                MaxDegree: 0, // Would need additional query to calculate
                ConnectedComponents: 1, // Would need additional query to calculate
                LastUpdated: DateTimeOffset.UtcNow
            );

            _logger.LogDebug("Retrieved graph statistics: {NodeCount} nodes, {RelationshipCount} relationships", 
                nodeCountResult.Value, relationshipCountResult.Value);

            return Result<GraphStatistics>.Success(statistics);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Statistics retrieval was cancelled");
            return Result<GraphStatistics>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving graph statistics");
            return Result<GraphStatistics>.WithFailure($"Error retrieving statistics: {ex.Message}");
        }
    }
}
