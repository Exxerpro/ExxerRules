using IndQuestResults;
using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Models.PatternGraph;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace IndFusion.Mcp.Core.Services;

/// <summary>
/// Service for managing caching of symbol graphs to improve performance.
/// Handles storage, retrieval, and invalidation of cached graph data.
/// </summary>
public class GraphCacheManager : IGraphCacheManager
{
	private readonly ILogger<GraphCacheManager> _logger;
	private readonly ConcurrentDictionary<string, SymbolGraph> _cache = new();

	/// <summary>
	/// Initializes a new instance of the GraphCacheManager class.
	/// </summary>
	/// <param name="logger">Logger instance for this service.</param>
	public GraphCacheManager(ILogger<GraphCacheManager> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	/// <summary>
	/// Retrieves a cached symbol graph by project hash.
	/// </summary>
	/// <param name="projectHash">The hash identifier for the project.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result containing the cached symbol graph or null if not found.</returns>
	public Task<Result<SymbolGraph?>> GetAsync(
		string projectHash, 
		CancellationToken cancellationToken = default)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			
			_logger.LogInformation("Retrieving cached symbol graph for hash: {ProjectHash}", projectHash);
			
			if (string.IsNullOrEmpty(projectHash))
			{
				_logger.LogWarning("Project hash is null or empty");
				return Task.FromResult(Result<SymbolGraph?>.Success(null));
			}
			
			var found = _cache.TryGetValue(projectHash, out var graph);
			if (found)
			{
				_logger.LogDebug("Found cached symbol graph for hash: {ProjectHash}", projectHash);
				return Task.FromResult(Result<SymbolGraph?>.Success(graph));
			}
			
			_logger.LogDebug("No cached symbol graph found for hash: {ProjectHash}", projectHash);
			return Task.FromResult(Result<SymbolGraph?>.Success(null));
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Operation was cancelled while retrieving cached symbol graph for hash: {ProjectHash}", projectHash);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving cached symbol graph for hash: {ProjectHash}", projectHash);
			return Task.FromResult(Result<SymbolGraph?>.WithFailure($"Failed to retrieve cached symbol graph: {ex.Message}"));
		}
	}

	/// <summary>
	/// Stores a symbol graph in the cache with the specified project hash.
	/// </summary>
	/// <param name="projectHash">The hash identifier for the project.</param>
	/// <param name="graph">The symbol graph to cache.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result indicating success or failure of the cache operation.</returns>
	public Task<Result> SetAsync(
		string projectHash, 
		SymbolGraph graph, 
		CancellationToken cancellationToken = default)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			
			_logger.LogInformation("Caching symbol graph for hash: {ProjectHash}", projectHash);
			
			if (string.IsNullOrEmpty(projectHash))
			{
				_logger.LogWarning("Project hash is null or empty");
				return Task.FromResult(Result.WithFailure("Project hash cannot be null or empty"));
			}
			
			if (graph == null)
			{
				_logger.LogWarning("Symbol graph is null");
				return Task.FromResult(Result.WithFailure("Symbol graph cannot be null"));
			}
			
			_cache[projectHash] = graph;
			_logger.LogDebug("Successfully cached symbol graph for hash: {ProjectHash}", projectHash);
			
			return Task.FromResult(Result.Success());
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Operation was cancelled while caching symbol graph for hash: {ProjectHash}", projectHash);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error caching symbol graph for hash: {ProjectHash}", projectHash);
			return Task.FromResult(Result.WithFailure($"Failed to cache symbol graph: {ex.Message}"));
		}
	}

	/// <summary>
	/// Invalidates and removes a cached symbol graph by project hash.
	/// </summary>
	/// <param name="projectHash">The hash identifier for the project to invalidate.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result indicating success or failure of the invalidation operation.</returns>
	public Task<Result> InvalidateAsync(
		string projectHash, 
		CancellationToken cancellationToken = default)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			
			_logger.LogInformation("Invalidating cached symbol graph for hash: {ProjectHash}", projectHash);
			
			if (string.IsNullOrEmpty(projectHash))
			{
				_logger.LogWarning("Project hash is null or empty");
				return Task.FromResult(Result.WithFailure("Project hash cannot be null or empty"));
			}
			
			var removed = _cache.TryRemove(projectHash, out _);
			if (removed)
			{
				_logger.LogDebug("Successfully invalidated cached symbol graph for hash: {ProjectHash}", projectHash);
			}
			else
			{
				_logger.LogDebug("No cached symbol graph found to invalidate for hash: {ProjectHash}", projectHash);
			}
			
			return Task.FromResult(Result.Success());
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Operation was cancelled while invalidating cached symbol graph for hash: {ProjectHash}", projectHash);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error invalidating cached symbol graph for hash: {ProjectHash}", projectHash);
			return Task.FromResult(Result.WithFailure($"Failed to invalidate cached symbol graph: {ex.Message}"));
		}
	}
}
