using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;
using IndQuestResults;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// MCP tool for querying pattern-specific graph operations.
/// This tool provides access to pattern graph queries, relationships, and analytics.
/// </summary>
[McpServerToolType]
public static class PatternGraphQueryTool
{
    /// <summary>
    /// Queries the pattern graph with a specific query.
    /// </summary>
    /// <param name="query">The pattern graph query to execute.</param>
    /// <param name="parameters">Optional parameters for the query (JSON format).</param>
    /// <param name="maxResults">Maximum number of results to return (default: 100).</param>
    /// <param name="timeoutMs">Query timeout in milliseconds (default: 30000).</param>
    /// <param name="progress">Optional progress reporter for operation status updates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A structured result containing pattern graph query results.</returns>
    [McpServerTool, Description("Query the pattern graph with a specific query")]
    public static async Task<string> QueryPatternGraph(
        [Description("The pattern graph query to execute")] string query,
        [Description("Optional parameters for the query (JSON format)")] string? parameters = null,
        [Description("Maximum number of results to return (default: 100)")] int maxResults = 100,
        [Description("Query timeout in milliseconds (default: 30000)")] int timeoutMs = 30000,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Starting pattern graph query...");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new McpException("Query cannot be null or empty");
            }

            if (maxResults <= 0)
            {
                throw new McpException("Max results must be greater than 0");
            }

