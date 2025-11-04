using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Domain.Services;
using IndQuestResults;
using Microsoft.Extensions.Logging;

namespace IndFusion.SemanticRag.Application.Services;

/// <summary>
/// Service for querying pattern-specific graph operations.
/// </summary>
public class PatternGraphQueryService : IPatternGraphQueryService
{
    private readonly IKnowledgeGraphPort _knowledgeGraphPort;
    private readonly ILogger<PatternGraphQueryService> _logger;

    /// <summary>
    /// Initializes a new instance of the PatternGraphQueryService class.
    /// </summary>
    /// <param name="knowledgeGraphPort">The knowledge graph port for graph operations.</param>
    /// <param name="logger">The logger for this service.</param>
    public PatternGraphQueryService(IKnowledgeGraphPort knowledgeGraphPort, ILogger<PatternGraphQueryService> logger)
    {
        _knowledgeGraphPort = knowledgeGraphPort ?? throw new ArgumentNullException(nameof(knowledgeGraphPort));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Queries the pattern graph with a specific query.
    /// </summary>
    /// <param name="query">The pattern graph query to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the pattern graph results.</returns>
    public async Task<Result<PatternGraphResult>> QueryPatternGraphAsync(
        PatternGraphQuery query, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var validationResult = query.Validate();
            if (validationResult.IsFailure)
            {
                _logger.LogWarning("Pattern graph query validation failed: {Error}", validationResult.Error);
                return Result<PatternGraphResult>.WithFailure(validationResult.Error!);
            }

            _logger.LogDebug("Executing pattern graph query: {Query}", query.Query);

            var startTime = DateTimeOffset.UtcNow;

            // Execute the query using the knowledge graph port
            var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(query.Query, query.Parameters, cancellationToken).ConfigureAwait(false);
            var relationshipsResult = await _knowledgeGraphPort.QueryRelationshipsAsync(query.Query, query.Parameters, cancellationToken).ConfigureAwait(false);

            var executionTime = DateTimeOffset.UtcNow - startTime;
            var executionTimeMs = (long)executionTime.TotalMilliseconds;

            if (nodesResult.IsFailure && relationshipsResult.IsFailure)
            {
                _logger.LogError("Pattern graph query execution failed");
                return Result<PatternGraphResult>.WithFailure("Pattern graph query execution failed");
            }

            // Convert nodes to pattern definitions
            var patterns = new List<PatternDefinition>();
            if (nodesResult.IsSuccess && nodesResult.Value != null)
            {
                patterns = nodesResult.Value.Select(node => new PatternDefinition(
                    Id: node.GetProperty<string>("id") ?? node.Id,
                    Name: node.GetProperty<string>("name") ?? "Unknown",
                    Description: node.GetProperty<string>("description") ?? "",
                    Category: node.GetProperty<string>("category") ?? "Unknown",
                    Severity: Enum.TryParse<PatternSeverity>(node.GetProperty<string>("severity"), out var severity) ? severity : PatternSeverity.Info,
                    Pattern: node.GetProperty<string>("pattern") ?? "",
                    Tags: node.GetProperty<IReadOnlyList<string>>("tags") ?? [],
                    IsEnabled: node.GetProperty<bool?>("isEnabled") ?? true,
                    CreatedAt: node.GetProperty<DateTimeOffset?>("createdAt"),
                    UpdatedAt: node.GetProperty<DateTimeOffset?>("updatedAt")
                )).ToList();
            }

            // Convert relationships to pattern relationships
            var relationships = new List<PatternRelationship>();
            if (relationshipsResult.IsSuccess && relationshipsResult.Value != null)
            {
                relationships = relationshipsResult.Value.Select(rel => new PatternRelationship(
                    Id: rel.Id,
                    Type: rel.RelationshipType,
                    SourcePatternId: rel.FromNodeId,
                    TargetPatternId: rel.ToNodeId,
                    Properties: rel.Properties,
                    Strength: rel.Properties.TryGetValue("strength", out var strengthValue) && strengthValue is float strength ? strength : 1.0f
                )).ToList();
            }

            var totalResults = patterns.Count + relationships.Count;
            var hasMoreResults = totalResults >= query.MaxResults;

            var result = new PatternGraphResult(
                Patterns: patterns.Take(query.MaxResults).ToList(),
                Relationships: relationships.Take(query.MaxResults - patterns.Count).ToList(),
                ExecutionTimeMs: executionTimeMs,
                TotalResults: totalResults,
                HasMoreResults: hasMoreResults
            );

            _logger.LogDebug("Pattern graph query executed successfully in {ExecutionTimeMs}ms, {TotalResults} results", 
                executionTimeMs, totalResults);

            return Result<PatternGraphResult>.Success(result);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Pattern graph query was cancelled");
            return Result<PatternGraphResult>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing pattern graph query: {Query}", query.Query);
            return Result<PatternGraphResult>.WithFailure($"Error executing pattern graph query: {ex.Message}");
        }
    }

