using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Service for knowledge graph operations using Neo4j.
/// </summary>
public interface IKnowledgeGraphService
{
    /// <summary>
    /// Executes a Cypher query against the knowledge graph.
    /// </summary>
    /// <param name="query">Graph query to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Query results.</returns>
    Task<GraphQueryResult> QueryAsync(
        GraphQuery query, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new node to the knowledge graph.
    /// </summary>
    /// <param name="node">Node to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Added node.</returns>
    Task<GraphNode> AddNodeAsync(
        GraphNode node, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing node in the knowledge graph.
    /// </summary>
    /// <param name="nodeId">Node identifier.</param>
    /// <param name="node">Updated node data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated node.</returns>
    Task<GraphNode> UpdateNodeAsync(
        string nodeId, 
        GraphNode node, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a node from the knowledge graph.
    /// </summary>
    /// <param name="nodeId">Node identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task DeleteNodeAsync(
        string nodeId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a relationship between two nodes.
    /// </summary>
    /// <param name="relationship">Relationship to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created relationship.</returns>
    Task<GraphRelationship> CreateRelationshipAsync(
        GraphRelationship relationship, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a relationship from the knowledge graph.
    /// </summary>
    /// <param name="relationshipId">Relationship identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task DeleteRelationshipAsync(
        string relationshipId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets context information for a given query.
    /// </summary>
    /// <param name="query">Query to get context for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Context information.</returns>
    Task<GraphQueryResult> GetContextAsync(
        string query, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a code node to the knowledge graph.
    /// </summary>
    /// <param name="codeNode">Code node to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Added code node.</returns>
    Task<CodeNode> AddCodeNodeAsync(
        CodeNode codeNode, 
        CancellationToken cancellationToken = default);
}
