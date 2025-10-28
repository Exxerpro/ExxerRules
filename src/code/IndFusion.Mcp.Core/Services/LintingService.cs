using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Logging;
using IndFusion.Mcp.Core.Abstractions;
using System.Collections.Immutable;

namespace IndFusion.Mcp.Core.Services;

/// <summary>
/// Default implementation of <see cref="ILintingService"/> that runs EXXER analyzers
/// and applies policies to provide comprehensive linting results.
/// </summary>
public class LintingService : ILintingService
{
    private readonly ILogger<LintingService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LintingService"/> class.
    /// </summary>
    /// <param name="logger">The logger used to record operational information and errors.</param>
    public LintingService(ILogger<LintingService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<LintingResult> RunLintingAsync(LintingRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting linting analysis for solution: {SolutionPath}, scope: {Scope}", 
            request.SolutionPath, request.Scope);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // Validate inputs
            if (!File.Exists(request.SolutionPath))
            {
                throw new ArgumentException($"Solution file not found: {request.SolutionPath}", nameof(request));
            }

            // Load solution
            var solution = await LoadSolutionAsync(request.SolutionPath, cancellationToken);
            
            // Determine files to analyze
            var filesToAnalyze = await GetFilesToAnalyzeAsync(solution, request.Scope, cancellationToken);
            
            // Run analyzers
            var violations = await RunAnalyzersAsync(solution, filesToAnalyze, request, cancellationToken);
            
            // Apply policies
            var policyDecisions = await ApplyPoliciesAsync(violations, request, cancellationToken);
            
            // Generate summary
            var summary = GenerateSummary(violations, filesToAnalyze.Count);
            
            stopwatch.Stop();

            _logger.LogInformation("Linting analysis completed in {ElapsedMs}ms. Found {ViolationCount} violations", 
                stopwatch.ElapsedMilliseconds, violations.Count());

            return new LintingResult(
                Success: true,
                Violations: violations,
                Summary: summary,
                PolicyDecisions: policyDecisions,
                ExecutionTimeMs: stopwatch.ElapsedMilliseconds
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during linting analysis for solution: {SolutionPath}", request.SolutionPath);
            stopwatch.Stop();

            return new LintingResult(
                Success: false,
                Violations: Enumerable.Empty<LintingViolation>(),
                Summary: new LintingSummary(0, 0, 0, 0, 0, 0, 0),
                PolicyDecisions: Enumerable.Empty<PolicyDecision>(),
                ExecutionTimeMs: stopwatch.ElapsedMilliseconds,
                ErrorDetails: ex.Message
            );
        }
    }

    /// <inheritdoc />
    public async Task<LintingResult> StartWatcherAsync(LintingWatchRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting linting watcher for solution: {SolutionPath}", request.SolutionPath);

        // TODO: Implement file watcher functionality
        // This would involve setting up FileSystemWatcher and debouncing logic
        await Task.Delay(100, cancellationToken); // Placeholder

        return new LintingResult(
            Success: true,
            Violations: Enumerable.Empty<LintingViolation>(),
            Summary: new LintingSummary(0, 0, 0, 0, 0, 0, 0),
            PolicyDecisions: Enumerable.Empty<PolicyDecision>(),
            ExecutionTimeMs: 0
        );
    }

    /// <inheritdoc />
    public async Task<LintingResult> StopWatcherAsync(string solutionPath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping linting watcher for solution: {SolutionPath}", solutionPath);

        // TODO: Implement watcher stop functionality
        await Task.Delay(100, cancellationToken); // Placeholder

        return new LintingResult(
            Success: true,
            Violations: Enumerable.Empty<LintingViolation>(),
            Summary: new LintingSummary(0, 0, 0, 0, 0, 0, 0),
            PolicyDecisions: Enumerable.Empty<PolicyDecision>(),
            ExecutionTimeMs: 0
        );
    }