    /// <summary>
    /// Finds pattern relationships starting from a specific pattern.
    /// </summary>
    /// <param name="patternId">The ID of the pattern to start from.</param>
    /// <param name="maxDepth">Maximum depth to traverse.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern relationships.</returns>
    public async Task<Result<IReadOnlyList<PatternRelationship>>> FindPatternRelationshipsAsync(
        string patternId, 
        int maxDepth = 3, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(patternId))
            {
                _logger.LogWarning("Pattern ID cannot be null or empty");
                return Result<IReadOnlyList<PatternRelationship>>.WithFailure("Pattern ID cannot be null or empty");
            }

            if (maxDepth < 0)
            {
                _logger.LogWarning("Max depth cannot be negative");
                return Result<IReadOnlyList<PatternRelationship>>.WithFailure("Max depth cannot be negative");
            }

            _logger.LogDebug("Finding pattern relationships for pattern {PatternId} with max depth {MaxDepth}", patternId, maxDepth);

            var cypherQuery = $"MATCH path = (p:PatternDefinition)-[r*1..{maxDepth}]->(related:PatternDefinition) " +
                             $"WHERE p.id = $patternId " +
                             $"RETURN r, length(path) as depth";

            var parameters = new Dictionary<string, object> { ["patternId"] = patternId };

            var relationshipsResult = await _knowledgeGraphPort.QueryRelationshipsAsync(cypherQuery, parameters, cancellationToken).ConfigureAwait(false);

            if (relationshipsResult.IsFailure)
            {
                _logger.LogError("Failed to find pattern relationships: {Error}", relationshipsResult.Error);
                return Result<IReadOnlyList<PatternRelationship>>.WithFailure(relationshipsResult.Error!);
            }

            var patternRelationships = (relationshipsResult.Value ?? []).Select(rel => new PatternRelationship(
                Id: rel.Id,
                Type: rel.RelationshipType,
                SourcePatternId: rel.FromNodeId,
                TargetPatternId: rel.ToNodeId,
                Properties: rel.Properties,
                Strength: rel.Properties.TryGetValue("strength", out var strengthValue) && strengthValue is float strength ? strength : 1.0f
            )).ToList();

            _logger.LogDebug("Found {Count} pattern relationships for pattern {PatternId}", patternRelationships.Count, patternId);

