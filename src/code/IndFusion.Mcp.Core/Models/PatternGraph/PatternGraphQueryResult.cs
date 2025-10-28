namespace IndFusion.Mcp.Core.Models.PatternGraph;

/// <summary>
/// Represents the result of a pattern graph query containing nodes, edges, and metadata.
/// </summary>
/// <param name="Nodes">Collection of graph nodes found in the query.</param>
/// <param name="Edges">Collection of graph edges (relationships) found in the query.</param>
/// <param name="QueryMetadata">Additional metadata about the query execution.</param>
/// <param name="ExecutionTime">Time taken to execute the query.</param>
public record PatternGraphQueryResult(
	IReadOnlyCollection<GraphNode> Nodes,
	IReadOnlyCollection<GraphEdge> Edges,
	QueryMetadata QueryMetadata,
	TimeSpan ExecutionTime);