            if (timeoutMs <= 0)
            {
                throw new McpException("Timeout must be greater than 0");
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

            progress?.Report("Executing pattern graph query...");

            // Get the pattern graph query service from DI
            var serviceProvider = ServiceProviderAccessor.ServiceProvider;
            if (serviceProvider == null)
            {
                throw new McpException("Service provider is not available");
            }

            var patternGraphQueryService = serviceProvider.GetService(typeof(IPatternGraphQueryService)) as IPatternGraphQueryService;
            if (patternGraphQueryService == null)
            {
                throw new McpException("Pattern graph query service is not available");
            }

            // Create pattern graph query
            var patternGraphQuery = new PatternGraphQuery(
                Query: query,
                Parameters: parameterDict,
                MaxResults: maxResults,
                TimeoutMs: timeoutMs
            );

            // Execute query
            var result = await patternGraphQueryService.QueryPatternGraphAsync(patternGraphQuery, cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                throw new McpException($"Pattern graph query failed: {result.Error}");
            }

            progress?.Report($"Query executed successfully: {result.Value.PatternCount} patterns, {result.Value.RelationshipCount} relationships");

            // Format the response
            var response = new PatternGraphQueryResponse(
                Result: result.Value,
                Success: true,
                ErrorMessage: null
            );

            return FormatPatternGraphQueryResponse(response);
        }
        catch (OperationCanceledException)
        {
            progress?.Report("Pattern graph query was cancelled");
            throw new McpException("Operation was cancelled");
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            progress?.Report($"Error during pattern graph query: {ex.Message}");
            throw new McpException($"Pattern graph query failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Finds pattern relationships starting from a specific pattern.
    /// </summary>
    /// <param name="patternId">The ID of the pattern to start from.</param>
    /// <param name="maxDepth">Maximum depth to traverse (default: 3).</param>
    /// <param name="progress">Optional progress reporter for operation status updates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A structured result containing pattern relationships.</returns>
    [McpServerTool, Description("Find pattern relationships starting from a specific pattern")]
    public static async Task<string> FindPatternRelationships(
        [Description("The ID of the pattern to start from")] string patternId,
        [Description("Maximum depth to traverse (default: 3)")] int maxDepth = 3,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Starting pattern relationship search...");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(patternId))
            {
                throw new McpException("Pattern ID cannot be null or empty");
            }

            if (maxDepth < 0)
            {
                throw new McpException("Max depth cannot be negative");
            }

            progress?.Report($"Finding relationships for pattern {patternId} with max depth {maxDepth}");

            // Get the pattern graph query service from DI
            var serviceProvider = ServiceProviderAccessor.ServiceProvider;
            if (serviceProvider == null)
            {
                throw new McpException("Service provider is not available");
            }

            var patternGraphQueryService = serviceProvider.GetService(typeof(IPatternGraphQueryService)) as IPatternGraphQueryService;
            if (patternGraphQueryService == null)
            {
                throw new McpException("Pattern graph query service is not available");
            }

            // Execute relationship search
            var result = await patternGraphQueryService.FindPatternRelationshipsAsync(patternId, maxDepth, cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                throw new McpException($"Pattern relationship search failed: {result.Error}");
            }

            progress?.Report($"Found {result.Value?.Count ?? 0} pattern relationships");

            // Format the response
            var response = new PatternRelationshipResponse(
                Relationships: result.Value ?? [],
                TotalRelationships: result.Value?.Count ?? 0,
                PatternId: patternId,
                MaxDepth: maxDepth
            );

            return FormatPatternRelationshipResponse(response);
        }
        catch (OperationCanceledException)
        {
            progress?.Report("Pattern relationship search was cancelled");
            throw new McpException("Operation was cancelled");
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            progress?.Report($"Error during pattern relationship search: {ex.Message}");
            throw new McpException($"Pattern relationship search failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Finds patterns that are similar to the specified pattern.
    /// </summary>
    /// <param name="patternId">The ID of the pattern to find similarities for.</param>
    /// <param name="similarityThreshold">Minimum similarity threshold (0.0 to 1.0, default: 0.7).</param>
    /// <param name="maxResults">Maximum number of results to return (default: 10).</param>
    /// <param name="progress">Optional progress reporter for operation status updates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A structured result containing similar patterns with similarity scores.</returns>
    [McpServerTool, Description("Find patterns that are similar to the specified pattern")]
    public static async Task<string> FindSimilarPatterns(
        [Description("The ID of the pattern to find similarities for")] string patternId,
        [Description("Minimum similarity threshold (0.0 to 1.0, default: 0.7)")] float similarityThreshold = 0.7f,
        [Description("Maximum number of results to return (default: 10)")] int maxResults = 10,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Starting similar pattern search...");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(patternId))
            {
                throw new McpException("Pattern ID cannot be null or empty");
            }

            if (similarityThreshold < 0.0f || similarityThreshold > 1.0f)
            {
                throw new McpException("Similarity threshold must be between 0.0 and 1.0");
            }

            if (maxResults <= 0)
            {
                throw new McpException("Max results must be greater than 0");
            }

            progress?.Report($"Finding patterns similar to {patternId} with threshold {similarityThreshold}");

            // Get the pattern graph query service from DI
            var serviceProvider = ServiceProviderAccessor.ServiceProvider;
            if (serviceProvider == null)
            {
                throw new McpException("Service provider is not available");
            }

            var patternGraphQueryService = serviceProvider.GetService(typeof(IPatternGraphQueryService)) as IPatternGraphQueryService;
            if (patternGraphQueryService == null)
            {
                throw new McpException("Pattern graph query service is not available");
            }

            // Execute similarity search
            var result = await patternGraphQueryService.FindSimilarPatternsAsync(patternId, similarityThreshold, maxResults, cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                throw new McpException($"Similar pattern search failed: {result.Error}");
            }

            progress?.Report($"Found {result.Value?.Count ?? 0} similar patterns");

            // Format the response
            var response = new PatternSimilarityResponse(
                Similarities: result.Value ?? [],
                TotalSimilarities: result.Value?.Count ?? 0,
                PatternId: patternId,
                SimilarityThreshold: similarityThreshold
            );

            return FormatPatternSimilarityResponse(response);
        }
        catch (OperationCanceledException)
        {
            progress?.Report("Similar pattern search was cancelled");
            throw new McpException("Operation was cancelled");
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            progress?.Report($"Error during similar pattern search: {ex.Message}");
            throw new McpException($"Similar pattern search failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets pattern usage statistics across the codebase.
    /// </summary>
    /// <param name="patternId">The ID of the pattern to get statistics for.</param>
    /// <param name="progress">Optional progress reporter for operation status updates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A structured result containing pattern usage statistics.</returns>
    [McpServerTool, Description("Get pattern usage statistics across the codebase")]
    public static async Task<string> GetPatternUsageStatistics(
        [Description("The ID of the pattern to get statistics for")] string patternId,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Starting pattern usage statistics retrieval...");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(patternId))
            {
                throw new McpException("Pattern ID cannot be null or empty");
            }

            progress?.Report($"Getting usage statistics for pattern {patternId}");

            // Get the pattern graph query service from DI
            var serviceProvider = ServiceProviderAccessor.ServiceProvider;
            if (serviceProvider == null)
            {
                throw new McpException("Service provider is not available");
            }

            var patternGraphQueryService = serviceProvider.GetService(typeof(IPatternGraphQueryService)) as IPatternGraphQueryService;
            if (patternGraphQueryService == null)
            {
                throw new McpException("Pattern graph query service is not available");
            }

            // Execute statistics retrieval
            var result = await patternGraphQueryService.GetPatternUsageStatisticsAsync(patternId, cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                throw new McpException($"Pattern usage statistics retrieval failed: {result.Error}");
            }

            progress?.Report($"Retrieved usage statistics: {result.Value.UsageCount} usages, {result.Value.FileCount} files, {result.Value.ProjectCount} projects");

            // Format the response
            var response = new PatternUsageStatisticsResponse(
                Statistics: result.Value,
                Success: true,
                ErrorMessage: null
            );

            return FormatPatternUsageStatisticsResponse(response);
        }
        catch (OperationCanceledException)
        {
            progress?.Report("Pattern usage statistics retrieval was cancelled");
            throw new McpException("Operation was cancelled");
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            progress?.Report($"Error during pattern usage statistics retrieval: {ex.Message}");
            throw new McpException($"Pattern usage statistics retrieval failed: {ex.Message}");
        }
    }

    private static string FormatPatternGraphQueryResponse(PatternGraphQueryResponse response)
    {
        var patterns = response.Result.Patterns.Select(p => new
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Category = p.Category,
            Severity = p.Severity.ToString(),
            Pattern = p.Pattern,
            Tags = p.Tags,
            IsEnabled = p.IsEnabled
        }).ToList();

        var relationships = response.Result.Relationships.Select(r => new
        {
            Id = r.Id,
            Type = r.Type,
            SourcePatternId = r.SourcePatternId,
            TargetPatternId = r.TargetPatternId,
            Strength = r.Strength
        }).ToList();

        var result = new
        {
            Success = response.Success,
            PatternCount = response.Result.PatternCount,
            RelationshipCount = response.Result.RelationshipCount,
            TotalResults = response.Result.TotalResults,
            HasMoreResults = response.Result.HasMoreResults,
            ExecutionTimeMs = response.Result.ExecutionTimeMs,
            Patterns = patterns,
            Relationships = relationships,
            ErrorMessage = response.ErrorMessage
        };

        return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

    private static string FormatPatternRelationshipResponse(PatternRelationshipResponse response)
    {
        var relationships = response.Relationships.Select(r => new
        {
            Id = r.Id,
            Type = r.Type,
            SourcePatternId = r.SourcePatternId,
            TargetPatternId = r.TargetPatternId,
            Strength = r.Strength
        }).ToList();

        var result = new
        {
            Success = true,
            TotalRelationships = response.TotalRelationships,
            PatternId = response.PatternId,
            MaxDepth = response.MaxDepth,
            Relationships = relationships
        };

        return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

    private static string FormatPatternSimilarityResponse(PatternSimilarityResponse response)
    {
        var similarities = response.Similarities.Select(s => new
        {
            PatternId = s.PatternId,
            SimilarityScore = s.SimilarityScore,
            SimilarityType = s.SimilarityType,
            CommonElements = s.CommonElements
        }).ToList();

        var result = new
        {
            Success = true,
            TotalSimilarities = response.TotalSimilarities,
            PatternId = response.PatternId,
            SimilarityThreshold = response.SimilarityThreshold,
            Similarities = similarities
        };

        return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

    private static string FormatPatternUsageStatisticsResponse(PatternUsageStatisticsResponse response)
    {
        var result = new
        {
            Success = response.Success,
            PatternId = response.Statistics.PatternId,
            UsageCount = response.Statistics.UsageCount,
            FileCount = response.Statistics.FileCount,
            ProjectCount = response.Statistics.ProjectCount,
            LastUsed = response.Statistics.LastUsed,
            Trend = response.Statistics.Trend.ToString(),
            AverageUsagePerFile = response.Statistics.AverageUsagePerFile,
            AverageUsagePerProject = response.Statistics.AverageUsagePerProject,
            ErrorMessage = response.ErrorMessage
        };

        return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }
}