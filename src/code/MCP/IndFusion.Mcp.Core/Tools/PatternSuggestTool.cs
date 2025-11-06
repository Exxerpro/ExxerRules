using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;
using IndQuestResults;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// MCP tool for suggesting patterns based on code context.
/// This tool analyzes code and suggests appropriate design patterns and best practices.
/// </summary>
[McpServerToolType]
public static class PatternSuggestTool
{
    /// <summary>
    /// Suggests patterns based on the provided code context.
    /// </summary>
    /// <param name="codeContext">The code context to analyze for pattern suggestions.</param>
    /// <param name="maxSuggestions">Maximum number of suggestions to return (default: 10).</param>
    /// <param name="minConfidence">Minimum confidence threshold for suggestions (0.0 to 1.0, default: 0.5).</param>
    /// <param name="categories">Comma-separated list of pattern categories to filter by (optional).</param>
    /// <param name="includeCodeExamples">Whether to include code examples in suggestions (default: true).</param>
    /// <param name="includeEffortEstimate">Whether to include effort estimates in suggestions (default: true).</param>
    /// <param name="progress">Optional progress reporter for operation status updates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A structured result containing pattern suggestions with confidence scores and examples.</returns>
    [McpServerTool, Description("Suggest patterns based on code context analysis")]
    public static async Task<string> PatternSuggest(
        [Description("The code context to analyze for pattern suggestions")] string codeContext,
        [Description("Maximum number of suggestions to return (default: 10)")] int maxSuggestions = 10,
        [Description("Minimum confidence threshold for suggestions (0.0 to 1.0, default: 0.5)")] float minConfidence = 0.5f,
        [Description("Comma-separated list of pattern categories to filter by (optional)")] string? categories = null,
        [Description("Whether to include code examples in suggestions (default: true)")] bool includeCodeExamples = true,
        [Description("Whether to include effort estimates in suggestions (default: true)")] bool includeEffortEstimate = true,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Starting pattern suggestion analysis...");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(codeContext))
            {
                throw new McpException("Code context cannot be null or empty");
            }

            if (maxSuggestions <= 0)
            {
                throw new McpException("Max suggestions must be greater than 0");
            }

            if (minConfidence < 0.0f || minConfidence > 1.0f)
            {
                throw new McpException("Min confidence must be between 0.0 and 1.0");
            }

            progress?.Report("Validating pattern suggestion options...");

            // Parse categories if provided
            var categoryList = string.IsNullOrWhiteSpace(categories)
                ? null
                : categories.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim())
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .ToList();

            // Create pattern suggestion options
            var options = new PatternSuggestionOptions(
                MaxSuggestions: maxSuggestions,
                MinConfidence: minConfidence,
                Categories: categoryList,
                IncludeCodeExamples: includeCodeExamples,
                IncludeEffortEstimate: includeEffortEstimate
            );

            progress?.Report("Analyzing code context for patterns...");

            // Get the pattern suggest service from DI
            var serviceProvider = ServiceProviderAccessor.ServiceProvider;
            if (serviceProvider == null)
            {
                throw new McpException("Service provider is not available");
            }

            var patternSuggestService = serviceProvider.GetService(typeof(IPatternSuggestService)) as IPatternSuggestService;
            if (patternSuggestService == null)
            {
                throw new McpException("Pattern suggest service is not available");
            }

            // Execute pattern suggestion
            var result = await patternSuggestService.SuggestPatternsAsync(codeContext, options, cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                throw new McpException($"Pattern suggestion failed: {result.Error}");
            }

            progress?.Report($"Generated {result.Value?.Count ?? 0} pattern suggestions");

            // Format the response
            var response = new PatternSuggestionResponse(
                Suggestions: result.Value ?? [],
                TotalSuggestions: result.Value?.Count ?? 0,
                AnalysisTimeMs: 0, // Would be calculated in real implementation
                ConfidenceThreshold: minConfidence,
                CategoriesFiltered: categoryList?.Count ?? 0
            );

