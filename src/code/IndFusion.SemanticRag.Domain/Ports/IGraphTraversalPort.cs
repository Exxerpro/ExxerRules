using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port for knowledge graph traversal operations.
/// </summary>
public interface IGraphTraversalPort
{
    /// <summary>
    /// Adds a new node to the knowledge graph.
    /// </summary>
    /// <param name="node">The graph node to add.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> AddNodeAsync(
        GraphNode node, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new edge/relationship to the knowledge graph.
    /// </summary>
    /// <param name="edge">The graph relationship to add.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> AddEdgeAsync(
        GraphRelationship edge, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Traverses the graph from a starting node using breadth-first search.
    /// </summary>
    /// <param name="startNodeId">The ID of the starting node.</param>
    /// <param name="maxDepth">Maximum depth to traverse (optional).</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result containing the traversed nodes or an error.</returns>
    Task<Result<IReadOnlyList<GraphNode>>> TraverseBfsAsync(
        string startNodeId, 
        int? maxDepth = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Traverses the graph from a starting node using depth-first search.
    /// </summary>
    /// <param name="startNodeId">The ID of the starting node.</param>
    /// <param name="maxDepth">Maximum depth to traverse (optional).</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result containing the traversed nodes or an error.</returns>
    Task<Result<IReadOnlyList<GraphNode>>> TraverseDfsAsync(
        string startNodeId, 
        int? maxDepth = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds the shortest path between two nodes.
    /// </summary>
    /// <param name="startNodeId">The ID of the starting node.</param>
    /// <param name="endNodeId">The ID of the ending node.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result containing the path nodes or an error.</returns>
    Task<Result<IReadOnlyList<GraphNode>>> FindShortestPathAsync(
        string startNodeId, 
        string endNodeId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all nodes connected to a specific node.
    /// </summary>
    /// <param name="nodeId">The ID of the node to get connections for.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result containing connected nodes or an error.</returns>
    Task<Result<IReadOnlyList<GraphNode>>> GetConnectedNodesAsync(
        string nodeId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a node and all its connections from the graph.
    /// </summary>
    /// <param name="nodeId">The ID of the node to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> DeleteNodeAsync(
        string nodeId, 
        CancellationToken cancellationToken = default);
}