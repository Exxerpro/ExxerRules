using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;
using IndQuestResults;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// MCP tool for graph traversal operations.
/// This tool provides access to graph traversal, path finding, and statistics.
/// </summary>
[McpServerToolType]
public static class GraphTraversalTool
{
    /// <summary>
    /// Executes a graph query and returns the results.
    /// </summary>
    /// <param name="query">The graph query to execute.</param>
    /// <param name="parameters">Optional parameters for the query (JSON format).</param>
    /// <param name="progress">Optional progress reporter for operation status updates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A structured result containing graph query results.</returns>
    [McpServerTool, Description("Execute a graph query and return the results")]
    public static async Task<string> ExecuteGraphQuery(
        [Description("The graph query to execute")] string query,
        [Description("Optional parameters for the query (JSON format)")] string? parameters = null,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Starting graph query execution...");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new McpException("Query cannot be null or empty");
            }

            progress?.Report("Parsing query parameters...");

            // Parse parameters if provided
            IReadOnlyDictionary<string, object>? parameterDict = null;
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                try
                {
                    var parsedParams = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(parameters);
                    parameterDict = parsedParams;
                }
                catch (System.Text.Json.JsonException ex)
                {
                    throw new McpException($"Invalid JSON parameters: {ex.Message}");
                }
            }

            progress?.Report("Executing graph query...");

            // Get the graph query service from DI
            var serviceProvider = ServiceProviderAccessor.ServiceProvider;
            if (serviceProvider == null)
            {
                throw new McpException("Service provider is not available");
            }

            var graphQueryService = serviceProvider.GetService(typeof(IGraphQueryService)) as IGraphQueryService;
            if (graphQueryService == null)
            {
                throw new McpException("Graph query service is not available");
            }

            // Execute query
            var result = await graphQueryService.ExecuteQueryAsync(query, parameterDict, cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                throw new McpException($"Graph query failed: {result.Error}");
            }

            progress?.Report($"Query executed successfully: {result.Value.RecordCount} records, {result.Value.ExecutionTimeMs}ms");

            // Format the response
            var response = new GraphQueryResponse(
                Result: result.Value,
                Success: true,
                ErrorMessage: null
            );

            return FormatGraphQueryResponse(response);
        }
        catch (OperationCanceledException)
        {
            progress?.Report("Graph query was cancelled");
            throw new McpException("Operation was cancelled");
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            progress?.Report($"Error during graph query: {ex.Message}");
            throw new McpException($"Graph query failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves nodes of a specific type with optional filters.
    /// </summary>
    /// <param name="nodeType">The type of nodes to retrieve.</param>
    /// <param name="filters">Optional filters to apply (JSON format).</param>
    /// <param name="progress">Optional progress reporter for operation status updates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A structured result containing the matching nodes.</returns>
    [McpServerTool, Description("Retrieve nodes of a specific type with optional filters")]
    public static async Task<string> GetNodes(
        [Description("The type of nodes to retrieve")] string nodeType,
        [Description("Optional filters to apply (JSON format)")] string? filters = null,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Starting node retrieval...");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(nodeType))
            {
                throw new McpException("Node type cannot be null or empty");
            }

            progress?.Report("Parsing filters...");

            // Parse filters if provided
            IReadOnlyDictionary<string, object>? filterDict = null;
            if (!string.IsNullOrWhiteSpace(filters))
            {
                try
                {
                    var parsedFilters = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(filters);
                    filterDict = parsedFilters;
                }
                catch (System.Text.Json.JsonException ex)
                {
                    throw new McpException($"Invalid JSON filters: {ex.Message}");
                }
            }

            progress?.Report($"Retrieving nodes of type: {nodeType}");

            // Get the graph query service from DI
            var serviceProvider = ServiceProviderAccessor.ServiceProvider;
            if (serviceProvider == null)
            {
                throw new McpException("Service provider is not available");
            }

            var graphQueryService = serviceProvider.GetService(typeof(IGraphQueryService)) as IGraphQueryService;
            if (graphQueryService == null)
            {
                throw new McpException("Graph query service is not available");
            }

            // Execute node retrieval
            var result = await graphQueryService.GetNodesAsync(nodeType, filterDict, cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                throw new McpException($"Node retrieval failed: {result.Error}");
            }

            progress?.Report($"Retrieved {result.Value?.Count ?? 0} nodes of type {nodeType}");

            // Format the response
            var response = new GraphNodeResponse(
                Nodes: result.Value ?? new List<GraphNode>(),
                TotalNodes: result.Value?.Count ?? 0,
                NodeType: nodeType
            );

            return FormatGraphNodeResponse(response);
        }
        catch (OperationCanceledException)
        {
            progress?.Report("Node retrieval was cancelled");
            throw new McpException("Operation was cancelled");
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            progress?.Report($"Error during node retrieval: {ex.Message}");
            throw new McpException($"Node retrieval failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Executes a traversal query starting from a specific node.
    /// </summary>
    /// <param name="startNodeId">The ID of the starting node.</param>
    /// <param name="maxDepth">Maximum traversal depth (default: 3).</param>
    /// <param name="relationshipTypes">Comma-separated list of relationship types to follow (optional).</param>
    /// <param name="progress">Optional progress reporter for operation status updates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A structured result containing the traversal results.</returns>
    [McpServerTool, Description("Execute a traversal query starting from a specific node")]
    public static async Task<string> TraverseGraph(
        [Description("The ID of the starting node")] string startNodeId,
        [Description("Maximum traversal depth (default: 3)")] int maxDepth = 3,
        [Description("Comma-separated list of relationship types to follow (optional)")] string? relationshipTypes = null,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Starting graph traversal...");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(startNodeId))
            {
                throw new McpException("Start node ID cannot be null or empty");
            }

            if (maxDepth < 0)
            {
                throw new McpException("Max depth cannot be negative");
            }

            progress?.Report("Parsing relationship types...");

            // Parse relationship types if provided
            IReadOnlyList<string>? relationshipTypeList = null;
            if (!string.IsNullOrWhiteSpace(relationshipTypes))
            {
                relationshipTypeList = relationshipTypes.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(rt => rt.Trim())
                    .Where(rt => !string.IsNullOrWhiteSpace(rt))
                    .ToList();
            }

            progress?.Report($"Starting traversal from node {startNodeId} with max depth {maxDepth}");

            // Get the graph query service from DI
            var serviceProvider = ServiceProviderAccessor.ServiceProvider;
            if (serviceProvider == null)
            {
                throw new McpException("Service provider is not available");
            }

            var graphQueryService = serviceProvider.GetService(typeof(IGraphQueryService)) as IGraphQueryService;
            if (graphQueryService == null)
            {
                throw new McpException("Graph query service is not available");
            }

            // Execute traversal
            var result = await graphQueryService.TraverseAsync(startNodeId, maxDepth, relationshipTypeList, cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                throw new McpException($"Graph traversal failed: {result.Error}");
            }

            progress?.Report($"Traversal completed: {result.Value.TotalNodesVisited} nodes, {result.Value.TotalRelationshipsTraversed} relationships");

            // Format the response
            var response = new GraphTraversalResponse(
                TraversalResult: result.Value,
                Success: true,
                ErrorMessage: null
            );

            return FormatGraphTraversalResponse(response);
        }
        catch (OperationCanceledException)
        {
            progress?.Report("Graph traversal was cancelled");
            throw new McpException("Operation was cancelled");
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            progress?.Report($"Error during graph traversal: {ex.Message}");
            throw new McpException($"Graph traversal failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Finds the shortest path between two nodes.
    /// </summary>
    /// <param name="startNodeId">The ID of the starting node.</param>
    /// <param name="endNodeId">The ID of the ending node.</param>
    /// <param name="maxDepth">Maximum path length (default: 10).</param>
    /// <param name="progress">Optional progress reporter for operation status updates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A structured result containing the shortest path.</returns>
    [McpServerTool, Description("Find the shortest path between two nodes")]
    public static async Task<string> FindShortestPath(
        [Description("The ID of the starting node")] string startNodeId,
        [Description("The ID of the ending node")] string endNodeId,
        [Description("Maximum path length (default: 10)")] int maxDepth = 10,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Starting shortest path search...");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(startNodeId))
            {
                throw new McpException("Start node ID cannot be null or empty");
            }

            if (string.IsNullOrWhiteSpace(endNodeId))
            {
                throw new McpException("End node ID cannot be null or empty");
            }

            if (maxDepth < 0)
            {
                throw new McpException("Max depth cannot be negative");
            }

            progress?.Report($"Finding shortest path from {startNodeId} to {endNodeId}");

            // Get the graph query service from DI
            var serviceProvider = ServiceProviderAccessor.ServiceProvider;
            if (serviceProvider == null)
            {
                throw new McpException("Service provider is not available");
            }

            var graphQueryService = serviceProvider.GetService(typeof(IGraphQueryService)) as IGraphQueryService;
            if (graphQueryService == null)
            {
                throw new McpException("Graph query service is not available");
            }

            // Execute shortest path search
            var result = await graphQueryService.FindShortestPathAsync(startNodeId, endNodeId, maxDepth, cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                throw new McpException($"Shortest path search failed: {result.Error}");
            }

            if (result.Value == null)
            {
                progress?.Report("No path found between the specified nodes");
            }
            else
            {
                progress?.Report($"Found path with {result.Value?.Relationships.Count ?? 0} relationships");
            }

            // Format the response
            var response = new GraphPathResponse(
                Path: result.Value,
                Success: true,
                ErrorMessage: null
            );

            return FormatGraphPathResponse(response);
        }
        catch (OperationCanceledException)
        {
            progress?.Report("Shortest path search was cancelled");
            throw new McpException("Operation was cancelled");
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            progress?.Report($"Error during shortest path search: {ex.Message}");
            throw new McpException($"Shortest path search failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets statistics about the graph structure.
    /// </summary>
    /// <param name="progress">Optional progress reporter for operation status updates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A structured result containing graph statistics.</returns>
    [McpServerTool, Description("Get statistics about the graph structure")]
    public static async Task<string> GetGraphStatistics(
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Starting graph statistics retrieval...");

            // Get the graph query service from DI
            var serviceProvider = ServiceProviderAccessor.ServiceProvider;
            if (serviceProvider == null)
            {
                throw new McpException("Service provider is not available");
            }

            var graphQueryService = serviceProvider.GetService(typeof(IGraphQueryService)) as IGraphQueryService;
            if (graphQueryService == null)
            {
                throw new McpException("Graph query service is not available");
            }

            // Execute statistics retrieval
            var result = await graphQueryService.GetStatisticsAsync(cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                throw new McpException($"Graph statistics retrieval failed: {result.Error}");
            }

            progress?.Report($"Retrieved graph statistics: {result.Value.TotalNodes} nodes, {result.Value.TotalRelationships} relationships");

            // Format the response
            var response = new GraphStatisticsResponse(
                Statistics: result.Value,
                Success: true,
                ErrorMessage: null
            );

            return FormatGraphStatisticsResponse(response);
        }
        catch (OperationCanceledException)
        {
            progress?.Report("Graph statistics retrieval was cancelled");
            throw new McpException("Operation was cancelled");
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            progress?.Report($"Error during graph statistics retrieval: {ex.Message}");
            throw new McpException($"Graph statistics retrieval failed: {ex.Message}");
        }
    }

    private static string FormatGraphQueryResponse(GraphQueryResponse response)
    {
        var records = response.Result.Records.Select(r => new
        {
            Values = r.Values,
            Keys = r.Keys
        }).ToList();

        var result = new
        {
            Success = response.Success,
            RecordCount = response.Result.RecordCount,
            RecordsAffected = response.Result.RecordsAffected,
            ExecutionTimeMs = response.Result.ExecutionTimeMs,
            IsSuccess = response.Result.IsSuccess,
            Records = records,
            ErrorMessage = response.ErrorMessage
        };

        return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

    private static string FormatGraphNodeResponse(GraphNodeResponse response)
    {
        var nodes = response.Nodes.Select(n => new
        {
            Id = n.Id,
            Type = n.Type,
            Properties = n.Properties,
            Labels = n.Labels
        }).ToList();

        var result = new
        {
            Success = true,
            TotalNodes = response.TotalNodes,
            NodeType = response.NodeType,
            Nodes = nodes
        };

        return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

    private static string FormatGraphTraversalResponse(GraphTraversalResponse response)
    {
        var nodes = response.TraversalResult.Nodes.Select(n => new
        {
            Id = n.Id,
            Type = n.Type,
            Properties = n.Properties,
            Labels = n.Labels
        }).ToList();

        var relationships = response.TraversalResult.Relationships.Select(r => new
        {
            Id = r.Id,
            Type = r.Type,
            StartNodeId = r.StartNodeId,
            EndNodeId = r.EndNodeId,
            Properties = r.Properties
        }).ToList();

        var paths = response.TraversalResult.Paths.Select(p => new
        {
            Length = p.Length,
            Weight = p.Weight,
            NodeCount = p.Nodes.Count,
            RelationshipCount = p.Relationships.Count
        }).ToList();

        var result = new
        {
            Success = response.Success,
            TotalNodesVisited = response.TraversalResult.TotalNodesVisited,
            TotalRelationshipsTraversed = response.TraversalResult.TotalRelationshipsTraversed,
            MaxDepthReached = response.TraversalResult.MaxDepthReached,
            PathCount = response.TraversalResult.PathCount,
            HasPaths = response.TraversalResult.HasPaths,
            Nodes = nodes,
            Relationships = relationships,
            Paths = paths,
            ErrorMessage = response.ErrorMessage
        };

        return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

    private static string FormatGraphPathResponse(GraphPathResponse response)
    {
        var result = new
        {
            Success = response.Success,
            PathFound = response.Path != null,
            Path = response.Path != null ? new
            {
                Length = response.Path.Value.Length,
                Weight = response.Path.Value.Weight,
                StartNodeId = response.Path.Value.StartNode.Id,
                EndNodeId = response.Path.Value.EndNode.Id,
                NodeCount = response.Path.Value.Nodes.Count,
                RelationshipCount = response.Path.Value.Relationships.Count
            } : null,
            ErrorMessage = response.ErrorMessage
        };

        return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

    private static string FormatGraphStatisticsResponse(GraphStatisticsResponse response)
    {
        var nodeTypes = response.Statistics.NodeTypes.Select(nt => new
        {
            Type = nt.Key,
            Count = nt.Value
        }).ToList();

        var relationshipTypes = response.Statistics.RelationshipTypes.Select(rt => new
        {
            Type = rt.Key,
            Count = rt.Value
        }).ToList();

        var result = new
        {
            Success = response.Success,
            TotalNodes = response.Statistics.TotalNodes,
            TotalRelationships = response.Statistics.TotalRelationships,
            AverageDegree = response.Statistics.AverageDegree,
            MaxDegree = response.Statistics.MaxDegree,
            ConnectedComponents = response.Statistics.ConnectedComponents,
            Density = response.Statistics.Density,
            MostCommonNodeType = response.Statistics.MostCommonNodeType,
            MostCommonRelationshipType = response.Statistics.MostCommonRelationshipType,
            LastUpdated = response.Statistics.LastUpdated,
            NodeTypes = nodeTypes,
            RelationshipTypes = relationshipTypes,
            ErrorMessage = response.ErrorMessage
        };

        return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }
}

/// <summary>
/// Response model for graph query operations.
/// </summary>
/// <param name="Result">The graph query result.</param>
/// <param name="Success">Whether the query was successful.</param>
/// <param name="ErrorMessage">Error message if query failed.</param>
public readonly record struct GraphQueryResponse(
    GraphQueryResult Result,
    bool Success,
    string? ErrorMessage);

/// <summary>
/// Response model for graph node operations.
/// </summary>
/// <param name="Nodes">The graph nodes found.</param>
/// <param name="TotalNodes">Total number of nodes.</param>
/// <param name="NodeType">The node type that was searched.</param>
public readonly record struct GraphNodeResponse(
    IReadOnlyList<GraphNode> Nodes,
    int TotalNodes,
    string NodeType);

/// <summary>
/// Response model for graph traversal operations.
/// </summary>
/// <param name="TraversalResult">The graph traversal result.</param>
/// <param name="Success">Whether the traversal was successful.</param>
/// <param name="ErrorMessage">Error message if traversal failed.</param>
public readonly record struct GraphTraversalResponse(
    GraphTraversalResult TraversalResult,
    bool Success,
    string? ErrorMessage);

/// <summary>
/// Response model for graph path operations.
/// </summary>
/// <param name="Path">The graph path found.</param>
/// <param name="Success">Whether the path search was successful.</param>
/// <param name="ErrorMessage">Error message if path search failed.</param>
public readonly record struct GraphPathResponse(
    GraphPath? Path,
    bool Success,
    string? ErrorMessage);

/// <summary>
/// Response model for graph statistics operations.
/// </summary>
/// <param name="Statistics">The graph statistics.</param>
/// <param name="Success">Whether the operation was successful.</param>
/// <param name="ErrorMessage">Error message if operation failed.</param>
public readonly record struct GraphStatisticsResponse(
    GraphStatistics Statistics,
    bool Success,
    string? ErrorMessage);
