using IndQuestResults;
using IndFusion.Mcp.Core.Models.PatternGraph;

namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Service for querying and managing pattern graphs in codebases.
/// Provides functionality to analyze code patterns, relationships, and structural information.
/// </summary>
public interface IPatternGraphService
{
	/// <summary>
	/// Queries the pattern graph for a specific project with given filters and criteria.
	/// </summary>
	/// <param name="query">The pattern graph query containing filters and search criteria.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result containing the pattern graph query results or failure information.</returns>
	Task<Result<PatternGraphQueryResult>> QueryAsync(
		PatternGraphQuery query, 
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Retrieves all graph nodes for a specific project path.
	/// </summary>
	/// <param name="projectPath">The path to the project to analyze.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result containing the collection of graph nodes or failure information.</returns>
	Task<Result<IReadOnlyCollection<GraphNode>>> GetNodesAsync(
		string projectPath, 
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Retrieves all graph edges (relationships) for a specific project path.
	/// </summary>
	/// <param name="projectPath">The path to the project to analyze.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result containing the collection of graph edges or failure information.</returns>
	Task<Result<IReadOnlyCollection<GraphEdge>>> GetEdgesAsync(
		string projectPath, 
		CancellationToken cancellationToken = default);
}
