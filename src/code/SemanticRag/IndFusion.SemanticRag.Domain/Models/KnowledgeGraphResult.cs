namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Result of a knowledge graph query operation.
/// </summary>
/// <param name="Nodes">Nodes returned by the query.</param>
/// <param name="Relationships">Relationships returned by the query.</param>
/// <param name="TotalCount">Total number of results available.</param>
/// <param name="QueryTime">Time taken to execute the query.</param>
public record KnowledgeGraphResult(
    IEnumerable<GraphNode> Nodes,
    IEnumerable<GraphRelationship> Relationships,
    long TotalCount,
    TimeSpan QueryTime
);