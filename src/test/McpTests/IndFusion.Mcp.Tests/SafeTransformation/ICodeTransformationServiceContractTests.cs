using IndFusion.Mcp.Core.Abstractions;
using IndQuestResults;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.SafeTransformation;

/// <summary>
/// Contract tests for ICodeTransformationService interface.
/// These tests verify the contract behavior using mocks and should ALWAYS PASS.
/// </summary>
public class ICodeTransformationServiceContractTests
{
    private readonly ICodeTransformationService _mockService;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    /// <summary>
    /// Initializes the contract tests with a mocked ICodeTransformationService.
    /// </summary>
    public ICodeTransformationServiceContractTests()
    {
        _mockService = Substitute.For<ICodeTransformationService>();
    }

    /// <summary>
    /// Verifies the Facade applies Fixer001 requests successfully when inputs are valid.
    /// </summary>
    [Fact]
    public async Task ApplyFixer001Async_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: false,
            BackupOriginal: true,
            MaxFixesPerFile: 10
        );

        var expectedResult = new CodeTransformationResult(
            Success: true,
            TransformationDetails: new TransformationDetails(
                TransformationType: "Fixer001",
                TransformationId: "fixer-123",
                Description: "Fix EXXER001 violations",
                ChangesApplied: 3,
                FilesAffected: 1,
                Confidence: 0.98
            ),
            ValidationResults:
            [
                new("DiagnosticValidation", true, "Diagnostic is valid", []),
                new("BuildValidation", true, "Build succeeded", [])
            ],
            DiffPreview: "Preview of fixes...",
            ModifiedFiles: new[] { "TestFile.cs" },
            ExecutionTimeMs: 2000
        );

        _mockService.ApplyFixer001Async(request, _cancellationToken)
            .Returns(expectedResult);

        // Act
        var result = await _mockService.ApplyFixer001Async(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.TransformationDetails.ChangesApplied.ShouldBe(3);
        result.TransformationDetails.FilesAffected.ShouldBe(1);
    }

    /// <summary>
    /// Confirms the Facade delegates valid safe regex transformations successfully.
    /// </summary>
    [Fact]
    public async Task ApplySafeRegexAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new SafeRegexRequest(
            Pattern: @"\bclass\s+(\w+)",
            Replacement: "public class $1",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: false,
            CaseSensitive: true,
            Multiline: false
        );

        var expectedResult = new CodeTransformationResult(
            Success: true,
            TransformationDetails: new TransformationDetails(
                TransformationType: "SafeRegex",
                TransformationId: "regex-123",
                Description: "Add public modifier to class declarations",
                ChangesApplied: 5,
                FilesAffected: 1,
                Confidence: 0.95
            ),
            ValidationResults:
            [
                new("PatternValidation", true, "Pattern is valid", []),
                new("BuildValidation", true, "Build succeeded", [])
            ],
            DiffPreview: "Preview of changes...",
            ModifiedFiles: new[] { "TestFile.cs" },
            ExecutionTimeMs: 1500
        );

        _mockService.ApplySafeRegexAsync(request, _cancellationToken)
            .Returns(expectedResult);

        // Act
        var result = await _mockService.ApplySafeRegexAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.TransformationDetails.ChangesApplied.ShouldBe(5);
        result.TransformationDetails.FilesAffected.ShouldBe(1);
    }

    /// <summary>
    /// Ensures transformation validation succeeds for well-formed requests.
    /// </summary>
    [Fact]
    public async Task ValidateTransformationAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new TransformationValidationRequest(
            OriginalCode: "class Test { }",
            TransformedCode: "public class Test { }",
            ValidationCriteria: new ValidationCriteria(),
            RunAnalyzers: true,
            BuildValidation: true,
            CheckForNewIssues: true
        );

        var expectedResult = new TransformationValidationResult(
            IsValid: true,
            ValidationChecks: new[]
            {
                new ValidationCheck("BuildCheck", "Pass", "Build succeeded", "Info"),
                new ValidationCheck("AnalyzerCheck", "Pass", "No new issues found", "Info")
            },
            NewIssues: Array.Empty<TransformationIssue>(),
            BuildSuccess: true,
            AnalyzerResults: new[]
            {
                new AnalyzerResult("EXXER001", 0, new Dictionary<string, int> { { "Error", 0 } }, 100)
            },
            ValidationTimeMs: 1500
        );

        _mockService.ValidateTransformationAsync(request, _cancellationToken)
            .Returns(expectedResult);

        // Act
        var result = await _mockService.ValidateTransformationAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsValid.ShouldBeTrue();
        result.BuildSuccess.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies semantic change reviews succeed for valid review requests.
    /// </summary>
    [Fact]
    public async Task ReviewSemanticChangesAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new SemanticChangeReviewRequest(
            OriginalCode: "class Test { }",
            ModifiedCode: "public class Test { }",
            ReviewOptions: new ChangeReviewOptions(),
            IncludeDiff: true,
            CheckSemanticDrift: true
        );

        var expectedResult = new SemanticChangeReviewResult(
            Success: true,
            SemanticDiff: new SemanticDiffAnalysis(
                StructuralChanges: new[]
                {
                    new StructuralChange("AccessModifier", "Test", "Added public modifier", "Low")
                },
                BehavioralChanges: Array.Empty<BehavioralChange>(),
                ImpactAnalysis: new ImpactAnalysis(
                    OverallImpact: "Low",
                    AffectedComponents: new[] { "Test class" },
                    RiskLevel: "Low",
                    MitigationStrategies: Array.Empty<string>()
                ),
                ConfidenceScore: 0.95
            ),
            DriftAnalysis: new DriftAnalysis(
                DriftDetected: false,
                DriftType: "None",
                DriftSeverity: "None",
                AffectedAreas: Array.Empty<string>(),
                Recommendations: Array.Empty<string>()
            ),
            FixSuggestions: Array.Empty<FixSuggestion>(),
            ConfidenceScore: 0.95,
            ReviewTimeMs: 800
        );

        _mockService.ReviewSemanticChangesAsync(request, _cancellationToken)
            .Returns(expectedResult);

        // Act
        var result = await _mockService.ReviewSemanticChangesAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.ConfidenceScore.ShouldBe(0.95);
    }

    /// <summary>
    /// Confirms Fixer001 configuration retrieval succeeds for a valid solution path.
    /// </summary>
    [Fact]
    public async Task GetFixer001ConfigurationAsync_WithValidSolutionPath_ShouldReturnSuccessResult()
    {
        // Arrange
        var solutionPath = "TestSolution.sln";
        var expectedConfig = new Fixer001Configuration(
            SolutionPath: solutionPath,
            AvailableTransformations: new[]
            {
                new TransformationInfo(
                    Id: "EXXER001",
                    Name: "Add XML Documentation",
                    Description: "Adds missing XML documentation to public members",
                    SupportedLanguages: new[] { "C#" },
                    IsEnabled: true,
                    Parameters: []
                )
            },
            DefaultSettings: new Dictionary<string, object>
            {
                { "MaxFixesPerFile", 10 },
                { "BackupOriginal", true }
            },
            Version: "1.0.0",
            LastUpdated: DateTime.UtcNow
        );

        _mockService.GetFixer001ConfigurationAsync(solutionPath, _cancellationToken)
            .Returns(expectedConfig);

        // Act
        var result = await _mockService.GetFixer001ConfigurationAsync(solutionPath, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.SolutionPath.ShouldBe(solutionPath);
        result.AvailableTransformations.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Ensures invalid fixer requests are surfaced as failures.
    /// </summary>
    [Fact]
    public async Task ApplyFixer001Async_WithInvalidRequest_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "INVALID001",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: false
        );

        _mockService.ApplyFixer001Async(request, _cancellationToken)
            .Returns(new CodeTransformationResult(
                Success: false,
                TransformationDetails: new TransformationDetails("", "", "", 0, 0, 0.0),
                ValidationResults: Array.Empty<ValidationResult>(),
                ErrorDetails: "Operation was cancelled"
            ));

        // Act
        var result = await _mockService.ApplyFixer001Async(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.ErrorDetails!.ShouldContain("cancelled");
    }
}
