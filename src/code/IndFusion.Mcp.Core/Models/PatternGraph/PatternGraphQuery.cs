namespace IndFusion.Mcp.Core.Models.PatternGraph;

/// <summary>
/// Represents a query for pattern graph analysis with filters and search criteria.
/// </summary>
/// <param name="ProjectPath">The path to the project to analyze.</param>
/// <param name="NodeTypes">Optional filter for specific node types to include.</param>
/// <param name="EdgeTypes">Optional filter for specific edge types to include.</param>
/// <param name="MaxDepth">Maximum depth for graph traversal.</param>
/// <param name="IncludeMetadata">Whether to include additional metadata in results.</param>
public record PatternGraphQuery(
	string ProjectPath,
	IReadOnlyCollection<string>? NodeTypes = null,
	IReadOnlyCollection<string>? EdgeTypes = null,
	int MaxDepth = 10,
	bool IncludeMetadata = true);
