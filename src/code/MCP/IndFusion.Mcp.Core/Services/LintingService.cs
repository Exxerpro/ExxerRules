using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;
using IndFusion.Mcp.Core.Abstractions;
using System.Collections.Immutable;
using IndFusion.Analyzer;

namespace IndFusion.Mcp.Core.Services;

/// <summary>
/// Default implementation of <see cref="ILintingService"/> that runs EXXER analyzers
/// and applies policies to provide comprehensive linting results.
/// </summary>
public class LintingService : ILintingService
{
    private readonly ILogger<LintingService> _logger;
    
    // Static workspace cache to prevent duplicate solution loading
    private static readonly Dictionary<string, (MSBuildWorkspace Workspace, Solution Solution, DateTime LastAccessed)> _workspaceCache = new();
    private static readonly object _cacheLock = new();
    private static readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

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
        _logger.LogInformation("=== RUNLINTINGASYNC STARTED ===");
        _logger.LogInformation("Solution: {SolutionPath}, Scope: {Scope}", request.SolutionPath, request.Scope);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var stepStopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // Step 1: Validate inputs
            _logger.LogInformation("STEP 1.1: Validating solution file exists");
            stepStopwatch.Restart();
            
            if (!File.Exists(request.SolutionPath))
            {
                _logger.LogError("Solution file not found: {SolutionPath}", request.SolutionPath);
                throw new ArgumentException($"Solution file not found: {request.SolutionPath}", nameof(request));
            }

            stepStopwatch.Stop();
            _logger.LogInformation("STEP 1.1: Validation completed in {ElapsedMs}ms", stepStopwatch.ElapsedMilliseconds);

            // Step 2: Get or create solution
            _logger.LogInformation("STEP 1.2: Getting or creating solution with timeout (30s)");
            stepStopwatch.Restart();
            
            using var solutionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            solutionCts.CancelAfter(TimeSpan.FromSeconds(30)); // Progressive timeout for solution loading
            
            var solution = await GetOrCreateSolutionAsync(request.SolutionPath, solutionCts.Token);
            
            stepStopwatch.Stop();
            _logger.LogInformation("STEP 1.2: Solution loaded in {ElapsedMs}ms. Projects: {ProjectCount}", 
                stepStopwatch.ElapsedMilliseconds, solution.ProjectIds.Count);
            
            // Step 3: Determine files to analyze
            _logger.LogInformation("STEP 1.3: Determining files to analyze with timeout (5s)");
            stepStopwatch.Restart();
            
            using var filesCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            filesCts.CancelAfter(TimeSpan.FromSeconds(5)); // Progressive timeout for file enumeration
            
            var filesToAnalyze = await GetFilesToAnalyzeAsync(solution, request.Scope, filesCts.Token);
            
            stepStopwatch.Stop();
            _logger.LogInformation("STEP 1.3: File enumeration completed in {ElapsedMs}ms. Files: {FileCount}", 
                stepStopwatch.ElapsedMilliseconds, filesToAnalyze.Count);
            
            // Step 4: Run analyzers
            _logger.LogInformation("STEP 1.4: Running analyzers (no internal timeout - relies on test-level timeout)");
            stepStopwatch.Restart();
            
            // NOTE: No progressive timeout for analyzer execution - it can take 40-50 seconds for large solutions.
            // The test-level timeout (90s) provides sufficient protection against hangs.
            // Using cancellationToken directly without additional timeout allows full use of test timeout.
            
            var violations = await RunAnalyzersAsync(solution, filesToAnalyze, request, cancellationToken);
            
            stepStopwatch.Stop();
            _logger.LogInformation("STEP 1.4: Analyzers completed in {ElapsedMs}ms. Violations: {ViolationCount}", 
                stepStopwatch.ElapsedMilliseconds, violations.Count());
            
            // Step 5: Apply policies
            _logger.LogInformation("STEP 1.5: Applying policies with timeout (5s)");
            stepStopwatch.Restart();
            
            using var policiesCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            policiesCts.CancelAfter(TimeSpan.FromSeconds(5)); // Progressive timeout for policy application
            
            var policyDecisions = await ApplyPoliciesAsync(violations, request, policiesCts.Token);
            
            stepStopwatch.Stop();
            _logger.LogInformation("STEP 1.5: Policy application completed in {ElapsedMs}ms. Decisions: {DecisionCount}", 
                stepStopwatch.ElapsedMilliseconds, policyDecisions.Count());
            