    /// <inheritdoc />
    public async Task<LintingPolicy> GetPolicyAsync(string solutionPath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting linting policy for solution: {SolutionPath}", solutionPath);

        // TODO: Implement policy retrieval from configuration files
        await Task.Delay(100, cancellationToken); // Placeholder

        return new LintingPolicy(
            SolutionPath: solutionPath,
            RuleSeverities: new Dictionary<string, string>
            {
                { "EXXER001", "Error" },
                { "EXXER002", "Warning" },
                { "EXXER003", "Info" }
            },
            GlobalSettings: new Dictionary<string, object>
            {
                { "TreatWarningsAsErrors", false },
                { "EnableAutoFix", true }
            },
            LastUpdated: DateTime.UtcNow,
            Version: "1.0.0"
        );
    }

    /// <inheritdoc />
    public async Task<LintingResult> UpdatePolicyAsync(string solutionPath, LintingPolicy policy, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating linting policy for solution: {SolutionPath}", solutionPath);

        // TODO: Implement policy update functionality
        await Task.Delay(100, cancellationToken); // Placeholder

        return new LintingResult(
            Success: true,
            Violations: Enumerable.Empty<LintingViolation>(),
            Summary: new LintingSummary(0, 0, 0, 0, 0, 0, 0),
            PolicyDecisions: Enumerable.Empty<PolicyDecision>(),
            ExecutionTimeMs: 0
        );
    }

    private async Task<Solution> LoadSolutionAsync(string solutionPath, CancellationToken cancellationToken)
    {
        // TODO: Implement proper solution loading with caching
        // For now, use a simple workspace approach
        using var workspace = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(solutionPath, progress: null, cancellationToken);
        
        return solution;
    }

    private async Task<List<Document>> GetFilesToAnalyzeAsync(Solution solution, string? scope, CancellationToken cancellationToken)
    {
        var documents = new List<Document>();

        if (string.IsNullOrEmpty(scope) || scope.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            // Analyze all C# documents in the solution
            documents.AddRange(solution.Projects
                .SelectMany(p => p.Documents)
                .Where(d => d.FilePath?.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) == true));
        }
        else
        {
            // Analyze specific file or directory
            if (File.Exists(scope))
            {
                var document = solution.Projects
                    .SelectMany(p => p.Documents)
                    .FirstOrDefault(d => d.FilePath?.Equals(scope, StringComparison.OrdinalIgnoreCase) == true);
                
                if (document != null)
                {
                    documents.Add(document);
                }
            }
            else if (Directory.Exists(scope))
            {
                // Analyze all C# files in directory
                var files = Directory.GetFiles(scope, "*.cs", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var document = solution.Projects
                        .SelectMany(p => p.Documents)
                        .FirstOrDefault(d => d.FilePath?.Equals(file, StringComparison.OrdinalIgnoreCase) == true);
                    
                    if (document != null)
                    {
                        documents.Add(document);
                    }
                }
            }
        }

