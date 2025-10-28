using IndQuestResults;
using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Models.PatternGraph;
using Microsoft.Extensions.Logging;

namespace IndFusion.Mcp.Core.Services;

/// <summary>
/// Service for querying and managing pattern graphs in codebases.
/// Provides functionality to analyze code patterns, relationships, and structural information.
/// </summary>
public class PatternGraphService : IPatternGraphService
{
	private readonly ILogger<PatternGraphService> _logger;
	private readonly ISymbolGraphBuilder _symbolGraphBuilder;
	private readonly IGraphCacheManager _cacheManager;

	/// <summary>
	/// Initializes a new instance of the PatternGraphService class.
	/// </summary>
	/// <param name="logger">Logger instance for this service.</param>
	/// <param name="symbolGraphBuilder">Service for building symbol graphs.</param>
	/// <param name="cacheManager">Service for managing graph caching.</param>
	public PatternGraphService(
		ILogger<PatternGraphService> logger,
		ISymbolGraphBuilder symbolGraphBuilder,
		IGraphCacheManager cacheManager)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_symbolGraphBuilder = symbolGraphBuilder ?? throw new ArgumentNullException(nameof(symbolGraphBuilder));
		_cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
	}

	/// <summary>
	/// Queries the pattern graph for a specific project with given filters and criteria.
	/// </summary>
	/// <param name="query">The pattern graph query containing filters and search criteria.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result containing the pattern graph query results or failure information.</returns>
	public async Task<Result<PatternGraphQueryResult>> QueryAsync(
		PatternGraphQuery query, 
		CancellationToken cancellationToken = default)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			
			if (query == null)
			{
				_logger.LogWarning("Pattern graph query is null");
				return Result<PatternGraphQueryResult>.WithFailure("Pattern graph query cannot be null");
			}

			_logger.LogInformation("Querying pattern graph for project: {ProjectPath}", query.ProjectPath);

			// Get or build the symbol graph
			var symbolGraphResult = await GetOrBuildSymbolGraphAsync(query.ProjectPath, cancellationToken);
			if (symbolGraphResult.IsFailure)
			{
				return Result<PatternGraphQueryResult>.WithFailure($"Failed to get symbol graph: {symbolGraphResult.Error}");
			}

			var symbolGraph = symbolGraphResult.Value!;
			
			// Apply filters to nodes and edges
			var filteredNodes = ApplyNodeFilters(symbolGraph.Nodes, query.NodeTypes);
			var filteredEdges = ApplyEdgeFilters(symbolGraph.Edges, query.EdgeTypes);
			
			// Create query metadata
			var queryMetadata = new QueryMetadata(
				QueryId: Guid.NewGuid().ToString(),
				Timestamp: DateTime.UtcNow,
				NodeCount: filteredNodes.Count,
				EdgeCount: filteredEdges.Count,
				FiltersApplied: new Dictionary<string, object>
				{
					["NodeTypes"] = query.NodeTypes ?? new string[0],
					["EdgeTypes"] = query.EdgeTypes ?? new string[0],
					["MaxDepth"] = query.MaxDepth,
					["IncludeMetadata"] = query.IncludeMetadata
				});

			// Create query result
			var queryResult = new PatternGraphQueryResult(
				Nodes: filteredNodes,
				Edges: filteredEdges,
				QueryMetadata: queryMetadata,
				ExecutionTime: TimeSpan.Zero // Will be set by the caller
			);

			_logger.LogInformation("Query completed with {NodeCount} nodes and {EdgeCount} edges for project: {ProjectPath}", 
				filteredNodes.Count, filteredEdges.Count, query.ProjectPath);

			return Result<PatternGraphQueryResult>.Success(queryResult);
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Pattern graph query was cancelled for project: {ProjectPath}", query?.ProjectPath);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error querying pattern graph for project: {ProjectPath}", query?.ProjectPath);
			return Result<PatternGraphQueryResult>.WithFailure($"Error querying pattern graph: {ex.Message}");
		}
	}

	/// <summary>
	/// Retrieves all graph nodes for a specific project path.
	/// </summary>
	/// <param name="projectPath">The path to the project to analyze.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result containing the collection of graph nodes or failure information.</returns>
	public async Task<Result<IReadOnlyCollection<GraphNode>>> GetNodesAsync(
		string projectPath, 
		CancellationToken cancellationToken = default)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			
			if (string.IsNullOrWhiteSpace(projectPath))
			{
				_logger.LogWarning("Project path is null or empty");
				return Result<IReadOnlyCollection<GraphNode>>.WithFailure("Project path cannot be null or empty");
			}

			_logger.LogInformation("Retrieving graph nodes for project: {ProjectPath}", projectPath);

			// Get or build the symbol graph
			var symbolGraphResult = await GetOrBuildSymbolGraphAsync(projectPath, cancellationToken);
			if (symbolGraphResult.IsFailure)
			{
				return Result<IReadOnlyCollection<GraphNode>>.WithFailure($"Failed to get symbol graph: {symbolGraphResult.Error}");
			}

			var nodes = symbolGraphResult.Value!.Nodes;
			
			_logger.LogInformation("Retrieved {NodeCount} graph nodes for project: {ProjectPath}", nodes.Count, projectPath);
			return Result<IReadOnlyCollection<GraphNode>>.Success(nodes);
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Get nodes operation was cancelled for project: {ProjectPath}", projectPath);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving graph nodes for project: {ProjectPath}", projectPath);
			return Result<IReadOnlyCollection<GraphNode>>.WithFailure($"Error retrieving graph nodes: {ex.Message}");
		}
	}

	/// <summary>
	/// Retrieves all graph edges (relationships) for a specific project path.
	/// </summary>
	/// <param name="projectPath">The path to the project to analyze.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result containing the collection of graph edges or failure information.</returns>
	public async Task<Result<IReadOnlyCollection<GraphEdge>>> GetEdgesAsync(
		string projectPath, 
		CancellationToken cancellationToken = default)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			
			if (string.IsNullOrWhiteSpace(projectPath))
			{
				_logger.LogWarning("Project path is null or empty");
				return Result<IReadOnlyCollection<GraphEdge>>.WithFailure("Project path cannot be null or empty");
			}

			_logger.LogInformation("Retrieving graph edges for project: {ProjectPath}", projectPath);

			// Get or build the symbol graph
			var symbolGraphResult = await GetOrBuildSymbolGraphAsync(projectPath, cancellationToken);
			if (symbolGraphResult.IsFailure)
			{
				return Result<IReadOnlyCollection<GraphEdge>>.WithFailure($"Failed to get symbol graph: {symbolGraphResult.Error}");
			}

			var edges = symbolGraphResult.Value!.Edges;
			
			_logger.LogInformation("Retrieved {EdgeCount} graph edges for project: {ProjectPath}", edges.Count, projectPath);
			return Result<IReadOnlyCollection<GraphEdge>>.Success(edges);
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Get edges operation was cancelled for project: {ProjectPath}", projectPath);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving graph edges for project: {ProjectPath}", projectPath);
			return Result<IReadOnlyCollection<GraphEdge>>.WithFailure($"Error retrieving graph edges: {ex.Message}");
		}
	}

	/// <summary>
	/// Gets or builds a symbol graph for the specified project path.
	/// </summary>
	/// <param name="projectPath">The path to the project.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result containing the symbol graph or failure information.</returns>
	private async Task<Result<SymbolGraph>> GetOrBuildSymbolGraphAsync(string projectPath, CancellationToken cancellationToken)
	{
		// Generate project hash for caching
		var projectHash = GenerateProjectHash(projectPath);
		
		// Try to get from cache first
		var cachedResult = await _cacheManager.GetAsync(projectHash, cancellationToken);
		if (cachedResult.IsSuccess && cachedResult.Value != null)
		{
			_logger.LogDebug("Retrieved symbol graph from cache for project: {ProjectPath}", projectPath);
			return Result<SymbolGraph>.Success(cachedResult.Value);
		}

		// Build new symbol graph
		_logger.LogInformation("Building new symbol graph for project: {ProjectPath}", projectPath);
		var buildResult = await _symbolGraphBuilder.BuildAsync(projectPath, cancellationToken);
		if (buildResult.IsFailure)
		{
			return buildResult;
		}

		// Cache the new symbol graph
		var cacheResult = await _cacheManager.SetAsync(projectHash, buildResult.Value!, cancellationToken);
		if (cacheResult.IsFailure)
		{
			_logger.LogWarning("Failed to cache symbol graph for project: {ProjectPath}, Error: {Error}", 
				projectPath, cacheResult.Error);
		}

		return buildResult;
	}

	/// <summary>
	/// Applies node type filters to a collection of graph nodes.
	/// </summary>
	/// <param name="nodes">The collection of nodes to filter.</param>
	/// <param name="nodeTypes">The node types to include (null means include all).</param>
	/// <returns>A filtered collection of nodes.</returns>
	private IReadOnlyCollection<GraphNode> ApplyNodeFilters(IReadOnlyCollection<GraphNode> nodes, IReadOnlyCollection<string>? nodeTypes)
	{
		if (nodeTypes == null || nodeTypes.Count == 0)
		{
			return nodes;
		}

		return nodes.Where(node => nodeTypes.Contains(node.Type, StringComparer.OrdinalIgnoreCase)).ToList();
	}

	/// <summary>
	/// Applies edge type filters to a collection of graph edges.
	/// </summary>
	/// <param name="edges">The collection of edges to filter.</param>
	/// <param name="edgeTypes">The edge types to include (null means include all).</param>
	/// <returns>A filtered collection of edges.</returns>
	private IReadOnlyCollection<GraphEdge> ApplyEdgeFilters(IReadOnlyCollection<GraphEdge> edges, IReadOnlyCollection<string>? edgeTypes)
	{
		if (edgeTypes == null || edgeTypes.Count == 0)
		{
			return edges;
		}

		return edges.Where(edge => edgeTypes.Contains(edge.RelationshipType, StringComparer.OrdinalIgnoreCase)).ToList();
	}

	/// <summary>
	/// Generates a simple hash for the project path.
	/// </summary>
	/// <param name="projectPath">The project path.</param>
	/// <returns>A hash string.</returns>
	private string GenerateProjectHash(string projectPath)
	{
		using var sha256 = System.Security.Cryptography.SHA256.Create();
		var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(projectPath));
		return Convert.ToBase64String(hashBytes)[..16]; // Use first 16 characters
	}
}