            // Step 6: Generate summary
            _logger.LogInformation("STEP 1.6: Generating summary");
            stepStopwatch.Restart();
            
            var summary = GenerateSummary(violations, filesToAnalyze.Count);
            
            stepStopwatch.Stop();
            _logger.LogInformation("STEP 1.6: Summary generation completed in {ElapsedMs}ms", stepStopwatch.ElapsedMilliseconds);
            
            stopwatch.Stop();

            _logger.LogInformation("=== RUNLINTINGASYNC COMPLETED SUCCESSFULLY in {TotalElapsedMs}ms ===", 
                stopwatch.ElapsedMilliseconds);
            _logger.LogInformation("Total violations: {ViolationCount}", violations.Count());

            return new LintingResult(
                Success: true,
                Violations: violations,
                Summary: summary,
                PolicyDecisions: policyDecisions,
                ExecutionTimeMs: stopwatch.ElapsedMilliseconds
            );
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "=== RUNLINTINGASYNC CANCELLED after {ElapsedMs}ms ===", stopwatch.ElapsedMilliseconds);
            _logger.LogInformation("Last completed step: Check logs above for step markers");
            stopwatch.Stop();
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "=== RUNLINTINGASYNC FAILED after {ElapsedMs}ms ===", stopwatch.ElapsedMilliseconds);
            _logger.LogInformation("Last completed step: Check logs above for step markers");
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
        _logger.LogInformation("RunAnalyzersAsync: Starting analyzer execution");
        var stepStopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        cancellationToken.ThrowIfCancellationRequested();
        
        var violations = new List<LintingViolation>();

        _logger.LogInformation("RunAnalyzersAsync: STEP 1 - Getting compilation from first project");
        stepStopwatch.Restart();
        
        // Create analyzer compilation
        var firstProject = solution.Projects.FirstOrDefault();
        if (firstProject == null)
        {
            _logger.LogWarning("RunAnalyzersAsync: No projects found in solution");
            return violations;
        }
        
        _logger.LogInformation("RunAnalyzersAsync: Getting compilation for project: {ProjectName}", firstProject.Name);
        var compilation = await firstProject.GetCompilationAsync(cancellationToken);
        
        stepStopwatch.Stop();
        _logger.LogInformation("RunAnalyzersAsync: Compilation retrieved in {ElapsedMs}ms", stepStopwatch.ElapsedMilliseconds);
        
        if (compilation == null)
        {
            _logger.LogWarning("RunAnalyzersAsync: Compilation is null");
            return violations;
        }

        _logger.LogInformation("RunAnalyzersAsync: STEP 2 - Creating analyzers and compilation with analyzers");
        stepStopwatch.Restart();
        
        // Create analyzer - Load actual IndFusion analyzers
        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new IndFusionAnalyzer());
        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);

        stepStopwatch.Stop();
        _logger.LogInformation("RunAnalyzersAsync: Compilation with analyzers created in {ElapsedMs}ms", stepStopwatch.ElapsedMilliseconds);

        _logger.LogInformation("RunAnalyzersAsync: STEP 3 - Running analyzer diagnostics (this may take time)");
        stepStopwatch.Restart();
        
        // Run analysis
        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken);

        stepStopwatch.Stop();
        _logger.LogInformation("RunAnalyzersAsync: Analyzer diagnostics completed in {ElapsedMs}ms. Found {DiagnosticCount} diagnostics", 
            stepStopwatch.ElapsedMilliseconds, diagnostics.Length);

        _logger.LogInformation("RunAnalyzersAsync: STEP 4 - Filtering diagnostics by severity");
        stepStopwatch.Restart();
        
        // Filter by severity
        var filteredDiagnostics = diagnostics.Where(d => 
            request.SeverityFilter.Equals("All", StringComparison.OrdinalIgnoreCase) ||
            d.Severity.ToString().Equals(request.SeverityFilter, StringComparison.OrdinalIgnoreCase));

        stepStopwatch.Stop();
        _logger.LogInformation("RunAnalyzersAsync: Severity filtering completed in {ElapsedMs}ms. Filtered count: {Count}", 
            stepStopwatch.ElapsedMilliseconds, filteredDiagnostics.Count());

        // Filter by rule IDs if specified
        if (request.RuleIds?.Any() == true)
        {
            _logger.LogInformation("RunAnalyzersAsync: STEP 5 - Filtering by rule IDs");
            stepStopwatch.Restart();
            
            filteredDiagnostics = filteredDiagnostics.Where(d => 
                request.RuleIds.Contains(d.Id, StringComparer.OrdinalIgnoreCase));
            
            stepStopwatch.Stop();
            _logger.LogInformation("RunAnalyzersAsync: Rule ID filtering completed in {ElapsedMs}ms. Final count: {Count}", 
                stepStopwatch.ElapsedMilliseconds, filteredDiagnostics.Count());
        }

        _logger.LogInformation("RunAnalyzersAsync: STEP 6 - Converting diagnostics to violations");
        stepStopwatch.Restart();
        
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

        stepStopwatch.Stop();
        _logger.LogInformation("RunAnalyzersAsync: Violation conversion completed in {ElapsedMs}ms. Total violations: {Count}", 
            stepStopwatch.ElapsedMilliseconds, violations.Count);

        return violations;
    }

    private async Task<IEnumerable<PolicyDecision>> ApplyPoliciesAsync(
        IEnumerable<LintingViolation> violations, 
        LintingRequest request, 
        CancellationToken cancellationToken)
    {
        var decisions = new List<PolicyDecision>();
        
        // Group violations by rule ID for efficient policy application
        var violationsByRule = violations.GroupBy(v => v.RuleId);
        
        foreach (var ruleGroup in violationsByRule)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var ruleId = ruleGroup.Key;
            var ruleViolations = ruleGroup.ToList();
            
            // Apply policy rules based on rule ID and severity
            var decision = ApplyPolicyRule(ruleId, ruleViolations, request);
            decisions.Add(decision);
        }
        
        return decisions;
    }
    
    /// <summary>
    /// Applies policy rules to violations for a specific rule ID.
    /// </summary>
    private PolicyDecision ApplyPolicyRule(string ruleId, List<LintingViolation> violations, LintingRequest request)
    {
        var timestamp = DateTime.UtcNow;
        
        // Policy rules based on rule ID patterns and severity
        return ruleId switch
        {
            // Architecture rules - High priority enforcement
            var id when id.StartsWith("EXXER001") => new PolicyDecision(
                RuleId: ruleId,
                Decision: "Enforce",
                Reason: "Architecture violations must be fixed to maintain code quality",
                Timestamp: timestamp
            ),
            
            // Async rules - Critical for performance
            var id when id.StartsWith("EXXER002") => new PolicyDecision(
                RuleId: ruleId,
                Decision: "Enforce",
                Reason: "Async patterns are critical for application performance",
                Timestamp: timestamp
            ),
            
            // Error handling rules - Critical for reliability
            var id when id.StartsWith("EXXER003") => new PolicyDecision(
                RuleId: ruleId,
                Decision: "Enforce",
                Reason: "Proper error handling is essential for application reliability",
                Timestamp: timestamp
            ),
            
            // Testing rules - Important for quality
            var id when id.StartsWith("EXXER004") => new PolicyDecision(
                RuleId: ruleId,
                Decision: "Enforce",
                Reason: "Testing standards ensure code quality and maintainability",
                Timestamp: timestamp
            ),
            
            // Documentation rules - Medium priority
            var id when id.StartsWith("EXXER005") => new PolicyDecision(
                RuleId: ruleId,
                Decision: violations.Count > 10 ? "Enforce" : "Suppress",
                Reason: violations.Count > 10 
                    ? "Too many documentation violations, enforce standards"
                    : "Minor documentation issues, suppress for now",
                Timestamp: timestamp
            ),
            
            // Code quality rules - Enforce based on severity
            var id when id.StartsWith("EXXER006") => new PolicyDecision(
                RuleId: ruleId,
                Decision: violations.Any(v => v.Severity == "Error") ? "Enforce" : "Custom",
                Reason: violations.Any(v => v.Severity == "Error")
                    ? "Error-level violations must be fixed"
                    : "Warning-level violations can be addressed incrementally",
                Timestamp: timestamp
            ),
            
            // Modern C# rules - Encourage adoption
            var id when id.StartsWith("EXXER007") => new PolicyDecision(
                RuleId: ruleId,
                Decision: "Custom",
                Reason: "Modern C# patterns improve code readability and performance",
                Timestamp: timestamp
            ),
            
            // Performance rules - High priority
            var id when id.StartsWith("EXXER008") => new PolicyDecision(
                RuleId: ruleId,
                Decision: "Enforce",
                Reason: "Performance issues can impact user experience",
                Timestamp: timestamp
            ),
            
            // Logging rules - Important for debugging
            var id when id.StartsWith("EXXER009") => new PolicyDecision(
                RuleId: ruleId,
                Decision: "Enforce",
                Reason: "Proper logging is essential for debugging and monitoring",
                Timestamp: timestamp
            ),
            
            // Null safety rules - Critical for reliability
            var id when id.StartsWith("EXXER010") => new PolicyDecision(
                RuleId: ruleId,
                Decision: "Enforce",
                Reason: "Null safety prevents runtime exceptions",
                Timestamp: timestamp
            ),
            
            // Default policy for unknown rules
            _ => new PolicyDecision(
                RuleId: ruleId,
                Decision: violations.Count > 5 ? "Enforce" : "Custom",
                Reason: violations.Count > 5 
                    ? "Multiple violations detected, enforce policy"
                    : "Custom handling for unknown rule",
                Timestamp: timestamp
            )
        };
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
        var ruleId = diagnostic.Id;
        var severity = diagnostic.Severity.ToString();
        
        // Generate context-aware recommendations based on rule ID patterns
        return ruleId switch
        {
            // Architecture rules - Critical for maintainability
            var id when id.StartsWith("EXXER001") => new PolicyRecommendation(
                Action: "Fix",
                Reason: "Architecture violations compromise code maintainability and testability",
                Confidence: 0.95,
                AutoFixable: false, // Architecture changes require manual review
                EstimatedEffort: "High"
            ),
            
            // Async rules - Critical for performance
            var id when id.StartsWith("EXXER002") => new PolicyRecommendation(
                Action: "Fix",
                Reason: "Async patterns are essential for scalable applications",
                Confidence: 0.9,
                AutoFixable: true, // Many async patterns can be auto-fixed
                EstimatedEffort: "Medium"
            ),
            
            // Error handling rules - Critical for reliability
            var id when id.StartsWith("EXXER003") => new PolicyRecommendation(
                Action: "Fix",
                Reason: "Proper error handling prevents application crashes",
                Confidence: 0.95,
                AutoFixable: false, // Error handling requires careful consideration
                EstimatedEffort: "High"
            ),
            
            // Testing rules - Important for quality
            var id when id.StartsWith("EXXER004") => new PolicyRecommendation(
                Action: "Fix",
                Reason: "Testing standards ensure code reliability and maintainability",
                Confidence: 0.85,
                AutoFixable: true, // Test patterns can often be auto-fixed
                EstimatedEffort: "Medium"
            ),
            
            // Documentation rules - Important for maintainability
            var id when id.StartsWith("EXXER005") => new PolicyRecommendation(
                Action: severity == "Error" ? "Fix" : "Suppress",
                Reason: severity == "Error" 
                    ? "Missing documentation for public APIs is critical"
                    : "Documentation improves code maintainability",
                Confidence: 0.7,
                AutoFixable: false, // Documentation requires human input
                EstimatedEffort: "Low"
            ),
            
            // Code quality rules - Important for readability
            var id when id.StartsWith("EXXER006") => new PolicyRecommendation(
                Action: severity == "Error" ? "Fix" : "Custom",
                Reason: severity == "Error"
                    ? "Code quality errors must be addressed"
                    : "Code quality improvements enhance readability",
                Confidence: 0.8,
                AutoFixable: true, // Many code quality issues can be auto-fixed
                EstimatedEffort: "Low"
            ),
            
            // Modern C# rules - Performance and readability
            var id when id.StartsWith("EXXER007") => new PolicyRecommendation(
                Action: "Custom",
                Reason: "Modern C# patterns improve performance and code clarity",
                Confidence: 0.75,
                AutoFixable: true, // Modern patterns can often be auto-applied
                EstimatedEffort: "Low"
            ),
            
            // Performance rules - Critical for user experience
            var id when id.StartsWith("EXXER008") => new PolicyRecommendation(
                Action: "Fix",
                Reason: "Performance issues directly impact user experience",
                Confidence: 0.9,
                AutoFixable: false, // Performance fixes require analysis
                EstimatedEffort: "High"
            ),
            
            // Logging rules - Important for debugging
            var id when id.StartsWith("EXXER009") => new PolicyRecommendation(
                Action: "Fix",
                Reason: "Proper logging is essential for debugging and monitoring",
                Confidence: 0.85,
                AutoFixable: true, // Logging patterns can be standardized
                EstimatedEffort: "Low"
            ),
            
            // Null safety rules - Critical for reliability
            var id when id.StartsWith("EXXER010") => new PolicyRecommendation(
                Action: "Fix",
                Reason: "Null safety prevents runtime exceptions and improves reliability",
                Confidence: 0.95,
                AutoFixable: true, // Null safety can often be auto-applied
                EstimatedEffort: "Medium"
            ),
            
            // Default recommendation for unknown rules
            _ => new PolicyRecommendation(
                Action: severity == "Error" ? "Fix" : "Custom",
                Reason: severity == "Error"
                    ? "Error-level violations should be addressed"
                    : "Review violation and determine appropriate action",
                Confidence: 0.6,
                AutoFixable: false, // Unknown rules require manual review
                EstimatedEffort: "Medium"
            )
        };
    }

    private IEnumerable<RemediationSuggestion> GenerateRemediationSuggestions(Diagnostic diagnostic)
    {
        var ruleId = diagnostic.Id;
        var severity = diagnostic.Severity.ToString();
        
        // Generate rule-specific remediation suggestions with code examples
        return ruleId switch
        {
            // Architecture rules - Repository pattern suggestions
            var id when id.StartsWith("EXXER001") => new[]
            {
                new RemediationSuggestion(
                    Type: "Refactor",
                    Description: "Implement Repository pattern with focused interfaces",
                    CodeExample: """
                        // Before: Direct DbContext usage
                        public class UserService
                        {
                            private readonly DbContext _context;
                            public UserService(DbContext context) => _context = context;
                        }
                        
                        // After: Repository pattern
                        public interface IUserRepository
                        {
                            Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);
                        }
                        
                        public class UserService
                        {
                            private readonly IUserRepository _userRepository;
                            public UserService(IUserRepository userRepository) => _userRepository = userRepository;
                        }
                        """,
                    Confidence: 0.9,
                    Effort: "High"
                )
            },
            
            // Async rules - Cancellation token suggestions
            var id when id.StartsWith("EXXER002") => new[]
            {
                new RemediationSuggestion(
                    Type: "CodeFix",
                    Description: "Add CancellationToken parameter to async methods",
                    CodeExample: """
                        // Before
                        public async Task<string> GetDataAsync()
                        {
                            return await httpClient.GetStringAsync("https://api.example.com");
                        }
                        
                        // After
                        public async Task<string> GetDataAsync(CancellationToken cancellationToken = default)
                        {
                            return await httpClient.GetStringAsync("https://api.example.com", cancellationToken);
                        }
                        """,
                    Confidence: 0.95,
                    Effort: "Low"
                )
            },
            
            // Error handling rules - Result pattern suggestions
            var id when id.StartsWith("EXXER003") => new[]
            {
                new RemediationSuggestion(
                    Type: "Refactor",
                    Description: "Replace exceptions with Result pattern for better error handling",
                    CodeExample: """
                        // Before: Throwing exceptions
                        public User GetUser(int id)
                        {
                            if (id <= 0) throw new ArgumentException("Invalid ID");
                            return _repository.GetUser(id) ?? throw new UserNotFoundException();
                        }
                        
                        // After: Result pattern
                        public Result<User> GetUser(int id)
                        {
                            if (id <= 0) return Result<User>.WithFailure("Invalid ID");
                            var user = _repository.GetUser(id);
                            return user != null ? Result<User>.Success(user) : Result<User>.WithFailure("User not found");
                        }
                        """,
                    Confidence: 0.9,
                    Effort: "High"
                )
            },
            
            // Testing rules - XUnit v3 suggestions
            var id when id.StartsWith("EXXER004") => new[]
            {
                new RemediationSuggestion(
                    Type: "CodeFix",
                    Description: "Use XUnit v3 with Shouldly assertions instead of FluentAssertions",
                    CodeExample: """
                        // Before: FluentAssertions
                        result.Should().NotBeNull();
                        result.Name.Should().Be("Expected");
                        
                        // After: Shouldly
                        result.ShouldNotBeNull();
                        result.Name.ShouldBe("Expected");
                        """,
                    Confidence: 0.85,
                    Effort: "Low"
                )
            },
            
            // Documentation rules - XML documentation suggestions
            var id when id.StartsWith("EXXER005") => new[]
            {
                new RemediationSuggestion(
                    Type: "Documentation",
                    Description: "Add XML documentation comments to public members",
                    CodeExample: """
                        // Before
                        public class UserService
                        {
                            public async Task<User> CreateUserAsync(string name) { }
                        }
                        
                        // After
                        /// <summary>
                        /// Service for managing user operations.
                        /// </summary>
                        public class UserService
                        {
                            /// <summary>
                            /// Creates a new user with the specified name.
                            /// </summary>
                            /// <param name="name">The name of the user to create.</param>
                            /// <returns>A task representing the created user.</returns>
                            public async Task<User> CreateUserAsync(string name) { }
                        }
                        """,
                    Confidence: 0.8,
                    Effort: "Low"
                )
            },
            
            // Code quality rules - Modern C# suggestions
            var id when id.StartsWith("EXXER006") => new[]
            {
                new RemediationSuggestion(
                    Type: "CodeFix",
                    Description: "Use modern C# patterns for better readability",
                    CodeExample: """
                        // Before: Traditional patterns
                        if (user != null)
                        {
                            return user.Name;
                        }
                        return "Unknown";
                        
                        // After: Modern patterns
                        return user?.Name ?? "Unknown";
                        """,
                    Confidence: 0.8,
                    Effort: "Low"
                )
            },
            
            // Modern C# rules - Pattern matching suggestions
            var id when id.StartsWith("EXXER007") => new[]
            {
                new RemediationSuggestion(
                    Type: "CodeFix",
                    Description: "Use pattern matching for cleaner code",
                    CodeExample: """
                        // Before: Traditional switch
                        switch (shape)
                        {
                            case Circle c:
                                return Math.PI * c.Radius * c.Radius;
                            case Rectangle r:
                                return r.Width * r.Height;
                            default:
                                return 0;
                        }
                        
                        // After: Pattern matching
                        return shape switch
                        {
                            Circle c => Math.PI * c.Radius * c.Radius,
                            Rectangle r => r.Width * r.Height,
                            _ => 0
                        };
                        """,
                    Confidence: 0.85,
                    Effort: "Low"
                )
            },
            
            // Performance rules - LINQ optimization suggestions
            var id when id.StartsWith("EXXER008") => new[]
            {
                new RemediationSuggestion(
                    Type: "Refactor",
                    Description: "Optimize LINQ queries for better performance",
                    CodeExample: """
                        // Before: Multiple enumerations
                        var users = GetUsers();
                        var activeUsers = users.Where(u => u.IsActive);
                        var count = activeUsers.Count();
                        var names = activeUsers.Select(u => u.Name).ToList();
                        
                        // After: Single enumeration
                        var users = GetUsers();
                        var activeUsers = users.Where(u => u.IsActive).ToList();
                        var count = activeUsers.Count;
                        var names = activeUsers.Select(u => u.Name).ToList();
                        """,
                    Confidence: 0.8,
                    Effort: "Medium"
                )
            },
            
            // Logging rules - Structured logging suggestions
            var id when id.StartsWith("EXXER009") => new[]
            {
                new RemediationSuggestion(
                    Type: "CodeFix",
                    Description: "Use structured logging instead of string concatenation",
                    CodeExample: """
                        // Before: String concatenation
                        _logger.LogInformation("User " + userId + " performed action " + action);
                        
                        // After: Structured logging
                        _logger.LogInformation("User {UserId} performed action {Action}", userId, action);
                        """,
                    Confidence: 0.9,
                    Effort: "Low"
                )
            },
            
            // Null safety rules - Nullable reference types suggestions
            var id when id.StartsWith("EXXER010") => new[]
            {
                new RemediationSuggestion(
                    Type: "CodeFix",
                    Description: "Use nullable reference types for better null safety",
                    CodeExample: """
                        // Before: No null safety
                        public string GetUserName(User user)
                        {
                            return user.Name; // Could be null
                        }
                        
                        // After: Nullable reference types
                        public string? GetUserName(User? user)
                        {
                            return user?.Name;
                        }
                        """,
                    Confidence: 0.9,
                    Effort: "Medium"
                )
            },
            
            // Default suggestions for unknown rules
            _ => new[]
            {
                new RemediationSuggestion(
                    Type: "CodeFix",
                    Description: $"Review and fix {ruleId} violation",
                    CodeExample: "// TODO: Implement appropriate fix for this rule",
                    Confidence: 0.6,
                    Effort: "Medium"
                )
            }
        };
    }
    
    /// <summary>
    /// Gets or creates a cached solution for the given path.
    /// </summary>
    private async Task<Solution> GetOrCreateSolutionAsync(string solutionPath, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetOrCreateSolutionAsync: Starting solution retrieval");
        var stepStopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation("GetOrCreateSolutionAsync: STEP 1 - Checking cache");
        stepStopwatch.Restart();
        
        Solution? cachedSolution = null;
        lock (_cacheLock)
        {
            // Check if we have a valid cached solution
            if (_workspaceCache.TryGetValue(solutionPath, out var cachedEntry))
            {
                var (workspace, solution, lastAccessed) = cachedEntry;
                
                _logger.LogInformation("GetOrCreateSolutionAsync: Found cached solution. Last accessed: {LastAccessed}, Age: {Age}ms", 
                    lastAccessed, (DateTime.UtcNow - lastAccessed).TotalMilliseconds);
                
                // Check if cache is still valid
                if (DateTime.UtcNow - lastAccessed < _cacheExpiration)
                {
                    _logger.LogInformation("GetOrCreateSolutionAsync: Cache valid, returning cached solution");
                    // Update last accessed time
                    _workspaceCache[solutionPath] = (workspace, solution, DateTime.UtcNow);
                    cachedSolution = solution;
                }
                else
                {
                    _logger.LogInformation("GetOrCreateSolutionAsync: Cache expired, removing entry");
                    // Cache expired, remove it
                    _workspaceCache.Remove(solutionPath);
                    workspace.Dispose();
                }
            }
            else
            {
                _logger.LogInformation("GetOrCreateSolutionAsync: No cached solution found");
            }
        }
        
        if (cachedSolution != null)
        {
            stepStopwatch.Stop();
            _logger.LogInformation("GetOrCreateSolutionAsync: Returning cached solution in {ElapsedMs}ms", stepStopwatch.ElapsedMilliseconds);
            return cachedSolution;
        }
        
        _logger.LogInformation("GetOrCreateSolutionAsync: STEP 2 - Creating new workspace");
        stepStopwatch.Restart();
        
        // Create new workspace and solution
        var newWorkspace = MSBuildWorkspace.Create();
        
        stepStopwatch.Stop();
        _logger.LogInformation("GetOrCreateSolutionAsync: Workspace created in {ElapsedMs}ms", stepStopwatch.ElapsedMilliseconds);
        
        _logger.LogInformation("GetOrCreateSolutionAsync: STEP 3 - Opening solution (this may take time)");
        stepStopwatch.Restart();
        
        var newSolution = await newWorkspace.OpenSolutionAsync(solutionPath, progress: null, cancellationToken);
        
        stepStopwatch.Stop();
        _logger.LogInformation("GetOrCreateSolutionAsync: Solution opened in {ElapsedMs}ms. Projects: {ProjectCount}", 
            stepStopwatch.ElapsedMilliseconds, newSolution.ProjectIds.Count);
        
        _logger.LogInformation("GetOrCreateSolutionAsync: STEP 4 - Caching solution");
        stepStopwatch.Restart();
        
        lock (_cacheLock)
        {
            // Cache the new solution
            _workspaceCache[solutionPath] = (newWorkspace, newSolution, DateTime.UtcNow);
            
            // Clean up expired entries
            CleanupExpiredEntries();
        }
        
        stepStopwatch.Stop();
        _logger.LogInformation("GetOrCreateSolutionAsync: Solution cached in {ElapsedMs}ms", stepStopwatch.ElapsedMilliseconds);
        
        return newSolution;
    }
    
    /// <summary>
    /// Cleans up expired workspace cache entries.
    /// </summary>
    private static void CleanupExpiredEntries()
    {
        var expiredKeys = new List<string>();
        
        foreach (var kvp in _workspaceCache)
        {
            if (DateTime.UtcNow - kvp.Value.LastAccessed >= _cacheExpiration)
            {
                expiredKeys.Add(kvp.Key);
            }
        }
        
        foreach (var key in expiredKeys)
        {
            if (_workspaceCache.TryGetValue(key, out var entry))
            {
                entry.Workspace.Dispose();
                _workspaceCache.Remove(key);
            }
        }
    }
}