        return documents;
    }

    private async Task<IEnumerable<LintingViolation>> RunAnalyzersAsync(
        Solution solution, 
        List<Document> documents, 
        LintingRequest request, 
        CancellationToken cancellationToken)
    {
        var violations = new List<LintingViolation>();

        // Create analyzer compilation
        var compilation = await solution.Projects.First().GetCompilationAsync(cancellationToken);
        if (compilation == null) return violations;

        // Create analyzer - TODO: Replace with actual IndFusion analyzers
        var analyzers = ImmutableArray<DiagnosticAnalyzer>.Empty; // Placeholder for now
        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);

        // Run analysis
        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken);

        // Filter by severity
        var filteredDiagnostics = diagnostics.Where(d => 
            request.SeverityFilter.Equals("All", StringComparison.OrdinalIgnoreCase) ||
            d.Severity.ToString().Equals(request.SeverityFilter, StringComparison.OrdinalIgnoreCase));

        // Filter by rule IDs if specified
        if (request.RuleIds?.Any() == true)
        {
            filteredDiagnostics = filteredDiagnostics.Where(d => 
                request.RuleIds.Contains(d.Id, StringComparer.OrdinalIgnoreCase));
        }

        // Convert diagnostics to violations
        foreach (var diagnostic in filteredDiagnostics)
        {
            var location = diagnostic.Location;
            var filePath = location.SourceTree?.FilePath ?? "Unknown";
            var lineSpan = location.GetLineSpan();
            
            var violation = new LintingViolation(
                RuleId: diagnostic.Id,
                Severity: diagnostic.Severity.ToString(),
                Message: diagnostic.GetMessage(),
                FilePath: filePath,
                Line: lineSpan.StartLinePosition.Line + 1, // Convert to 1-based
                Column: lineSpan.StartLinePosition.Character + 1, // Convert to 1-based
                CodeSnippet: GetCodeSnippet(location),
                PolicyRecommendation: GeneratePolicyRecommendation(diagnostic),
                RemediationSuggestions: GenerateRemediationSuggestions(diagnostic),
                ConfidenceScore: 0.9 // High confidence for Roslyn analyzers
            );

            violations.Add(violation);
        }

        return violations;
    }

    private async Task<IEnumerable<PolicyDecision>> ApplyPoliciesAsync(
        IEnumerable<LintingViolation> violations, 
        LintingRequest request, 
        CancellationToken cancellationToken)
    {
        var decisions = new List<PolicyDecision>();

        // TODO: Implement actual policy application logic
        // This would involve checking policy rules, severity settings, etc.
        await Task.Delay(100, cancellationToken); // Placeholder

        foreach (var violation in violations)
        {
            decisions.Add(new PolicyDecision(
                RuleId: violation.RuleId,
                Decision: "Enforce", // Default decision
                Reason: "Standard policy enforcement",
                Timestamp: DateTime.UtcNow
            ));
        }

        return decisions;
    }

    private LintingSummary GenerateSummary(IEnumerable<LintingViolation> violations, int filesAnalyzed)
    {
        var violationList = violations.ToList();
        
        return new LintingSummary(
            TotalViolations: violationList.Count,
            ErrorCount: violationList.Count(v => v.Severity.Equals("Error", StringComparison.OrdinalIgnoreCase)),
            WarningCount: violationList.Count(v => v.Severity.Equals("Warning", StringComparison.OrdinalIgnoreCase)),
            InfoCount: violationList.Count(v => v.Severity.Equals("Info", StringComparison.OrdinalIgnoreCase)),
            HintCount: violationList.Count(v => v.Severity.Equals("Hidden", StringComparison.OrdinalIgnoreCase)),
            FilesAnalyzed: filesAnalyzed,
            RulesChecked: violationList.Select(v => v.RuleId).Distinct().Count()
        );
    }

    private string GetCodeSnippet(Location location)
    {
        try
        {
            var sourceTree = location.SourceTree;
            if (sourceTree == null) return "N/A";

            var span = location.SourceSpan;
            var text = sourceTree.GetText();
            var start = Math.Max(0, span.Start - 50);
            var end = Math.Min(text.Length, span.End + 50);
            
            return text.GetSubText(new Microsoft.CodeAnalysis.Text.TextSpan(start, end - start)).ToString();
        }
        catch
        {
            return "N/A";
        }
    }

    private PolicyRecommendation GeneratePolicyRecommendation(Diagnostic diagnostic)
    {
        // TODO: Implement intelligent policy recommendation based on rule type, context, etc.
        return new PolicyRecommendation(
            Action: "Fix",
            Reason: "Standard code quality improvement",
            Confidence: 0.8,
            AutoFixable: true,
            EstimatedEffort: "Low"
        );
    }

    private IEnumerable<RemediationSuggestion> GenerateRemediationSuggestions(Diagnostic diagnostic)
    {
        // TODO: Implement intelligent remediation suggestions based on rule type
        return new[]
        {
            new RemediationSuggestion(
                Type: "CodeFix",
                Description: $"Apply suggested fix for {diagnostic.Id}",
                CodeExample: "// TODO: Generate example code",
                Confidence: 0.8,
                Effort: "Low"
            )
        };
    }
}
