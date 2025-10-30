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
/// Service for analyzing code patterns and suggesting improvements.
/// </summary>
public class PatternSuggestService : IPatternSuggestService
{
    private readonly IKnowledgeGraphPort _knowledgeGraphPort;
    private readonly ILogger<PatternSuggestService> _logger;

    /// <summary>
    /// Initializes a new instance of the PatternSuggestService class.
    /// </summary>
    /// <param name="knowledgeGraphPort">The knowledge graph port for graph operations.</param>
    /// <param name="logger">The logger for this service.</param>
    public PatternSuggestService(IKnowledgeGraphPort knowledgeGraphPort, ILogger<PatternSuggestService> logger)
    {
        _knowledgeGraphPort = knowledgeGraphPort ?? throw new ArgumentNullException(nameof(knowledgeGraphPort));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Suggests patterns based on the provided code context.
    /// </summary>
    /// <param name="codeContext">The code context to analyze.</param>
    /// <param name="options">Options for pattern suggestion.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern suggestions.</returns>
    public async Task<Result<IReadOnlyList<PatternSuggestion>>> SuggestPatternsAsync(
        string codeContext, 
        PatternSuggestionOptions options, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(codeContext))
            {
                _logger.LogWarning("Code context cannot be null or empty");
                return Result<IReadOnlyList<PatternSuggestion>>.WithFailure("Code context cannot be null or empty");
            }

            var validationResult = options.Validate();
            if (validationResult.IsFailure)
            {
                _logger.LogWarning("Pattern suggestion options validation failed: {Error}", validationResult.Error);
                return Result<IReadOnlyList<PatternSuggestion>>.WithFailure(validationResult.Error!);
            }

            _logger.LogDebug("Analyzing code context for pattern suggestions");

            // Analyze the code context for patterns
            var analysisResult = await AnalyzeCodeContextAsync(codeContext, options, cancellationToken).ConfigureAwait(false);
            if (analysisResult.IsFailure)
            {
                return Result<IReadOnlyList<PatternSuggestion>>.WithFailure(analysisResult.Error!);
            }

            var suggestions = (analysisResult.Value ?? new List<PatternSuggestion>())
                .Where(s => s.Confidence >= options.MinConfidence)
                .OrderByDescending(s => s.Confidence)
                .Take(options.MaxSuggestions)
                .ToList();

            _logger.LogDebug("Generated {Count} pattern suggestions", suggestions.Count);

            return Result<IReadOnlyList<PatternSuggestion>>.Success(suggestions);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Pattern suggestion was cancelled");
            return Result<IReadOnlyList<PatternSuggestion>>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error suggesting patterns for code context");
            return Result<IReadOnlyList<PatternSuggestion>>.WithFailure($"Error suggesting patterns: {ex.Message}");
        }
    }

    /// <summary>
    /// Analyzes a specific pattern in the provided code.
    /// </summary>
    /// <param name="code">The code to analyze.</param>
    /// <param name="patternType">The type of pattern to analyze for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the pattern analysis.</returns>
    public async Task<Result<PatternAnalysis>> AnalyzePatternAsync(
        string code, 
        string patternType, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogWarning("Code cannot be null or empty");
                return Result<PatternAnalysis>.WithFailure("Code cannot be null or empty");
            }

            if (string.IsNullOrWhiteSpace(patternType))
            {
                _logger.LogWarning("Pattern type cannot be null or empty");
                return Result<PatternAnalysis>.WithFailure("Pattern type cannot be null or empty");
            }

            _logger.LogDebug("Analyzing code for pattern type: {PatternType}", patternType);

            var startTime = DateTimeOffset.UtcNow;

            // Get pattern definitions for the specified type
            var patternDefinitionsResult = await GetPatternDefinitionsByTypeAsync(patternType, cancellationToken).ConfigureAwait(false);
            if (patternDefinitionsResult.IsFailure)
            {
                return Result<PatternAnalysis>.WithFailure(patternDefinitionsResult.Error!);
            }

            var patternDefinitions = patternDefinitionsResult.Value ?? new List<PatternDefinition>();
            if (!patternDefinitions.Any())
            {
                _logger.LogWarning("No pattern definitions found for type: {PatternType}", patternType);
                return Result<PatternAnalysis>.WithFailure($"No pattern definitions found for type: {patternType}");
            }

            var matches = new List<PatternMatch>();
            var violations = new List<PatternViolation>();
            var suggestions = new List<PatternSuggestion>();

            // Analyze each pattern definition
            foreach (var patternDef in patternDefinitions)
            {
                var patternMatches = AnalyzePatternMatches(code, patternDef);
                matches.AddRange(patternMatches);

                if (!patternMatches.Any())
                {
                    // Generate suggestions for missing patterns
                    var suggestion = GeneratePatternSuggestion(code, patternDef);
                    if (suggestion != null)
                    {
                        suggestions.Add(suggestion.Value);
                    }
                }
                else
                {
                    // Check for violations
                    var patternViolations = AnalyzePatternViolations(code, patternDef, patternMatches);
                    violations.AddRange(patternViolations);
                }
            }

            var analysisTime = DateTimeOffset.UtcNow - startTime;
            var confidence = CalculateAnalysisConfidence(matches, violations, suggestions);

            var analysis = new PatternAnalysis(
                PatternType: patternType,
                Matches: matches,
                Violations: violations,
                Suggestions: suggestions,
                Confidence: confidence,
                AnalysisTimeMs: (long)analysisTime.TotalMilliseconds
            );

            _logger.LogDebug("Pattern analysis completed: {MatchCount} matches, {ViolationCount} violations, {SuggestionCount} suggestions", 
                matches.Count, violations.Count, suggestions.Count);

            return Result<PatternAnalysis>.Success(analysis);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Pattern analysis was cancelled");
            return Result<PatternAnalysis>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing pattern {PatternType}", patternType);
            return Result<PatternAnalysis>.WithFailure($"Error analyzing pattern: {ex.Message}");
        }
    }

    /// <summary>
    /// Finds pattern violations in the provided code.
    /// </summary>
    /// <param name="code">The code to analyze.</param>
    /// <param name="filePath">Optional file path for context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern violations.</returns>
    public async Task<Result<IReadOnlyList<PatternViolation>>> FindViolationsAsync(
        string code,
        string? filePath = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogWarning("Code cannot be null or empty");
                return Result<IReadOnlyList<PatternViolation>>.WithFailure("Code cannot be null or empty");
            }

            _logger.LogDebug("Finding pattern violations in code");

            var violations = new List<PatternViolation>();

            // Get all pattern definitions
            var allPatternsResult = await GetAllPatternDefinitionsAsync(cancellationToken).ConfigureAwait(false);
            if (allPatternsResult.IsFailure)
            {
                return Result<IReadOnlyList<PatternViolation>>.WithFailure(allPatternsResult.Error!);
            }

            var patternDefinitions = allPatternsResult.Value ?? new List<PatternDefinition>();

            // Analyze each pattern for violations
            foreach (var patternDef in patternDefinitions)
            {
                var patternMatches = AnalyzePatternMatches(code, patternDef);
                var patternViolations = AnalyzePatternViolations(code, patternDef, patternMatches);
                violations.AddRange(patternViolations);
            }

            _logger.LogDebug("Found {Count} pattern violations", violations.Count);

            return Result<IReadOnlyList<PatternViolation>>.Success(violations);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Violation finding was cancelled");
            return Result<IReadOnlyList<PatternViolation>>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding pattern violations");
            return Result<IReadOnlyList<PatternViolation>>.WithFailure($"Error finding violations: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets pattern definitions for a specific category.
    /// </summary>
    /// <param name="category">The pattern category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern definitions.</returns>
    public async Task<Result<IReadOnlyList<PatternDefinition>>> GetPatternDefinitionsAsync(
        string category,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(category))
            {
                _logger.LogWarning("Category cannot be null or empty");
                return Result<IReadOnlyList<PatternDefinition>>.WithFailure("Category cannot be null or empty");
            }

            _logger.LogDebug("Retrieving pattern definitions for category: {Category}", category);

            var cypherQuery = "MATCH (p:PatternDefinition) WHERE p.category = $category RETURN p";
            var parameters = new Dictionary<string, object> { ["category"] = category };

            var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(cypherQuery, parameters, cancellationToken).ConfigureAwait(false);

            if (nodesResult.IsFailure)
            {
                _logger.LogError("Failed to retrieve pattern definitions: {Error}", nodesResult.Error);
                return Result<IReadOnlyList<PatternDefinition>>.WithFailure(nodesResult.Error!);
            }

            var patternDefinitions = (nodesResult.Value ?? new List<KnowledgeNode>()).Select(node => new PatternDefinition(
                Id: node.GetProperty<string>("id") ?? node.Id,
                Name: node.GetProperty<string>("name") ?? "Unknown",
                Description: node.GetProperty<string>("description") ?? "",
                Category: node.GetProperty<string>("category") ?? category,
                Severity: Enum.TryParse<PatternSeverity>(node.GetProperty<string>("severity"), out var severity) ? severity : PatternSeverity.Info,
                Pattern: node.GetProperty<string>("pattern") ?? "",
                Tags: node.GetProperty<IReadOnlyList<string>>("tags") ?? new List<string>(),
                IsEnabled: node.GetProperty<bool?>("isEnabled") ?? true,
                CreatedAt: node.GetProperty<DateTimeOffset?>("createdAt"),
                UpdatedAt: node.GetProperty<DateTimeOffset?>("updatedAt")
            )).ToList();

            _logger.LogDebug("Retrieved {Count} pattern definitions for category {Category}", patternDefinitions.Count, category);

            return Result<IReadOnlyList<PatternDefinition>>.Success(patternDefinitions);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Pattern definition retrieval was cancelled");
            return Result<IReadOnlyList<PatternDefinition>>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pattern definitions for category {Category}", category);
            return Result<IReadOnlyList<PatternDefinition>>.WithFailure($"Error retrieving pattern definitions: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets all available pattern categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing pattern categories.</returns>
    public async Task<Result<IReadOnlyList<string>>> GetPatternCategoriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug("Retrieving pattern categories");

            var cypherQuery = "MATCH (p:PatternDefinition) RETURN DISTINCT p.category as category";
            var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(cypherQuery, null, cancellationToken).ConfigureAwait(false);

            if (nodesResult.IsFailure)
            {
                _logger.LogError("Failed to retrieve pattern categories: {Error}", nodesResult.Error);
                return Result<IReadOnlyList<string>>.WithFailure(nodesResult.Error!);
            }

            var categories = (nodesResult.Value ?? new List<KnowledgeNode>())
                .Select(node => node.GetProperty<string>("category"))
                .Where(category => !string.IsNullOrWhiteSpace(category))
                .Distinct()
                .OrderBy(c => c)
                .Cast<string>()
                .ToList();

            _logger.LogDebug("Retrieved {Count} pattern categories", categories.Count);

            return Result<IReadOnlyList<string>>.Success(categories);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Pattern category retrieval was cancelled");
            return Result<IReadOnlyList<string>>.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pattern categories");
            return Result<IReadOnlyList<string>>.WithFailure($"Error retrieving pattern categories: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates a pattern definition.
    /// </summary>
    /// <param name="patternDefinition">The pattern definition to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating whether the pattern is valid.</returns>
    public async Task<Result> ValidatePatternDefinitionAsync(
        PatternDefinition patternDefinition,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug("Validating pattern definition: {PatternId}", patternDefinition.Id);

            var validationResult = patternDefinition.Validate();
            if (validationResult.IsFailure)
            {
                _logger.LogWarning("Pattern definition validation failed: {Error}", validationResult.Error);
                return validationResult;
            }

            // Additional validation logic could be added here
            // For example, checking if the pattern regex is valid, etc.

            _logger.LogDebug("Pattern definition validation successful");

            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Pattern definition validation was cancelled");
            return Result.WithFailure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating pattern definition {PatternId}", patternDefinition.Id);
            return Result.WithFailure($"Error validating pattern definition: {ex.Message}");
        }
    }

    private async Task<Result<IReadOnlyList<PatternSuggestion>>> AnalyzeCodeContextAsync(
        string codeContext, 
        PatternSuggestionOptions options, 
        CancellationToken cancellationToken)
    {
        var suggestions = new List<PatternSuggestion>();

        // Get all pattern definitions
        var allPatternsResult = await GetAllPatternDefinitionsAsync(cancellationToken).ConfigureAwait(false);
        if (allPatternsResult.IsFailure)
        {
            return Result<IReadOnlyList<PatternSuggestion>>.WithFailure(allPatternsResult.Error!);
        }

        var patternDefinitions = allPatternsResult.Value ?? new List<PatternDefinition>();

        // Filter by categories if specified
        if (options.Categories != null && options.Categories.Count > 0)
        {
            patternDefinitions = patternDefinitions.Where(p => options.Categories.Contains(p.Category)).ToList();
        }

        // Analyze each pattern
        foreach (var patternDef in patternDefinitions)
        {
            var matches = AnalyzePatternMatches(codeContext, patternDef);
            if (!matches.Any())
            {
                var suggestion = GeneratePatternSuggestion(codeContext, patternDef);
                if (suggestion != null)
                {
                    suggestions.Add(suggestion.Value);
                }
            }
        }

        return Result<IReadOnlyList<PatternSuggestion>>.Success(suggestions);
    }

    private async Task<Result<IReadOnlyList<PatternDefinition>>> GetPatternDefinitionsByTypeAsync(
        string patternType, 
        CancellationToken cancellationToken)
    {
        var cypherQuery = "MATCH (p:PatternDefinition) WHERE p.name CONTAINS $patternType OR p.category = $patternType RETURN p";
        var parameters = new Dictionary<string, object> { ["patternType"] = patternType };

        var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(cypherQuery, parameters, cancellationToken).ConfigureAwait(false);

        if (nodesResult.IsFailure)
        {
            return Result<IReadOnlyList<PatternDefinition>>.WithFailure(nodesResult.Error!);
        }

        var patternDefinitions = (nodesResult.Value ?? new List<KnowledgeNode>()).Select(node => new PatternDefinition(
            Id: node.GetProperty<string>("id") ?? node.Id,
            Name: node.GetProperty<string>("name") ?? "Unknown",
            Description: node.GetProperty<string>("description") ?? "",
            Category: node.GetProperty<string>("category") ?? "Unknown",
            Severity: Enum.TryParse<PatternSeverity>(node.GetProperty<string>("severity"), out var severity) ? severity : PatternSeverity.Info,
            Pattern: node.GetProperty<string>("pattern") ?? "",
            Tags: node.GetProperty<IReadOnlyList<string>>("tags") ?? new List<string>(),
            IsEnabled: node.GetProperty<bool?>("isEnabled") ?? true,
            CreatedAt: node.GetProperty<DateTimeOffset?>("createdAt"),
            UpdatedAt: node.GetProperty<DateTimeOffset?>("updatedAt")
        )).ToList();

        return Result<IReadOnlyList<PatternDefinition>>.Success(patternDefinitions);
    }

    private async Task<Result<IReadOnlyList<PatternDefinition>>> GetAllPatternDefinitionsAsync(CancellationToken cancellationToken)
    {
        var cypherQuery = "MATCH (p:PatternDefinition) RETURN p";
        var nodesResult = await _knowledgeGraphPort.QueryNodesAsync(cypherQuery, null, cancellationToken).ConfigureAwait(false);

        if (nodesResult.IsFailure)
        {
            return Result<IReadOnlyList<PatternDefinition>>.WithFailure(nodesResult.Error!);
        }

        var patternDefinitions = (nodesResult.Value ?? new List<KnowledgeNode>()).Select(node => new PatternDefinition(
            Id: node.GetProperty<string>("id") ?? node.Id,
            Name: node.GetProperty<string>("name") ?? "Unknown",
            Description: node.GetProperty<string>("description") ?? "",
            Category: node.GetProperty<string>("category") ?? "Unknown",
            Severity: Enum.TryParse<PatternSeverity>(node.GetProperty<string>("severity"), out var severity) ? severity : PatternSeverity.Info,
            Pattern: node.GetProperty<string>("pattern") ?? "",
            Tags: node.GetProperty<IReadOnlyList<string>>("tags") ?? new List<string>(),
            IsEnabled: node.GetProperty<bool?>("isEnabled") ?? true,
            CreatedAt: node.GetProperty<DateTimeOffset?>("createdAt"),
            UpdatedAt: node.GetProperty<DateTimeOffset?>("updatedAt")
        )).ToList();

        return Result<IReadOnlyList<PatternDefinition>>.Success(patternDefinitions);
    }

    private List<PatternMatch> AnalyzePatternMatches(string code, PatternDefinition patternDef)
    {
        var matches = new List<PatternMatch>();

        try
        {
            // Simple pattern matching implementation
            // In a real implementation, this would use more sophisticated pattern matching
            if (code.Contains(patternDef.Name, StringComparison.OrdinalIgnoreCase))
            {
                var semanticPattern = new SemanticPattern(
                    patternDef.Id,
                    patternDef.Name,
                    patternDef.Description,
                    patternDef.Pattern,
                    patternDef.Category,
                    patternDef.Severity == PatternSeverity.Error ? 0.9f : 0.7f
                );

                var match = new PatternMatch(
                    semanticPattern,
                    patternDef.Name,
                    0,
                    patternDef.Name.Length,
                    0.8f
                );

                matches.Add(match);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error analyzing pattern matches for {PatternId}", patternDef.Id);
        }

        return matches;
    }

    private List<PatternViolation> AnalyzePatternViolations(string code, PatternDefinition patternDef, List<PatternMatch> matches)
    {
        var violations = new List<PatternViolation>();

        // Simple violation detection
        // In a real implementation, this would analyze the code more thoroughly
        if (patternDef.Severity == PatternSeverity.Error && !matches.Any())
        {
            var violation = new PatternViolation(
                Id: Guid.NewGuid().ToString(),
                PatternId: patternDef.Id,
                PatternName: patternDef.Name,
                Severity: patternDef.Severity,
                Message: $"Missing required pattern: {patternDef.Name}",
                CodeSnippet: code.Length > 100 ? code.Substring(0, 100) + "..." : code
            );

            violations.Add(violation);
        }

        return violations;
    }

    private PatternSuggestion? GeneratePatternSuggestion(string code, PatternDefinition patternDef)
    {
        try
        {
            var suggestion = new PatternSuggestion(
                Id: Guid.NewGuid().ToString(),
                ViolationId: Guid.NewGuid().ToString(),
                Title: $"Consider implementing {patternDef.Name}",
                Description: patternDef.Description,
                CodeExample: GenerateCodeExample(patternDef),
                Confidence: CalculateSuggestionConfidence(code, patternDef),
                Effort: EstimateEffort(patternDef),
                Impact: EstimateImpact(patternDef)
            );

            return suggestion;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error generating suggestion for pattern {PatternId}", patternDef.Id);
            return null;
        }
    }

    private string? GenerateCodeExample(PatternDefinition patternDef)
    {
        // Simple code example generation
        // In a real implementation, this would generate more sophisticated examples
        return patternDef.Name switch
        {
            "Singleton" => "public class Singleton { private static Singleton instance; private Singleton() { } public static Singleton Instance => instance ??= new Singleton(); }",
            "Repository" => "public interface IRepository<T> { Task<T> GetByIdAsync(int id); Task<IEnumerable<T>> GetAllAsync(); }",
            _ => null
        };
    }

    private float CalculateSuggestionConfidence(string code, PatternDefinition patternDef)
    {
        // Simple confidence calculation
        // In a real implementation, this would use more sophisticated analysis
        var codeLength = code.Length;
        var patternComplexity = patternDef.Pattern.Length;

        if (codeLength < 50) return 0.3f;
        if (codeLength < 200) return 0.6f;
        if (patternComplexity > 100) return 0.8f;
        return 0.7f;
    }

    private SuggestionEffort EstimateEffort(PatternDefinition patternDef)
    {
        // Simple effort estimation
        return patternDef.Name switch
        {
            "Singleton" => SuggestionEffort.Low,
            "Repository" => SuggestionEffort.Medium,
            "Factory" => SuggestionEffort.Medium,
            _ => SuggestionEffort.High
        };
    }

    private SuggestionImpact EstimateImpact(PatternDefinition patternDef)
    {
        // Simple impact estimation
        return patternDef.Severity switch
        {
            PatternSeverity.Error => SuggestionImpact.VeryHigh,
            PatternSeverity.Warning => SuggestionImpact.High,
            PatternSeverity.Info => SuggestionImpact.Medium,
            _ => SuggestionImpact.Low
        };
    }

    private float CalculateAnalysisConfidence(List<PatternMatch> matches, List<PatternViolation> violations, List<PatternSuggestion> suggestions)
    {
        if (!matches.Any() && !violations.Any() && !suggestions.Any())
            return 0.1f;

        var totalItems = matches.Count + violations.Count + suggestions.Count;
        var confidenceSum = matches.Sum(m => m.Confidence) + 
                           violations.Count * 0.8f + 
                           suggestions.Sum(s => s.Confidence);

        return Math.Min(confidenceSum / totalItems, 1.0f);
    }
}
