using IndFusion.Mcp.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace IndFusion.Mcp.Tests.Integration.Contracts;

/// <summary>
/// Contract tests for ICodeTransformationService interface.
/// These tests validate that any implementation of ICodeTransformationService fulfills its contract.
/// </summary>
public class CodeTransformationServiceContractTests : ServiceContractTestBase<ICodeTransformationService, CodeTransformationServiceStub>
{
    public CodeTransformationServiceContractTests(ITestOutputHelper testOutput) : base(testOutput)
    {
    }

    protected override void ConfigureServiceDependencies(IServiceCollection services)
    {
        services.AddLogging();
        services.AddScoped<ILogger<CodeTransformationServiceStub>>();
    }

    [Fact]
    public async Task ApplyFixer001Async_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: new[] { "src/Example.cs", "src/Test.cs" },
            ValidationOptions: new TransformationValidationOptions(
                RunAnalyzers: true,
                BuildValidation: true,
                CheckForNewIssues: true,
                TimeoutMs: 60000,
                SeverityThreshold: "Warning"
            ),
            DryRun: false,
            BackupOriginal: true,
            MaxFixesPerFile: 5
        );

        // Act
        var result = await Service.ApplyFixer001Async(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Code transformation result should not be null");
        result.Success.ShouldBeTrue("Fixer001 application should succeed");
        result.TransformationDetails.ShouldNotBeNull("Transformation details should not be null");
        result.ValidationResults.ShouldNotBeNull("Validation results should not be null");
        result.ModifiedFiles.ShouldNotBeNull("Modified files should not be null");
        result.ExecutionTimeMs.ShouldBeGreaterThan(0, "Execution time should be positive");
        
        Logger.LogInformation("ApplyFixer001Async contract validation passed");
    }

    [Fact]
    public async Task ApplySafeRegexAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new SafeRegexRequest(
            Pattern: @"public\s+void\s+(\w+)\s*\(",
            Replacement: "public async Task $1Async(",
            TargetFiles: new[] { "src/Example.cs" },
            ValidationOptions: new TransformationValidationOptions(
                RunAnalyzers: true,
                BuildValidation: true,
                CheckForNewIssues: true,
                TimeoutMs: 60000,
                SeverityThreshold: "Error"
            ),
            DryRun: true,
            CaseSensitive: true,
            Multiline: false
        );

        // Act
        var result = await Service.ApplySafeRegexAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Safe regex result should not be null");
        result.Success.ShouldBeTrue("Safe regex application should succeed");
        result.TransformationDetails.ShouldNotBeNull("Transformation details should not be null");
        result.ValidationResults.ShouldNotBeNull("Validation results should not be null");
        result.DiffPreview.ShouldNotBeNullOrEmpty("Diff preview should be provided");
        result.ExecutionTimeMs.ShouldBeGreaterThan(0, "Execution time should be positive");
        
        Logger.LogInformation("ApplySafeRegexAsync contract validation passed");
    }

    [Fact]
    public async Task ValidateTransformationAsync_WithValidRequest_ShouldReturnValidationResult()
    {
        // Arrange
        var request = new TransformationValidationRequest(
            OriginalCode: "public void Method() { }",
            TransformedCode: "public async Task MethodAsync() { }",
            ValidationCriteria: new ValidationCriteria(
                MaxNewIssues: 0,
                SeverityThreshold: "Error",
                RequiredChecks: new[] { "Build", "Analyzer" },
                CustomRules: new Dictionary<string, object>()
            ),
            RunAnalyzers: true,
            BuildValidation: true,
            CheckForNewIssues: true
        );

        // Act
        var result = await Service.ValidateTransformationAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Validation result should not be null");
        result.IsValid.ShouldBeTrue("Transformation should be valid");
        result.ValidationChecks.ShouldNotBeNull("Validation checks should not be null");
        result.NewIssues.ShouldNotBeNull("New issues should not be null");
        result.BuildSuccess.ShouldBeTrue("Build should succeed");
        result.AnalyzerResults.ShouldNotBeNull("Analyzer results should not be null");
        result.ValidationTimeMs.ShouldBeGreaterThan(0, "Validation time should be positive");
        
        Logger.LogInformation("ValidateTransformationAsync contract validation passed");
    }

    [Fact]
    public async Task ReviewSemanticChangesAsync_WithValidRequest_ShouldReturnReviewResult()
    {
        // Arrange
        var request = new SemanticChangeReviewRequest(
            OriginalCode: "public class Example { public void Method() { } }",
            ModifiedCode: "public class Example { public async Task MethodAsync() { } }",
            ReviewOptions: new ChangeReviewOptions(
                IncludeMetrics: true,
                CheckPerformance: true,
                CheckSecurity: true,
                CheckMaintainability: true
            ),
            IncludeDiff: true,
            CheckSemanticDrift: true
        );

        // Act
        var result = await Service.ReviewSemanticChangesAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Semantic change review result should not be null");
        result.Success.ShouldBeTrue("Semantic change review should succeed");
        result.SemanticDiff.ShouldNotBeNull("Semantic diff should not be null");
        result.DriftAnalysis.ShouldNotBeNull("Drift analysis should not be null");
        result.FixSuggestions.ShouldNotBeNull("Fix suggestions should not be null");
        result.ConfidenceScore.ShouldBeGreaterThanOrEqualTo(0.0, "Confidence score should be non-negative");
        result.ConfidenceScore.ShouldBeLessThanOrEqualTo(1.0, "Confidence score should not exceed 1.0");
        result.ReviewTimeMs.ShouldBeGreaterThan(0, "Review time should be positive");
        
        Logger.LogInformation("ReviewSemanticChangesAsync contract validation passed");
    }

    [Fact]
    public async Task GetFixer001ConfigurationAsync_WithValidSolutionPath_ShouldReturnConfiguration()
    {
        // Arrange
        var solutionPath = "C:\\Test\\TestSolution.sln";

        // Act
        var configuration = await Service.GetFixer001ConfigurationAsync(solutionPath, CreateTestCancellationToken());

        // Assert
        configuration.ShouldNotBeNull("Fixer001 configuration should not be null");
        configuration.SolutionPath.ShouldBe(solutionPath, "Configuration should be for the correct solution");
        configuration.AvailableTransformations.ShouldNotBeNull("Available transformations should not be null");
        configuration.DefaultSettings.ShouldNotBeNull("Default settings should not be null");
        configuration.Version.ShouldNotBeNullOrEmpty("Version should not be null or empty");
        configuration.LastUpdated.ShouldBeLessThanOrEqualTo(DateTime.UtcNow, "Last updated should be in the past");
        
        Logger.LogInformation("GetFixer001ConfigurationAsync contract validation passed");
    }

    public override async Task Service_ShouldHandleCancellationTokens()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var fixerRequest = new Fixer001Request("EXXER001", new[] { "test.cs" }, new TransformationValidationOptions());
        var regexRequest = new SafeRegexRequest("pattern", "replacement", new[] { "test.cs" }, new TransformationValidationOptions());
        var validationRequest = new TransformationValidationRequest("original", "transformed", new ValidationCriteria());
        var reviewRequest = new SemanticChangeReviewRequest("original", "modified", new ChangeReviewOptions());

        // All methods should handle cancellation gracefully
        await Should.NotThrowAsync(async () => await Service.ApplyFixer001Async(fixerRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.ApplySafeRegexAsync(regexRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.ValidateTransformationAsync(validationRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.ReviewSemanticChangesAsync(reviewRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.GetFixer001ConfigurationAsync("test.sln", cts.Token));

        Logger.LogInformation("Cancellation token handling validation passed for all methods");
    }

    public override async Task Service_ShouldHandleNullParameters()
    {
        var fixerRequest = new Fixer001Request("EXXER001", new[] { "test.cs" }, new TransformationValidationOptions());
        var regexRequest = new SafeRegexRequest("pattern", "replacement", new[] { "test.cs" }, new TransformationValidationOptions());
        var validationRequest = new TransformationValidationRequest("original", "transformed", new ValidationCriteria());
        var reviewRequest = new SemanticChangeReviewRequest("original", "modified", new ChangeReviewOptions());

        // Methods should handle null parameters gracefully
        await Should.NotThrowAsync(async () => await Service.ApplyFixer001Async(fixerRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.ApplySafeRegexAsync(regexRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.ValidateTransformationAsync(validationRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.ReviewSemanticChangesAsync(reviewRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.GetFixer001ConfigurationAsync("test.sln", CancellationToken.None));

        Logger.LogInformation("Null parameter handling validation passed");
    }
}

/// <summary>
/// Stub implementation of ICodeTransformationService for contract testing.
/// </summary>
public class CodeTransformationServiceStub : ICodeTransformationService
{
    private readonly ILogger<CodeTransformationServiceStub> _logger;

    public CodeTransformationServiceStub(ILogger<CodeTransformationServiceStub> logger)
    {
        _logger = logger;
    }

    public async Task<CodeTransformationResult> ApplyFixer001Async(Fixer001Request request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Applying Fixer001 for diagnostic {DiagnosticId}", request.DiagnosticId);
        
        await Task.Delay(200, cancellationToken);
        
        return new CodeTransformationResult(
            Success: true,
            TransformationDetails: new TransformationDetails(
                TransformationType: "Fixer001",
                TransformationId: "fixer-001",
                Description: "Applied Fixer001 transformation",
                ChangesApplied: 3,
                FilesAffected: request.TargetFiles.Count(),
                Confidence: 0.95
            ),
            ValidationResults: new[]
            {
                new ValidationResult(
                    CheckName: "Build Validation",
                    Passed: true,
                    Message: "Build completed successfully",
                    Details: new Dictionary<string, object>()
                ),
                new ValidationResult(
                    CheckName: "Analyzer Validation",
                    Passed: true,
                    Message: "No new analyzer issues found",
                    Details: new Dictionary<string, object>()
                )
            },
            DiffPreview: "// Changes applied by Fixer001\n+ public async Task MethodAsync()\n- public void Method()",
            ModifiedFiles: request.TargetFiles,
            ExecutionTimeMs: 200
        );
    }

    public async Task<CodeTransformationResult> ApplySafeRegexAsync(SafeRegexRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Applying safe regex transformation with pattern: {Pattern}", request.Pattern);
        
        await Task.Delay(150, cancellationToken);
        
        return new CodeTransformationResult(
            Success: true,
            TransformationDetails: new TransformationDetails(
                TransformationType: "SafeRegex",
                TransformationId: "regex-001",
                Description: "Applied safe regex transformation",
                ChangesApplied: 2,
                FilesAffected: request.TargetFiles.Count(),
                Confidence: 0.9
            ),
            ValidationResults: new[]
            {
                new ValidationResult(
                    CheckName: "Regex Validation",
                    Passed: true,
                    Message: "Regex transformation applied safely",
                    Details: new Dictionary<string, object>()
                )
            },
            DiffPreview: "// Regex transformation applied\n+ public async Task MethodAsync()\n- public void Method()",
            ModifiedFiles: request.TargetFiles,
            ExecutionTimeMs: 150
        );
    }

    public async Task<TransformationValidationResult> ValidateTransformationAsync(TransformationValidationRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Validating transformation");
        
        await Task.Delay(100, cancellationToken);
        
        return new TransformationValidationResult(
            IsValid: true,
            ValidationChecks: new[]
            {
                new ValidationCheck(
                    CheckType: "Build",
                    Status: "Pass",
                    Message: "Code builds successfully",
                    Severity: "Info"
                ),
                new ValidationCheck(
                    CheckType: "Analyzer",
                    Status: "Pass",
                    Message: "No analyzer issues found",
                    Severity: "Info"
                )
            },
            NewIssues: new List<TransformationIssue>(),
            BuildSuccess: true,
            AnalyzerResults: new[]
            {
                new AnalyzerResult(
                    AnalyzerName: "EXXER001",
                    IssuesFound: 0,
                    SeverityDistribution: new Dictionary<string, int>(),
                    ExecutionTimeMs: 50
                )
            },
            ValidationTimeMs: 100
        );
    }

    public async Task<SemanticChangeReviewResult> ReviewSemanticChangesAsync(SemanticChangeReviewRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Reviewing semantic changes");
        
        await Task.Delay(250, cancellationToken);
        
        return new SemanticChangeReviewResult(
            Success: true,
            SemanticDiff: new SemanticDiffAnalysis(
                StructuralChanges: new[]
                {
                    new StructuralChange(
                        ChangeType: "Method Signature",
                        ElementName: "Method",
                        ChangeDescription: "Method signature changed from void to Task",
                        Impact: "Low"
                    )
                },
                BehavioralChanges: new[]
                {
                    new BehavioralChange(
                        BehaviorType: "Asynchronous",
                        ChangeDescription: "Method is now asynchronous",
                        Impact: "Medium",
                        Confidence: 0.9
                    )
                },
                ImpactAnalysis: new ImpactAnalysis(
                    OverallImpact: "Low",
                    AffectedComponents: new[] { "Method", "Callers" },
                    RiskLevel: "Low",
                    MitigationStrategies: new[] { "Update callers to await", "Add async/await pattern" }
                ),
                ConfidenceScore: 0.9
            ),
            DriftAnalysis: new DriftAnalysis(
                DriftDetected: false,
                DriftType: "None",
                DriftSeverity: "None",
                AffectedAreas: new List<string>(),
                Recommendations: new List<string>()
            ),
            FixSuggestions: new[]
            {
                new FixSuggestion(
                    SuggestionId: "suggestion-001",
                    SuggestionType: "Async Pattern",
                    Description: "Update callers to use async/await pattern",
                    CodeExample: "await MethodAsync();",
                    Confidence: 0.95,
                    Effort: "Low"
                )
            },
            ConfidenceScore: 0.9,
            ReviewTimeMs: 250
        );
    }

    public async Task<Fixer001Configuration> GetFixer001ConfigurationAsync(string solutionPath, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting Fixer001 configuration for solution: {SolutionPath}", solutionPath);
        
        await Task.Delay(50, cancellationToken);
        
        return new Fixer001Configuration(
            SolutionPath: solutionPath,
            AvailableTransformations: new[]
            {
                new TransformationInfo(
                    Id: "transformation-001",
                    Name: "Null Safety",
                    Description: "Adds null safety checks",
                    SupportedLanguages: new[] { "C#" },
                    IsEnabled: true,
                    Parameters: new Dictionary<string, object> { { "severity", "Warning" } }
                ),
                new TransformationInfo(
                    Id: "transformation-002",
                    Name: "Async Pattern",
                    Description: "Converts synchronous methods to asynchronous",
                    SupportedLanguages: new[] { "C#" },
                    IsEnabled: true,
                    Parameters: new Dictionary<string, object> { { "timeout", 30000 } }
                )
            },
            DefaultSettings: new Dictionary<string, object>
            {
                { "dryRun", false },
                { "backupOriginal", true },
                { "maxFixesPerFile", 10 }
            },
            Version: "1.0.0",
            LastUpdated: DateTime.UtcNow
        );
    }
}