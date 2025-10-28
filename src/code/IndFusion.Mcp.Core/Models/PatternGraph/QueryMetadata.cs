namespace IndFusion.Mcp.Core.Models.PatternGraph;

/// <summary>
/// Contains metadata about a pattern graph query execution.
/// </summary>
/// <param name="QueryId">Unique identifier for the query.</param>
/// <param name="Timestamp">When the query was executed.</param>
/// <param name="NodeCount">Number of nodes returned.</param>
/// <param name="EdgeCount">Number of edges returned.</param>
/// <param name="FiltersApplied">Filters that were applied to the query.</param>
public record QueryMetadata(
	string QueryId,
	DateTime Timestamp,
	int NodeCount,
	int EdgeCount,
	IReadOnlyDictionary<string, object> FiltersApplied);
