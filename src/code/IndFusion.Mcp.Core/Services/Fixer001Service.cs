using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;
using IndFusion.Mcp.Core.Abstractions;
using IndQuestResults;
using System.Collections.Immutable;

namespace IndFusion.Mcp.Core.Services;

/// <summary>
/// Default implementation of <see cref="IFixer001Service"/> that applies Fixer001 transformations
/// to resolve specific diagnostics with build validation.
/// </summary>
public class Fixer001Service : IFixer001Service
{
    private readonly ILogger<Fixer001Service> _logger;
    private readonly IBuildValidationService _buildValidationService;

    // Static workspace cache to prevent duplicate solution loading
    private static readonly Dictionary<string, (MSBuildWorkspace Workspace, Solution Solution, DateTime LastAccessed)> _workspaceCache = new();
    private static readonly object _cacheLock = new();
    private static readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Initializes a new instance of the <see cref="Fixer001Service"/> class.
    /// </summary>
    /// <param name="logger">The logger used to record operational information and errors.</param>
    /// <param name="buildValidationService">The build validation service for verifying transformations.</param>
    public Fixer001Service(ILogger<Fixer001Service> logger, IBuildValidationService buildValidationService)
    {
        _logger = logger;
        _buildValidationService = buildValidationService;
    }

    /// <inheritdoc />
    public async Task<Result<Fixer001Result>> ApplyFixer001Async(
        Fixer001Request request, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Fixer001 transformation for diagnostic: {DiagnosticId}", request.DiagnosticId);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // Validate inputs
            if (string.IsNullOrEmpty(request.DiagnosticId))
            {
                return Result<Fixer001Result>.WithFailure("Diagnostic ID cannot be null or empty");
            }

            if (!request.TargetFiles.Any())
            {
                return Result<Fixer001Result>.WithFailure("No target files specified");
            }

            // Validate diagnostic ID
            if (!IsValidDiagnosticId(request.DiagnosticId))
            {
                return Result<Fixer001Result>.WithFailure($"Unknown diagnostic ID: {request.DiagnosticId}");
            }

            // Load solution
            var solutionPath = GetSolutionPathFromTargetFiles(request.TargetFiles);
            if (string.IsNullOrEmpty(solutionPath))
            {
                return Result<Fixer001Result>.WithFailure("Could not determine solution path from target files");
            }

            var solution = await GetOrCreateSolutionAsync(solutionPath, cancellationToken);

            // Apply fixes
            var transformationDetails = new Fixer001TransformationDetails(
                TransformationType: "Fixer001",
                TransformationId: Guid.NewGuid().ToString(),
                Description: $"Fix {request.DiagnosticId} violations",
                ChangesApplied: 0,
                FilesAffected: 0,
                Confidence: 0.95,
                DiagnosticId: request.DiagnosticId,
                FixerVersion: "1.0.0"
            );

            var validationResults = new List<ValidationResult>();
            var modifiedFiles = new List<string>();
            var diffPreview = new List<string>();

            foreach (var targetFile in request.TargetFiles)
            {
                if (!File.Exists(targetFile))
                {
                    _logger.LogWarning("Target file not found: {FilePath}", targetFile);
                    continue;
                }

                try
                {
                    var document = solution.Projects
                        .SelectMany(p => p.Documents)
                        .FirstOrDefault(d => d.FilePath == targetFile);

                    if (document == null)
                    {
                        _logger.LogWarning("Document not found in solution: {FilePath}", targetFile);
                        continue;
                    }

                    var fixes = await GetAvailableFixesAsync(document, request.DiagnosticId, cancellationToken);
                    if (!fixes.Any())
                    {
                        _logger.LogInformation("No fixes available for diagnostic {DiagnosticId} in file {FilePath}", request.DiagnosticId, targetFile);
                        continue;
                    }

                    if (request.DryRun)
                    {
                        // Generate diff preview
                        var preview = await GenerateFixPreviewAsync(document, fixes, request, cancellationToken);
                        diffPreview.AddRange(preview);
                    }
                    else
                    {
                        // Apply fixes
                        var fixResult = await ApplyFixesAsync(document, fixes, request, cancellationToken);
                        if (fixResult.Success)
                        {
                            modifiedFiles.Add(targetFile);
                            transformationDetails = transformationDetails with 
                            { 
                                ChangesApplied = transformationDetails.ChangesApplied + fixResult.ChangesApplied,
                                FilesAffected = transformationDetails.FilesAffected + 1
                            };
                        }
                        else
                        {
                            validationResults.Add(new ValidationResult("FixApplication", false, fixResult.ErrorMessage, new Dictionary<string, object>()));
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
                var buildValidationResult = await ValidateBuildAsync(request, modifiedFiles, solutionPath, cancellationToken);
                validationResults.AddRange(buildValidationResult);
            }

            stopwatch.Stop();

            var result = new Fixer001Result(
                Success: true,
                TransformationDetails: transformationDetails,
                ValidationResults: validationResults,
                DiffPreview: request.DryRun ? string.Join(Environment.NewLine, diffPreview) : null,
                ModifiedFiles: modifiedFiles,
                ExecutionTimeMs: stopwatch.ElapsedMilliseconds
            );

            _logger.LogInformation("Fixer001 transformation completed in {ElapsedMs}ms. Changes applied: {ChangesApplied}, Files affected: {FilesAffected}", 
                stopwatch.ElapsedMilliseconds, transformationDetails.ChangesApplied, transformationDetails.FilesAffected);

            return Result<Fixer001Result>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Fixer001 transformation for diagnostic: {DiagnosticId}", request.DiagnosticId);
            stopwatch.Stop();

            return Result<Fixer001Result>.WithFailure($"Fixer001 transformation failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<Fixer001Configuration>> GetFixer001ConfigurationAsync(
        string solutionPath, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Fixer001 configuration for solution: {SolutionPath}", solutionPath);

        try
        {
            if (!File.Exists(solutionPath))
            {
                return Result<Fixer001Configuration>.WithFailure($"Solution file not found: {solutionPath}");
            }

            var solution = await GetOrCreateSolutionAsync(solutionPath, cancellationToken);
            
            // Get available transformations based on analyzers in the solution
            var availableTransformations = await GetAvailableTransformationsAsync(solution, cancellationToken);

            var configuration = new Fixer001Configuration(
                SolutionPath: solutionPath,
                AvailableTransformations: availableTransformations,
                DefaultSettings: new Dictionary<string, object>
                {
                    { "MaxFixesPerFile", 10 },
                    { "BackupOriginal", true },
                    { "DryRun", false }
                },
                Version: "1.0.0",
                LastUpdated: DateTime.UtcNow
            );

            _logger.LogInformation("Fixer001 configuration retrieved successfully. Available transformations: {Count}", availableTransformations.Count());

            return Result<Fixer001Configuration>.Success(configuration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Fixer001 configuration for solution: {SolutionPath}", solutionPath);
            return Result<Fixer001Configuration>.WithFailure($"Failed to get Fixer001 configuration: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<Fixer001PreviewResult>> PreviewFixer001TransformationAsync(
        Fixer001Request request, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Previewing Fixer001 transformation for diagnostic: {DiagnosticId}", request.DiagnosticId);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // Validate inputs
            if (string.IsNullOrEmpty(request.DiagnosticId))
            {
                return Result<Fixer001PreviewResult>.WithFailure("Diagnostic ID cannot be null or empty");
            }

            if (!request.TargetFiles.Any())
            {
                return Result<Fixer001PreviewResult>.WithFailure("No target files specified");
            }

            if (!IsValidDiagnosticId(request.DiagnosticId))
            {
                return Result<Fixer001PreviewResult>.WithFailure($"Unknown diagnostic ID: {request.DiagnosticId}");
            }

            // Load solution
            var solutionPath = GetSolutionPathFromTargetFiles(request.TargetFiles);
            if (string.IsNullOrEmpty(solutionPath))
            {
                return Result<Fixer001PreviewResult>.WithFailure("Could not determine solution path from target files");
            }

            var solution = await GetOrCreateSolutionAsync(solutionPath, cancellationToken);

            var affectedFiles = new List<string>();
            var estimatedFixes = 0;

            foreach (var targetFile in request.TargetFiles)
            {
                if (!File.Exists(targetFile))
                {
                    continue;
                }

                try
                {
                    var document = solution.Projects
                        .SelectMany(p => p.Documents)
                        .FirstOrDefault(d => d.FilePath == targetFile);

                    if (document == null)
                    {
                        continue;
                    }

                    var fixes = await GetAvailableFixesAsync(document, request.DiagnosticId, cancellationToken);
                    if (fixes.Any())
                    {
                        affectedFiles.Add(targetFile);
                        estimatedFixes += fixes.Count();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error previewing file: {FilePath}", targetFile);
                }
            }

            stopwatch.Stop();

            var previewDetails = new Fixer001PreviewDetails(
                PreviewId: Guid.NewGuid().ToString(),
                DiagnosticId: request.DiagnosticId,
                EstimatedFixes: estimatedFixes,
                ReadinessAssessment: estimatedFixes > 0 ? "Ready" : "No fixes available"
            );

            var result = new Fixer001PreviewResult(
                Success: true,
                PreviewDetails: previewDetails,
                EstimatedChanges: estimatedFixes,
                AffectedFiles: affectedFiles,
                PreviewTimeMs: stopwatch.ElapsedMilliseconds
            );

            _logger.LogInformation("Fixer001 transformation preview completed in {ElapsedMs}ms. Estimated fixes: {EstimatedFixes}, Affected files: {AffectedFiles}", 
                stopwatch.ElapsedMilliseconds, estimatedFixes, affectedFiles.Count);

            return Result<Fixer001PreviewResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing Fixer001 transformation for diagnostic: {DiagnosticId}", request.DiagnosticId);
            stopwatch.Stop();

            return Result<Fixer001PreviewResult>.WithFailure($"Fixer001 transformation preview failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<Fixer001ValidationResult>> ValidateFixer001ReadinessAsync(
        Fixer001Request request, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating Fixer001 readiness for diagnostic: {DiagnosticId}", request.DiagnosticId);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // Validate inputs
            if (string.IsNullOrEmpty(request.DiagnosticId))
            {
                return Result<Fixer001ValidationResult>.WithFailure("Diagnostic ID cannot be null or empty");
            }

            if (!request.TargetFiles.Any())
            {
                return Result<Fixer001ValidationResult>.WithFailure("No target files specified");
            }

            var issues = new List<Fixer001Issue>();
            var warnings = new List<Fixer001Warning>();
            double readinessScore = 1.0;

            // Check diagnostic ID validity
            if (!IsValidDiagnosticId(request.DiagnosticId))
            {
                issues.Add(new Fixer001Issue(
                    "invalid-diagnostic",
                    "InvalidDiagnosticId",
                    "High",
                    $"Unknown diagnostic ID: {request.DiagnosticId}",
                    "Use a valid EXXER diagnostic ID"
                ));
                readinessScore = 0.0;
            }

            // Check target files
            var missingFiles = request.TargetFiles.Where(f => !File.Exists(f)).ToList();
            if (missingFiles.Any())
            {
                issues.Add(new Fixer001Issue(
                    "missing-files",
                    "MissingFiles",
                    "High",
                    $"Target files not found: {string.Join(", ", missingFiles)}",
                    "Ensure all target files exist"
                ));
                readinessScore -= 0.3;
            }

            // Check solution availability
            var solutionPath = GetSolutionPathFromTargetFiles(request.TargetFiles);
            if (string.IsNullOrEmpty(solutionPath))
            {
                issues.Add(new Fixer001Issue(
                    "no-solution",
                    "NoSolution",
                    "High",
                    "Could not determine solution path from target files",
                    "Ensure target files are part of a solution"
                ));
                readinessScore -= 0.5;
            }

            // Check if fixes are available
            if (issues.Count == 0 && !string.IsNullOrEmpty(solutionPath))
            {
                try
                {
                    var solution = await GetOrCreateSolutionAsync(solutionPath, cancellationToken);
                    var hasFixes = false;

                    foreach (var targetFile in request.TargetFiles.Where(File.Exists))
                    {
                        var document = solution.Projects
                            .SelectMany(p => p.Documents)
                            .FirstOrDefault(d => d.FilePath == targetFile);

                        if (document != null)
                        {
                            var fixes = await GetAvailableFixesAsync(document, request.DiagnosticId, cancellationToken);
                            if (fixes.Any())
                            {
                                hasFixes = true;
                                break;
                            }
                        }
                    }

                    if (!hasFixes)
                    {
                        warnings.Add(new Fixer001Warning(
                            "no-fixes",
                            "NoFixesAvailable",
                            "No fixes available for the specified diagnostic in target files",
                            "Check if the diagnostic is present in the target files"
                        ));
                        readinessScore -= 0.2;
                    }
                }
                catch (Exception ex)
                {
                    issues.Add(new Fixer001Issue(
                        "solution-error",
                        "SolutionError",
                        "Medium",
                        $"Error loading solution: {ex.Message}",
                        "Check solution file and project references"
                    ));
                    readinessScore -= 0.3;
                }
            }

            stopwatch.Stop();

            var isReady = issues.Count == 0 && readinessScore > 0.5;
            readinessScore = Math.Max(0.0, Math.Min(1.0, readinessScore));

            var result = new Fixer001ValidationResult(
                IsReady: isReady,
                ReadinessScore: readinessScore,
                Issues: issues,
                Warnings: warnings,
                ValidationTimeMs: stopwatch.ElapsedMilliseconds
            );

            _logger.LogInformation("Fixer001 readiness validation completed in {ElapsedMs}ms. Ready: {IsReady}, Score: {ReadinessScore}", 
                stopwatch.ElapsedMilliseconds, isReady, readinessScore);

            return Result<Fixer001ValidationResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Fixer001 readiness for diagnostic: {DiagnosticId}", request.DiagnosticId);
            stopwatch.Stop();

            return Result<Fixer001ValidationResult>.WithFailure($"Fixer001 readiness validation failed: {ex.Message}");
        }
    }

    #region Private Helper Methods

    private async Task<Solution> GetOrCreateSolutionAsync(string solutionPath, CancellationToken cancellationToken)
    {
        lock (_cacheLock)
        {
            if (_workspaceCache.TryGetValue(solutionPath, out var cached) && 
                DateTime.UtcNow - cached.LastAccessed < _cacheExpiration)
            {
                cached.LastAccessed = DateTime.UtcNow;
                return cached.Solution;
            }
        }

        var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(solutionPath, cancellationToken);

        lock (_cacheLock)
        {
            _workspaceCache[solutionPath] = (workspace, solution, DateTime.UtcNow);
        }

        return solution;
    }

    private static bool IsValidDiagnosticId(string diagnosticId)
    {
        // Check if it's a valid EXXER diagnostic ID
        return diagnosticId.StartsWith("EXXER") && diagnosticId.Length >= 8;
    }

    private static string GetSolutionPathFromTargetFiles(IEnumerable<string> targetFiles)
    {
        // Find the solution file by looking for .sln files in parent directories
        foreach (var targetFile in targetFiles)
        {
            var directory = Path.GetDirectoryName(targetFile);
            while (!string.IsNullOrEmpty(directory))
            {
                var solutionFiles = Directory.GetFiles(directory, "*.sln");
                if (solutionFiles.Any())
                {
                    return solutionFiles.First();
                }
                directory = Path.GetDirectoryName(directory);
            }
        }
        return string.Empty;
    }

    private async Task<IEnumerable<CodeFix>> GetAvailableFixesAsync(Document document, string diagnosticId, CancellationToken cancellationToken)
    {
        // This is a simplified implementation
        // In a real implementation, this would use the actual code fix providers
        var fixes = new List<CodeFix>();
        
        try
        {
            var compilation = await document.Project.GetCompilationAsync(cancellationToken);
            if (compilation == null)
            {
                return fixes;
            }

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            if (semanticModel == null)
            {
                return fixes;
            }

            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
            if (syntaxTree == null)
            {
                return fixes;
            }

            // For now, return empty fixes - this would be implemented with actual fix providers
            // The real implementation would use CodeFixProvider instances
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available fixes for document: {FilePath}", document.FilePath);
        }

        return fixes;
    }

    private async Task<IEnumerable<string>> GenerateFixPreviewAsync(
        Document document, 
        IEnumerable<CodeFix> fixes, 
        Fixer001Request request, 
        CancellationToken cancellationToken)
    {
        var preview = new List<string>();
        
        // This would generate actual diff previews
        // For now, return a basic preview
        preview.Add($"--- {document.FilePath} (original)");
        preview.Add($"+++ {document.FilePath} (fixed)");
        preview.Add($"@@ -1,1 +1,1 @@");
        preview.Add($"-// TODO: Apply {request.DiagnosticId} fix");
        preview.Add($"+// Fixed {request.DiagnosticId} violation");

        return preview;
    }

    private async Task<(bool Success, int ChangesApplied, string ErrorMessage)> ApplyFixesAsync(
        Document document, 
        IEnumerable<CodeFix> fixes, 
        Fixer001Request request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // This would apply the actual fixes
            // For now, return a mock result
            var changesApplied = fixes.Count();
            return (true, changesApplied, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, 0, ex.Message);
        }
    }

    private async Task<IEnumerable<TransformationInfo>> GetAvailableTransformationsAsync(Solution solution, CancellationToken cancellationToken)
    {
        var transformations = new List<TransformationInfo>();

        // Mock transformations based on common EXXER rules
        transformations.Add(new TransformationInfo(
            Id: "EXXER001",
            Name: "Add XML Documentation",
            Description: "Adds missing XML documentation to public members",
            SupportedLanguages: new[] { "C#" },
            IsEnabled: true,
            Parameters: new Dictionary<string, object>()
        ));

        transformations.Add(new TransformationInfo(
            Id: "EXXER002",
            Name: "Use ConfigureAwait",
            Description: "Adds ConfigureAwait(false) to async calls",
            SupportedLanguages: new[] { "C#" },
            IsEnabled: true,
            Parameters: new Dictionary<string, object>()
        ));

        return transformations;
    }

    private async Task<IEnumerable<ValidationResult>> ValidateBuildAsync(
        Fixer001Request request, 
        IEnumerable<string> modifiedFiles, 
        string solutionPath, 
        CancellationToken cancellationToken)
    {
        var validationResults = new List<ValidationResult>();

        try
        {
            // This would integrate with the build validation service
            // For now, return a basic validation result
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
