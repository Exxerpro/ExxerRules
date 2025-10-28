namespace IndFusion.Mcp.Core.Models.PatternGraph;

/// <summary>
/// Represents a node in the pattern graph, typically a code symbol or structural element.
/// </summary>
/// <param name="Id">Unique identifier for the node.</param>
/// <param name="Type">Type of the node (e.g., Class, Method, Property).</param>
/// <param name="Name">Name of the symbol or element.</param>
/// <param name="FullName">Fully qualified name of the symbol.</param>
/// <param name="Location">Source location information.</param>
/// <param name="Metadata">Additional metadata about the node.</param>
public record GraphNode(
	string Id,
	string Type,
	string Name,
	string FullName,
	SourceLocation Location,
	IReadOnlyDictionary<string, object> Metadata);
