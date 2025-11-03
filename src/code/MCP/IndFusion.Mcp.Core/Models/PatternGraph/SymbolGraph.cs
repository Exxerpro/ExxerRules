namespace IndFusion.Mcp.Core.Models.PatternGraph;

/// <summary>
/// Represents a complete symbol graph containing nodes, edges, and metadata for a project.
/// </summary>
/// <param name="ProjectPath">Path to the project this graph represents.</param>
/// <param name="ProjectHash">Hash identifier for the project version.</param>
/// <param name="Nodes">Collection of all nodes in the graph.</param>
/// <param name="Edges">Collection of all edges in the graph.</param>
/// <param name="CreatedAt">Timestamp when the graph was created.</param>
/// <param name="LastUpdated">Timestamp when the graph was last updated.</param>
/// <param name="Metadata">Additional metadata about the graph.</param>
public record SymbolGraph(
	string ProjectPath,
	string ProjectHash,
	IReadOnlyCollection<GraphNode> Nodes,
	IReadOnlyCollection<GraphEdge> Edges,
	DateTime CreatedAt,
	DateTime LastUpdated,
	IReadOnlyDictionary<string, object> Metadata);
