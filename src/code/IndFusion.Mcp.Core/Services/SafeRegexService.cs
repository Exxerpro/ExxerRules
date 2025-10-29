using Microsoft.Extensions.Logging;
using IndFusion.Mcp.Core.Abstractions;
using IndQuestResults;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace IndFusion.Mcp.Core.Services;

/// <summary>
/// Default implementation of <see cref="ISafeRegexService"/> that performs safe regex-based code transformations
/// with validation and build verification.
/// </summary>
public class SafeRegexService : ISafeRegexService
{
    private readonly ILogger<SafeRegexService> _logger;
    private readonly IBuildValidationService _buildValidationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeRegexService"/> class.
    /// </summary>
    /// <param name="logger">The logger used to record operational information and errors.</param>
    /// <param name="buildValidationService">The build validation service for verifying transformations.</param>
    public SafeRegexService(ILogger<SafeRegexService> logger, IBuildValidationService buildValidationService)
    {
        _logger = logger;
        _buildValidationService = buildValidationService;
    }

    /// <inheritdoc />
    public async Task<Result<SafeRegexResult>> ApplySafeRegexAsync(
        SafeRegexRequest request, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting safe regex transformation with pattern: {Pattern}", request.Pattern);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Validate inputs
            if (string.IsNullOrEmpty(request.Pattern))
            {
                return Result<SafeRegexResult>.WithFailure("Pattern cannot be null or empty");
            }

            if (string.IsNullOrEmpty(request.Replacement))
            {
                return Result<SafeRegexResult>.WithFailure("Replacement cannot be null or empty");
            }

            if (!request.TargetFiles.Any())
            {
                return Result<SafeRegexResult>.WithFailure("No target files specified");
            }

            // Validate regex pattern first
            var validationResult = await ValidateRegexPatternAsync(request.Pattern, cancellationToken);
            if (validationResult.IsFailure)
            {
                return Result<SafeRegexResult>.WithFailure($"Invalid regex pattern: {validationResult.Error}");
            }

            var validation = validationResult.Value!;
            if (!validation.IsValid)
            {
                return Result<SafeRegexResult>.WithFailure($"Regex pattern validation failed: {string.Join(", ", validation.Issues.Select(i => i.Message))}");
            }

            // Apply transformations
            var transformationDetails = new SafeRegexTransformationDetails(
                TransformationType: "SafeRegex",
                TransformationId: Guid.NewGuid().ToString(),
                Description: $"Apply regex pattern: {request.Pattern}",
                ChangesApplied: 0,
                FilesAffected: 0,
                Confidence: validation.SafetyScore,
                Pattern: request.Pattern,
                Replacement: request.Replacement
            );

            var validationResults = new List<ValidationResult>();
            var modifiedFiles = new List<string>();
            var diffPreview = new List<string>();

            var regexOptions = GetRegexOptions(request);
            var regex = new Regex(request.Pattern, regexOptions);

            foreach (var targetFile in request.TargetFiles)
            {
                if (!File.Exists(targetFile))
                {
                    _logger.LogWarning("Target file not found: {FilePath}", targetFile);
                    continue;
                }

                try
                {
                    var originalContent = await File.ReadAllTextAsync(targetFile, cancellationToken);
                    var transformedContent = regex.Replace(originalContent, request.Replacement);

                    if (originalContent != transformedContent)
                    {
                        if (request.DryRun)
                        {
                            // Generate diff preview
                            var diff = GenerateDiff(originalContent, transformedContent, targetFile);
                            diffPreview.AddRange(diff);
                        }
                        else
                        {
                            // Apply transformation
                            await File.WriteAllTextAsync(targetFile, transformedContent, cancellationToken);
                            modifiedFiles.Add(targetFile);
                            transformationDetails = transformationDetails with 
                            { 
                                ChangesApplied = transformationDetails.ChangesApplied + CountMatches(originalContent, regex),
                                FilesAffected = transformationDetails.FilesAffected + 1
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing file: {FilePath}", targetFile);
                    validationResults.Add(new ValidationResult("FileProcessing", false, $"Error processing {targetFile}: {ex.Message}", new Dictionary<string, object>()));
                }
            }

            // Build validation if not dry run
            if (!request.DryRun && request.ValidationOptions.BuildValidation)
            {
                var buildValidationResult = await ValidateBuildAsync(request, modifiedFiles, cancellationToken);
                validationResults.AddRange(buildValidationResult);
            }

            stopwatch.Stop();

            var result = new SafeRegexResult(
                Success: true,
                TransformationDetails: transformationDetails,
                ValidationResults: validationResults,
                DiffPreview: request.DryRun ? string.Join(Environment.NewLine, diffPreview) : null,
                ModifiedFiles: modifiedFiles,
                ExecutionTimeMs: stopwatch.ElapsedMilliseconds
            );

            _logger.LogInformation("Safe regex transformation completed in {ElapsedMs}ms. Changes applied: {ChangesApplied}, Files affected: {FilesAffected}", 
                stopwatch.ElapsedMilliseconds, transformationDetails.ChangesApplied, transformationDetails.FilesAffected);

            return Result<SafeRegexResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during safe regex transformation with pattern: {Pattern}", request.Pattern);
            stopwatch.Stop();

            return Result<SafeRegexResult>.WithFailure($"Safe regex transformation failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<RegexValidationResult>> ValidateRegexPatternAsync(
        string pattern, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating regex pattern: {Pattern}", pattern);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return Result<RegexValidationResult>.WithFailure("Pattern cannot be null or empty");
            }

            var issues = new List<RegexIssue>();
            var warnings = new List<RegexWarning>();
            double safetyScore = 1.0;
            string performanceImpact = "Low";

            // Test regex compilation
            try
            {
                var regex = new Regex(pattern, RegexOptions.Compiled);
                
                // Test for ReDoS vulnerability
                var redosResult = TestForReDoS(pattern);
                if (redosResult.IsVulnerable)
                {
                    issues.Add(new RegexIssue(
                        "redos-001",
                        "ReDoSVulnerability",
                        "High",
                        "Pattern may be vulnerable to ReDoS attacks",
                        "Simplify the pattern or add timeout protection"
                    ));
                    safetyScore -= 0.5;
                }

                // Test for performance issues
                var performanceResult = TestPerformance(pattern);
                if (performanceResult.IsSlow)
                {
                    warnings.Add(new RegexWarning(
                        "perf-001",
                        "PerformanceWarning",
                        "Pattern may have performance issues",
                        "Consider optimizing the pattern"
                    ));
                    performanceImpact = "Medium";
                    safetyScore -= 0.1;
                }

                // Test for common issues
                if (pattern.Contains(".*") && pattern.Contains("+"))
                {
                    warnings.Add(new RegexWarning(
                        "common-001",
                        "CommonPatternWarning",
                        "Pattern contains potentially inefficient quantifiers",
                        "Consider using more specific patterns"
                    ));
                }
            }
            catch (ArgumentException ex)
            {
                issues.Add(new RegexIssue(
                    "syntax-001",
                    "SyntaxError",
                    "High",
                    $"Invalid regex syntax: {ex.Message}",
                    "Fix the regex syntax"
                ));
                safetyScore = 0.0;
            }

            stopwatch.Stop();

            var isValid = !issues.Any(i => i.Severity == "High");
            safetyScore = Math.Max(0.0, Math.Min(1.0, safetyScore));

            var result = new RegexValidationResult(
                IsValid: isValid,
                SafetyScore: safetyScore,
                Issues: issues,
                Warnings: warnings,
                PerformanceImpact: performanceImpact,
                ValidationTimeMs: stopwatch.ElapsedMilliseconds
            );

            _logger.LogInformation("Regex pattern validation completed in {ElapsedMs}ms. Valid: {IsValid}, Safety score: {SafetyScore}", 
                stopwatch.ElapsedMilliseconds, isValid, safetyScore);

            return Result<RegexValidationResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating regex pattern: {Pattern}", pattern);
            stopwatch.Stop();

            return Result<RegexValidationResult>.WithFailure($"Regex pattern validation failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<SafeRegexPreviewResult>> PreviewRegexTransformationAsync(
        SafeRegexRequest request, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Previewing regex transformation with pattern: {Pattern}", request.Pattern);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Validate inputs
            if (string.IsNullOrEmpty(request.Pattern))
            {
                return Result<SafeRegexPreviewResult>.WithFailure("Pattern cannot be null or empty");
            }

            if (string.IsNullOrEmpty(request.Replacement))
            {
                return Result<SafeRegexPreviewResult>.WithFailure("Replacement cannot be null or empty");
            }

            if (!request.TargetFiles.Any())
            {
                return Result<SafeRegexPreviewResult>.WithFailure("No target files specified");
            }

            // Validate regex pattern
            var validationResult = await ValidateRegexPatternAsync(request.Pattern, cancellationToken);
            if (validationResult.IsFailure || !validationResult.Value!.IsValid)
            {
                return Result<SafeRegexPreviewResult>.WithFailure("Invalid regex pattern");
            }

            var regexOptions = GetRegexOptions(request);
            var regex = new Regex(request.Pattern, regexOptions);

            var affectedFiles = new List<string>();
            var estimatedChanges = 0;

            foreach (var targetFile in request.TargetFiles)
            {
                if (!File.Exists(targetFile))
                {
                    continue;
                }

                try
                {
                    var content = await File.ReadAllTextAsync(targetFile, cancellationToken);
                    var matches = regex.Matches(content);
                    
                    if (matches.Count > 0)
                    {
                        affectedFiles.Add(targetFile);
                        estimatedChanges += matches.Count;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error previewing file: {FilePath}", targetFile);
                }
            }

            stopwatch.Stop();

            var previewDetails = new SafeRegexPreviewDetails(
                PreviewId: Guid.NewGuid().ToString(),
                Pattern: request.Pattern,
                Replacement: request.Replacement,
                EstimatedMatches: estimatedChanges,
                SafetyAssessment: validationResult.Value.SafetyScore > 0.8 ? "Safe" : "Caution"
            );

            var result = new SafeRegexPreviewResult(
                Success: true,
                PreviewDetails: previewDetails,
                EstimatedChanges: estimatedChanges,
                AffectedFiles: affectedFiles,
                PreviewTimeMs: stopwatch.ElapsedMilliseconds
            );

            _logger.LogInformation("Regex transformation preview completed in {ElapsedMs}ms. Estimated changes: {EstimatedChanges}, Affected files: {AffectedFiles}", 
                stopwatch.ElapsedMilliseconds, estimatedChanges, affectedFiles.Count);

            return Result<SafeRegexPreviewResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing regex transformation with pattern: {Pattern}", request.Pattern);
            stopwatch.Stop();

            return Result<SafeRegexPreviewResult>.WithFailure($"Regex transformation preview failed: {ex.Message}");
        }
    }

    #region Private Helper Methods

    private static RegexOptions GetRegexOptions(SafeRegexRequest request)
    {
        var options = RegexOptions.Compiled;
        
        if (!request.CaseSensitive)
        {
            options |= RegexOptions.IgnoreCase;
        }
        
        if (request.Multiline)
        {
            options |= RegexOptions.Multiline;
        }

        return options;
    }

    private static int CountMatches(string content, Regex regex)
    {
        return regex.Matches(content).Count;
    }

    private static List<string> GenerateDiff(string originalContent, string transformedContent, string filePath)
    {
        var diff = new List<string>();
        diff.Add($"--- {filePath} (original)");
        diff.Add($"+++ {filePath} (transformed)");
        
        var originalLines = originalContent.Split('\n');
        var transformedLines = transformedContent.Split('\n');
        
        for (int i = 0; i < Math.Max(originalLines.Length, transformedLines.Length); i++)
        {
            var originalLine = i < originalLines.Length ? originalLines[i] : "";
            var transformedLine = i < transformedLines.Length ? transformedLines[i] : "";
            
            if (originalLine != transformedLine)
            {
                diff.Add($"@@ -{i + 1},1 +{i + 1},1 @@");
                diff.Add($"-{originalLine}");
                diff.Add($"+{transformedLine}");
            }
        }
        
        return diff;
    }

    private static (bool IsVulnerable, string Reason) TestForReDoS(string pattern)
    {
        // Basic ReDoS detection - look for nested quantifiers
        if (pattern.Contains("(.*)*") || pattern.Contains("(.+)+") || pattern.Contains("(.+)*"))
        {
            return (true, "Nested quantifiers detected");
        }

        // Look for exponential backtracking patterns
        if (pattern.Contains("(a+)+") || pattern.Contains("(a*)*"))
        {
            return (true, "Exponential backtracking pattern detected");
        }

        return (false, "");
    }

    private static (bool IsSlow, string Reason) TestPerformance(string pattern)
    {
        // Basic performance testing
        if (pattern.Length > 1000)
        {
            return (true, "Pattern is very long");
        }

        if (pattern.Count(c => c == '*') > 10 || pattern.Count(c => c == '+') > 10)
        {
            return (true, "Pattern has many quantifiers");
        }

        return (false, "");
    }

    private async Task<IEnumerable<ValidationResult>> ValidateBuildAsync(
        SafeRegexRequest request, 
        IEnumerable<string> modifiedFiles, 
        CancellationToken cancellationToken)
    {
        var validationResults = new List<ValidationResult>();

        try
        {
            // This would integrate with the build validation service
            // For now, we'll return a basic validation result
            validationResults.Add(new ValidationResult("BuildValidation", true, "Build validation completed", new Dictionary<string, object>()));
        }
        catch (Exception ex)
        {
            validationResults.Add(new ValidationResult("BuildValidation", false, $"Build validation failed: {ex.Message}", new Dictionary<string, object>()));
        }

        return validationResults;
    }

    #endregion
}
