namespace IndFusion.Mcp.Core.Models.PatternGraph;

/// <summary>
/// Represents an edge (relationship) in the pattern graph between two nodes.
/// </summary>
/// <param name="Id">Unique identifier for the edge.</param>
/// <param name="SourceNodeId">Identifier of the source node.</param>
/// <param name="TargetNodeId">Identifier of the target node.</param>
/// <param name="RelationshipType">Type of relationship (e.g., Calls, References, Inherits).</param>
/// <param name="Weight">Weight or strength of the relationship.</param>
/// <param name="Metadata">Additional metadata about the edge.</param>
public record GraphEdge(
	string Id,
	string SourceNodeId,
	string TargetNodeId,
	string RelationshipType,
	double Weight = 1.0,
	IReadOnlyDictionary<string, object>? Metadata = null);