            return Result<IReadOnlyList<PatternRelationship>>.Success(patternRelationships);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Pattern relationship finding was cancelled");
            return Result<IReadOnlyList<PatternRelationship>>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding pattern relationships for pattern {PatternId}", patternId);
            return Result<IReadOnlyList<PatternRelationship>>.WithFailure($"Error finding pattern relationships: {ex.Message}");
        }
    }

    /// <summary>
    /// Finds patterns that are similar to the specified pattern.
    /// </summary>
    /// <param name="patternId">The ID of the pattern to find similarities for.</param>
    /// <param name="similarityThreshold">Minimum similarity threshold.</param>
    /// <param name="maxResults">Maximum number of results to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing similar patterns.</returns>
    public async Task<Result<IReadOnlyList<PatternSimilarity>>> FindSimilarPatternsAsync(
        string patternId,
        float similarityThreshold = 0.7f,
        int maxResults = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(patternId))
            {
                _logger.LogWarning("Pattern ID cannot be null or empty");
                return Result<IReadOnlyList<PatternSimilarity>>.WithFailure("Pattern ID cannot be null or empty");
            }

            if (similarityThreshold < 0.0f || similarityThreshold > 1.0f)
            {
                _logger.LogWarning("Similarity threshold must be between 0.0 and 1.0");
                return Result<IReadOnlyList<PatternSimilarity>>.WithFailure("Similarity threshold must be between 0.0 and 1.0");
            }

            if (maxResults <= 0)
            {
                _logger.LogWarning("Max results must be greater than 0");
                return Result<IReadOnlyList<PatternSimilarity>>.WithFailure("Max results must be greater than 0");
            }

            _logger.LogDebug("Finding similar patterns for pattern {PatternId} with threshold {SimilarityThreshold}", patternId, similarityThreshold);

            // Get the source pattern
            var sourcePatternResult = await GetPatternByIdAsync(patternId, cancellationToken).ConfigureAwait(false);
            if (sourcePatternResult.IsFailure)
            {
                return Result<IReadOnlyList<PatternSimilarity>>.WithFailure(sourcePatternResult.Error!);
            }

            var sourcePattern = sourcePatternResult.Value;

            // Find similar patterns using graph queries
            var cypherQuery = "MATCH (p:PatternDefinition) WHERE p.id <> $patternId RETURN p";
            var parameters = new Dictionary<string, object> { ["patternId"] = patternId };

            var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(cypherQuery, parameters, cancellationToken).ConfigureAwait(false);

            if (nodesResult.IsFailure)
            {
                _logger.LogError("Failed to find similar patterns: {Error}", nodesResult.Error);
                return Result<IReadOnlyList<PatternSimilarity>>.WithFailure(nodesResult.Error!);
            }

            var similarities = new List<PatternSimilarity>();

            foreach (var node in nodesResult.Value ?? [])
            {
                var targetPattern = new PatternDefinition(
                    Id: node.GetProperty<string>("id") ?? node.Id,
                    Name: node.GetProperty<string>("name") ?? "Unknown",
                    Description: node.GetProperty<string>("description") ?? "",
                    Category: node.GetProperty<string>("category") ?? "Unknown",
                    Severity: Enum.TryParse<PatternSeverity>(node.GetProperty<string>("severity"), out var severity) ? severity : PatternSeverity.Info,
                    Pattern: node.GetProperty<string>("pattern") ?? "",
                    Tags: node.GetProperty<IReadOnlyList<string>>("tags") ?? [],
                    IsEnabled: node.GetProperty<bool?>("isEnabled") ?? true,
                    CreatedAt: node.GetProperty<DateTimeOffset?>("createdAt"),
                    UpdatedAt: node.GetProperty<DateTimeOffset?>("updatedAt")
                );

                var similarityScore = CalculateSimilarity(sourcePattern, targetPattern);
                if (similarityScore >= similarityThreshold)
                {
                    var commonElements = FindCommonElements(sourcePattern, targetPattern);
                    var similarityType = DetermineSimilarityType(sourcePattern, targetPattern);

                    var similarity = new PatternSimilarity(
                        PatternId: targetPattern.Id,
                        SimilarityScore: similarityScore,
                        SimilarityType: similarityType,
                        CommonElements: commonElements
                    );

                    similarities.Add(similarity);
                }
            }

            var sortedSimilarities = similarities
                .OrderByDescending(s => s.SimilarityScore)
                .Take(maxResults)
                .ToList();

            _logger.LogDebug("Found {Count} similar patterns for pattern {PatternId}", sortedSimilarities.Count, patternId);

            return Result<IReadOnlyList<PatternSimilarity>>.Success(sortedSimilarities);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Similar pattern finding was cancelled");
            return Result<IReadOnlyList<PatternSimilarity>>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding similar patterns for pattern {PatternId}", patternId);
            return Result<IReadOnlyList<PatternSimilarity>>.WithFailure($"Error finding similar patterns: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets pattern usage statistics across the codebase.
    /// </summary>
    /// <param name="patternId">The ID of the pattern to get statistics for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern usage statistics.</returns>
    public async Task<Result<PatternUsageStatistics>> GetPatternUsageStatisticsAsync(
        string patternId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(patternId))
            {
                _logger.LogWarning("Pattern ID cannot be null or empty");
                return Result<PatternUsageStatistics>.WithFailure("Pattern ID cannot be null or empty");
            }

            _logger.LogDebug("Getting usage statistics for pattern {PatternId}", patternId);

            // Query for pattern usage data
            var usageQuery = "MATCH (p:PatternDefinition)-[:USED_IN]->(c:CodeNode) " +
                            "WHERE p.id = $patternId " +
                            "RETURN count(c) as usageCount, count(DISTINCT c.filePath) as fileCount, " +
                            "count(DISTINCT c.project) as projectCount, max(c.lastUsed) as lastUsed";

            var parameters = new Dictionary<string, object> { ["patternId"] = patternId };

            var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(usageQuery, parameters, cancellationToken).ConfigureAwait(false);

            if (nodesResult.IsFailure)
            {
                _logger.LogError("Failed to get pattern usage statistics: {Error}", nodesResult.Error);
                return Result<PatternUsageStatistics>.WithFailure(nodesResult.Error!);
            }

            var usageCount = 0;
            var fileCount = 0;
            var projectCount = 0;
            DateTimeOffset? lastUsed = null;

            if (nodesResult.Value != null && nodesResult.Value.Any())
            {
                var firstNode = nodesResult.Value.First();
                usageCount = firstNode.GetProperty<int?>("usageCount") ?? 0;
                fileCount = firstNode.GetProperty<int?>("fileCount") ?? 0;
                projectCount = firstNode.GetProperty<int?>("projectCount") ?? 0;
                lastUsed = firstNode.GetProperty<DateTimeOffset?>("lastUsed");
            }

            // Determine usage trend (simplified implementation)
            var trend = DetermineUsageTrend(usageCount, lastUsed);

            var statistics = new PatternUsageStatistics(
                PatternId: patternId,
                UsageCount: usageCount,
                FileCount: fileCount,
                ProjectCount: projectCount,
                LastUsed: lastUsed,
                Trend: trend
            );

            _logger.LogDebug("Retrieved usage statistics for pattern {PatternId}: {UsageCount} usages, {FileCount} files, {ProjectCount} projects", 
                patternId, usageCount, fileCount, projectCount);

            return Result<PatternUsageStatistics>.Success(statistics);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Pattern usage statistics retrieval was cancelled");
            return Result<PatternUsageStatistics>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting usage statistics for pattern {PatternId}", patternId);
            return Result<PatternUsageStatistics>.WithFailure($"Error getting usage statistics: {ex.Message}");
        }
    }

    /// <summary>
    /// Finds patterns that violate best practices.
    /// </summary>
    /// <param name="category">Optional pattern category to filter by.</param>
    /// <param name="severity">Minimum severity level.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing anti-pattern violations.</returns>
    public async Task<Result<IReadOnlyList<AntiPatternViolation>>> FindAntiPatternsAsync(
        string? category = null,
        PatternSeverity severity = PatternSeverity.Warning,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug("Finding anti-pattern violations with category {Category} and severity {Severity}", category, severity);

            var cypherQuery = "MATCH (ap:AntiPattern)-[:VIOLATED_BY]->(c:CodeNode) " +
                             "WHERE ap.severity >= $severity " +
                             (category != null ? "AND ap.category = $category " : "") +
                             "RETURN ap, c";

            var parameters = new Dictionary<string, object> { ["severity"] = severity.ToString() };
            if (category != null)
            {
                parameters["category"] = category;
            }

            var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(cypherQuery, parameters, cancellationToken).ConfigureAwait(false);

            if (nodesResult.IsFailure)
            {
                _logger.LogError("Failed to find anti-pattern violations: {Error}", nodesResult.Error);
                return Result<IReadOnlyList<AntiPatternViolation>>.WithFailure(nodesResult.Error!);
            }

            var violations = new List<AntiPatternViolation>();

            foreach (var node in nodesResult.Value ?? [])
            {
                var antiPatternId = node.GetProperty<string>("antiPatternId") ?? node.Id;
                var antiPatternName = node.GetProperty<string>("antiPatternName") ?? "Unknown Anti-Pattern";
                var violationSeverity = Enum.TryParse<PatternSeverity>(node.GetProperty<string>("severity"), out var sev) ? sev : severity;
                var message = node.GetProperty<string>("message") ?? "Anti-pattern violation detected";
                var filePath = node.GetProperty<string>("filePath");
                var lineNumber = node.GetProperty<int?>("lineNumber");
                var codeSnippet = node.GetProperty<string>("codeSnippet");
                var suggestedFix = node.GetProperty<string>("suggestedFix");

                var violation = new AntiPatternViolation(
                    Id: Guid.NewGuid().ToString(),
                    AntiPatternId: antiPatternId,
                    AntiPatternName: antiPatternName,
                    Severity: violationSeverity,
                    Message: message,
                    FilePath: filePath,
                    LineNumber: lineNumber,
                    CodeSnippet: codeSnippet,
                    SuggestedFix: suggestedFix
                );

                violations.Add(violation);
            }

            _logger.LogDebug("Found {Count} anti-pattern violations", violations.Count);

            return Result<IReadOnlyList<AntiPatternViolation>>.Success(violations);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Anti-pattern finding was cancelled");
            return Result<IReadOnlyList<AntiPatternViolation>>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding anti-pattern violations");
            return Result<IReadOnlyList<AntiPatternViolation>>.WithFailure($"Error finding anti-pattern violations: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets pattern evolution history for a specific pattern.
    /// </summary>
    /// <param name="patternId">The ID of the pattern to get history for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern evolution data.</returns>
    public async Task<Result<IReadOnlyList<PatternEvolution>>> GetPatternEvolutionAsync(
        string patternId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(patternId))
            {
                _logger.LogWarning("Pattern ID cannot be null or empty");
                return Result<IReadOnlyList<PatternEvolution>>.WithFailure("Pattern ID cannot be null or empty");
            }

            _logger.LogDebug("Getting evolution history for pattern {PatternId}", patternId);

            var cypherQuery = "MATCH (p:PatternDefinition)-[:EVOLVED_TO]->(evolution:PatternEvolution) " +
                             "WHERE p.id = $patternId " +
                             "RETURN evolution " +
                             "ORDER BY evolution.changedAt DESC";

            var parameters = new Dictionary<string, object> { ["patternId"] = patternId };

            var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(cypherQuery, parameters, cancellationToken).ConfigureAwait(false);

            if (nodesResult.IsFailure)
            {
                _logger.LogError("Failed to get pattern evolution: {Error}", nodesResult.Error);
                return Result<IReadOnlyList<PatternEvolution>>.WithFailure(nodesResult.Error!);
            }

            var evolutionHistory = (nodesResult.Value ?? []).Select(node => new PatternEvolution(
                PatternId: patternId,
                Version: node.GetProperty<string>("version") ?? "1.0",
                ChangeType: Enum.TryParse<PatternChangeType>(node.GetProperty<string>("changeType"), out var changeType) ? changeType : PatternChangeType.Updated,
                ChangeDescription: node.GetProperty<string>("changeDescription") ?? "Pattern updated",
                ChangedAt: node.GetProperty<DateTimeOffset?>("changedAt") ?? DateTimeOffset.UtcNow,
                ChangedBy: node.GetProperty<string>("changedBy")
            )).ToList();

            _logger.LogDebug("Retrieved {Count} evolution entries for pattern {PatternId}", evolutionHistory.Count, patternId);

            return Result<IReadOnlyList<PatternEvolution>>.Success(evolutionHistory);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Pattern evolution retrieval was cancelled");
            return Result<IReadOnlyList<PatternEvolution>>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting evolution history for pattern {PatternId}", patternId);
            return Result<IReadOnlyList<PatternEvolution>>.WithFailure($"Error getting evolution history: {ex.Message}");
        }
    }

    private async Task<Result<PatternDefinition>> GetPatternByIdAsync(string patternId, CancellationToken cancellationToken)
    {
        var cypherQuery = "MATCH (p:PatternDefinition) WHERE p.id = $patternId RETURN p";
        var parameters = new Dictionary<string, object> { ["patternId"] = patternId };

        var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(cypherQuery, parameters, cancellationToken).ConfigureAwait(false);

        if (nodesResult.IsFailure)
        {
            return Result<PatternDefinition>.WithFailure(nodesResult.Error!);
        }

        if (nodesResult.Value == null || !nodesResult.Value.Any())
        {
            return Result<PatternDefinition>.WithFailure($"Pattern with ID {patternId} not found");
        }

        var node = nodesResult.Value.First();
        var pattern = new PatternDefinition(
            Id: node.GetProperty<string>("id") ?? node.Id,
            Name: node.GetProperty<string>("name") ?? "Unknown",
            Description: node.GetProperty<string>("description") ?? "",
            Category: node.GetProperty<string>("category") ?? "Unknown",
            Severity: Enum.TryParse<PatternSeverity>(node.GetProperty<string>("severity"), out var severity) ? severity : PatternSeverity.Info,
            Pattern: node.GetProperty<string>("pattern") ?? "",
            Tags: node.GetProperty<IReadOnlyList<string>>("tags") ?? [],
            IsEnabled: node.GetProperty<bool?>("isEnabled") ?? true,
            CreatedAt: node.GetProperty<DateTimeOffset?>("createdAt"),
            UpdatedAt: node.GetProperty<DateTimeOffset?>("updatedAt")
        );

        return Result<PatternDefinition>.Success(pattern);
    }

    private float CalculateSimilarity(PatternDefinition source, PatternDefinition target)
    {
        var similarity = 0.0f;

        // Name similarity
        if (source.Name.Equals(target.Name, StringComparison.OrdinalIgnoreCase))
            similarity += 0.3f;
        else if (source.Name.Contains(target.Name, StringComparison.OrdinalIgnoreCase) || 
                 target.Name.Contains(source.Name, StringComparison.OrdinalIgnoreCase))
            similarity += 0.2f;

        // Category similarity
        if (source.Category.Equals(target.Category, StringComparison.OrdinalIgnoreCase))
            similarity += 0.3f;

        // Tag similarity
        var commonTags = source.Tags.Intersect(target.Tags, StringComparer.OrdinalIgnoreCase).Count();
        var totalTags = source.Tags.Count + target.Tags.Count;
        if (totalTags > 0)
            similarity += (float)commonTags / totalTags * 0.2f;

        // Description similarity (simplified)
        if (source.Description.Equals(target.Description, StringComparison.OrdinalIgnoreCase))
            similarity += 0.2f;

        return Math.Min(similarity, 1.0f);
    }

    private List<string> FindCommonElements(PatternDefinition source, PatternDefinition target)
    {
        var commonElements = new List<string>();

        if (source.Category.Equals(target.Category, StringComparison.OrdinalIgnoreCase))
            commonElements.Add($"Category: {source.Category}");

        var commonTags = source.Tags.Intersect(target.Tags, StringComparer.OrdinalIgnoreCase);
        commonElements.AddRange(commonTags.Select(tag => $"Tag: {tag}"));

        return commonElements;
    }

    private string DetermineSimilarityType(PatternDefinition source, PatternDefinition target)
    {
        if (source.Category.Equals(target.Category, StringComparison.OrdinalIgnoreCase))
            return "semantic";

        var commonTags = source.Tags.Intersect(target.Tags, StringComparer.OrdinalIgnoreCase);
        if (commonTags.Any())
            return "structural";

        return "conceptual";
    }

    private UsageTrend DetermineUsageTrend(int usageCount, DateTimeOffset? lastUsed)
    {
        if (usageCount == 0)
            return UsageTrend.Unknown;

        if (lastUsed.HasValue)
        {
            var daysSinceLastUsed = (DateTimeOffset.UtcNow - lastUsed.Value).Days;
            if (daysSinceLastUsed < 7)
                return UsageTrend.Increasing;
            if (daysSinceLastUsed < 30)
                return UsageTrend.Stable;
            return UsageTrend.Decreasing;
        }

        return UsageTrend.Unknown;
    }
}