            return FormatPatternSuggestionResponse(response);
        }
        catch (OperationCanceledException)
        {
            progress?.Report("Pattern suggestion was cancelled");
            throw new McpException("Operation was cancelled");
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            progress?.Report($"Error during pattern suggestion: {ex.Message}");
            throw new McpException($"Pattern suggestion failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Analyzes a specific pattern in the provided code.
    /// </summary>
    /// <param name="code">The code to analyze.</param>
    /// <param name="patternType">The type of pattern to analyze for.</param>
    /// <param name="progress">Optional progress reporter for operation status updates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A structured result containing pattern analysis with matches, violations, and suggestions.</returns>
    [McpServerTool, Description("Analyze a specific pattern in the provided code")]
    public static async Task<string> AnalyzePattern(
        [Description("The code to analyze")] string code,
        [Description("The type of pattern to analyze for")] string patternType,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Starting pattern analysis...");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new McpException("Code cannot be null or empty");
            }

            if (string.IsNullOrWhiteSpace(patternType))
            {
                throw new McpException("Pattern type cannot be null or empty");
            }

            progress?.Report($"Analyzing code for pattern type: {patternType}");

            // Get the pattern suggest service from DI
            var serviceProvider = ServiceProviderAccessor.ServiceProvider;
            if (serviceProvider == null)
            {
                throw new McpException("Service provider is not available");
            }

            var patternSuggestService = serviceProvider.GetService(typeof(IPatternSuggestService)) as IPatternSuggestService;
            if (patternSuggestService == null)
            {
                throw new McpException("Pattern suggest service is not available");
            }

            // Execute pattern analysis
            var result = await patternSuggestService.AnalyzePatternAsync(code, patternType, cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                throw new McpException($"Pattern analysis failed: {result.Error}");
            }

            progress?.Report($"Pattern analysis completed: {result.Value.MatchCount} matches, {result.Value.ViolationCount} violations, {result.Value.SuggestionCount} suggestions");

            // Format the response
            var response = new PatternAnalysisResponse(
                Analysis: result.Value,
                Success: true,
                ErrorMessage: null
            );

            return FormatPatternAnalysisResponse(response);
        }
        catch (OperationCanceledException)
        {
            progress?.Report("Pattern analysis was cancelled");
            throw new McpException("Operation was cancelled");
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            progress?.Report($"Error during pattern analysis: {ex.Message}");
            throw new McpException($"Pattern analysis failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Finds pattern violations in the provided code.
    /// </summary>
    /// <param name="code">The code to analyze for violations.</param>
    /// <param name="filePath">Optional file path for context.</param>
    /// <param name="progress">Optional progress reporter for operation status updates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A structured result containing pattern violations with locations and suggested fixes.</returns>
    [McpServerTool, Description("Find pattern violations in the provided code")]
    public static async Task<string> FindViolations(
        [Description("The code to analyze for violations")] string code,
        [Description("Optional file path for context")] string? filePath = null,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Starting violation detection...");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new McpException("Code cannot be null or empty");
            }

            progress?.Report("Analyzing code for pattern violations...");

            // Get the pattern suggest service from DI
            var serviceProvider = ServiceProviderAccessor.ServiceProvider;
            if (serviceProvider == null)
            {
                throw new McpException("Service provider is not available");
            }

            var patternSuggestService = serviceProvider.GetService(typeof(IPatternSuggestService)) as IPatternSuggestService;
            if (patternSuggestService == null)
            {
                throw new McpException("Pattern suggest service is not available");
            }

            // Execute violation detection
            var result = await patternSuggestService.FindViolationsAsync(code, filePath, cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                throw new McpException($"Violation detection failed: {result.Error}");
            }

            progress?.Report($"Found {result.Value?.Count ?? 0} pattern violations");

            // Format the response
            var response = new PatternViolationResponse(
                Violations: result.Value ?? [],
                TotalViolations: result.Value?.Count ?? 0,
                FilePath: filePath,
                AnalysisTimeMs: 0 // Would be calculated in real implementation
            );

            return FormatPatternViolationResponse(response);
        }
        catch (OperationCanceledException)
        {
            progress?.Report("Violation detection was cancelled");
            throw new McpException("Operation was cancelled");
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            progress?.Report($"Error during violation detection: {ex.Message}");
            throw new McpException($"Violation detection failed: {ex.Message}");
        }
    }

    private static string FormatPatternSuggestionResponse(PatternSuggestionResponse response)
    {
        var suggestions = response.Suggestions.Select(s => new
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            CodeExample = s.CodeExample,
            Confidence = s.Confidence,
            Effort = s.Effort.ToString(),
            Impact = s.Impact.ToString()
        }).ToList();

        var result = new
        {
            Success = true,
            TotalSuggestions = response.TotalSuggestions,
            ConfidenceThreshold = response.ConfidenceThreshold,
            CategoriesFiltered = response.CategoriesFiltered,
            AnalysisTimeMs = response.AnalysisTimeMs,
            Suggestions = suggestions
        };

        return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

    private static string FormatPatternAnalysisResponse(PatternAnalysisResponse response)
    {
        var matches = response.Analysis.Matches.Select(m => new
        {
            Pattern = m.Pattern.Name,
            Match = m.Match,
            StartIndex = m.StartIndex,
            EndIndex = m.EndIndex,
            Confidence = m.Confidence
        }).ToList();

        var violations = response.Analysis.Violations.Select(v => new
        {
            Id = v.Id,
            PatternName = v.PatternName,
            Severity = v.Severity.ToString(),
            Message = v.Message,
            Location = v.Location,
            CodeSnippet = v.CodeSnippet
        }).ToList();

        var suggestions = response.Analysis.Suggestions.Select(s => new
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            CodeExample = s.CodeExample,
            Confidence = s.Confidence,
            Effort = s.Effort.ToString(),
            Impact = s.Impact.ToString()
        }).ToList();

        var result = new
        {
            Success = response.Success,
            PatternType = response.Analysis.PatternType,
            MatchCount = response.Analysis.MatchCount,
            ViolationCount = response.Analysis.ViolationCount,
            SuggestionCount = response.Analysis.SuggestionCount,
            Confidence = response.Analysis.Confidence,
            AnalysisTimeMs = response.Analysis.AnalysisTimeMs,
            Matches = matches,
            Violations = violations,
            Suggestions = suggestions,
            ErrorMessage = response.ErrorMessage
        };

        return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

    private static string FormatPatternViolationResponse(PatternViolationResponse response)
    {
        var violations = response.Violations.Select(v => new
        {
            Id = v.Id,
            PatternId = v.PatternId,
            PatternName = v.PatternName,
            Severity = v.Severity.ToString(),
            Message = v.Message,
            FilePath = v.FilePath,
            LineNumber = v.LineNumber,
            ColumnNumber = v.ColumnNumber,
            CodeSnippet = v.CodeSnippet,
            Location = v.Location
        }).ToList();

        var result = new
        {
            Success = true,
            TotalViolations = response.TotalViolations,
            FilePath = response.FilePath,
            AnalysisTimeMs = response.AnalysisTimeMs,
            Violations = violations
        };

        return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }
}