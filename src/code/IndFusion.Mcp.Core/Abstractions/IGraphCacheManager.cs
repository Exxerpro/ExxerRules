using IndQuestResults;
using IndFusion.Mcp.Core.Models.PatternGraph;

namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Service for managing caching of symbol graphs to improve performance.
/// Handles storage, retrieval, and invalidation of cached graph data.
/// </summary>
public interface IGraphCacheManager
{
	/// <summary>
	/// Retrieves a cached symbol graph by project hash.
	/// </summary>
	/// <param name="projectHash">The hash identifier for the project.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result containing the cached symbol graph or null if not found.</returns>
	Task<Result<SymbolGraph?>> GetAsync(
		string projectHash, 
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Stores a symbol graph in the cache with the specified project hash.
	/// </summary>
	/// <param name="projectHash">The hash identifier for the project.</param>
	/// <param name="graph">The symbol graph to cache.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result indicating success or failure of the cache operation.</returns>
	Task<Result> SetAsync(
		string projectHash, 
		SymbolGraph graph, 
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Invalidates and removes a cached symbol graph by project hash.
	/// </summary>
	/// <param name="projectHash">The hash identifier for the project to invalidate.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result indicating success or failure of the invalidation operation.</returns>
	Task<Result> InvalidateAsync(
		string projectHash, 
		CancellationToken cancellationToken = default);
}
