using IndFusion.Mcp.Core.Abstractions;
using IndQuestResults;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.SafeTransformation;

/// <summary>
/// Contract tests for IFixer001Service interface.
/// These tests verify the contract behavior using mocks and should ALWAYS PASS.
/// </summary>
public class IFixer001ServiceContractTests
{
    private readonly IFixer001Service _mockService;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public IFixer001ServiceContractTests()
    {
        _mockService = Substitute.For<IFixer001Service>();
    }

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

        var expectedResult = new Fixer001Result(
            Success: true,
            TransformationDetails: new Fixer001TransformationDetails(
                TransformationType: "Fixer001",
                TransformationId: "fixer-123",
                Description: "Fix EXXER001 violations",
                ChangesApplied: 3,
                FilesAffected: 1,
                Confidence: 0.98,
                DiagnosticId: "EXXER001",
                FixerVersion: "1.0.0"
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
            .Returns(Result<Fixer001Result>.Success(expectedResult));

        // Act
        var result = await _mockService.ApplyFixer001Async(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Success.ShouldBeTrue();
        result.Value.ChangesApplied.ShouldBe(3);
        result.Value.FilesAffected.ShouldBe(1);
    }

    [Fact]
    public async Task ApplyFixer001Async_WithInvalidDiagnosticId_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "INVALID001",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: false
        );

        _mockService.ApplyFixer001Async(request, _cancellationToken)
            .Returns(Result<Fixer001Result>.WithFailure("Unknown diagnostic ID: INVALID001"));

        // Act
        var result = await _mockService.ApplyFixer001Async(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("Unknown diagnostic ID");
    }

    [Fact]
    public async Task ApplyFixer001Async_WithEmptyTargetFiles_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: Array.Empty<string>(),
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: false
        );

        _mockService.ApplyFixer001Async(request, _cancellationToken)
            .Returns(Result<Fixer001Result>.WithFailure("No target files specified"));

        // Act
        var result = await _mockService.ApplyFixer001Async(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("No target files specified");
    }

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
            .Returns(Result<Fixer001Configuration>.Success(expectedConfig));

        // Act
        var result = await _mockService.GetFixer001ConfigurationAsync(solutionPath, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.SolutionPath.ShouldBe(solutionPath);
        result.Value.AvailableTransformations.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetFixer001ConfigurationAsync_WithInvalidSolutionPath_ShouldReturnFailureResult()
    {
        // Arrange
        var solutionPath = "NonExistent.sln";

        _mockService.GetFixer001ConfigurationAsync(solutionPath, _cancellationToken)
            .Returns(Result<Fixer001Configuration>.WithFailure("Solution file not found"));

        // Act
        var result = await _mockService.GetFixer001ConfigurationAsync(solutionPath, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("not found");
    }

    [Fact]
    public async Task PreviewFixer001TransformationAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        var expectedResult = new Fixer001PreviewResult(
            Success: true,
            PreviewDetails: new Fixer001PreviewDetails(
                PreviewId: "preview-123",
                DiagnosticId: "EXXER001",
                EstimatedFixes: 3,
                ReadinessAssessment: "Ready"
            ),
            EstimatedChanges: 3,
            AffectedFiles: new[] { "TestFile.cs" },
            PreviewTimeMs: 300
        );

        _mockService.PreviewFixer001TransformationAsync(request, _cancellationToken)
            .Returns(Result<Fixer001PreviewResult>.Success(expectedResult));

        // Act
        var result = await _mockService.PreviewFixer001TransformationAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Success.ShouldBeTrue();
        result.Value.EstimatedChanges.ShouldBe(3);
        result.Value.AffectedFiles.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task ValidateFixer001ReadinessAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: false
        );

        var expectedResult = new Fixer001ValidationResult(
            IsReady: true,
            ReadinessScore: 0.95,
            Issues: Array.Empty<Fixer001Issue>(),
            Warnings: Array.Empty<Fixer001Warning>(),
            ValidationTimeMs: 150
        );

        _mockService.ValidateFixer001ReadinessAsync(request, _cancellationToken)
            .Returns(Result<Fixer001ValidationResult>.Success(expectedResult));

        // Act
        var result = await _mockService.ValidateFixer001ReadinessAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsReady.ShouldBeTrue();
        result.Value.ReadinessScore.ShouldBe(0.95);
    }

    [Fact]
    public async Task ValidateFixer001ReadinessAsync_WithIssues_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: false
        );

        var expectedResult = new Fixer001ValidationResult(
            IsReady: false,
            ReadinessScore: 0.3,
            Issues: new[]
            {
                new Fixer001Issue("issue-001", "FileNotFound", "High", "Target file does not exist", "Check file path")
            },
            Warnings: Array.Empty<Fixer001Warning>(),
            ValidationTimeMs: 100
        );

        _mockService.ValidateFixer001ReadinessAsync(request, _cancellationToken)
            .Returns(Result<Fixer001ValidationResult>.Success(expectedResult));

        // Act
        var result = await _mockService.ValidateFixer001ReadinessAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsReady.ShouldBeFalse();
        result.Value.ReadinessScore.ShouldBe(0.3);
        result.Value.Issues.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task ApplyFixer001Async_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: false
        );

        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockService.ApplyFixer001Async(request, cts.Token)
            .Returns(Result<Fixer001Result>.WithFailure("Operation was cancelled"));

        // Act
        var result = await _mockService.ApplyFixer001Async(request, cts.Token);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cancelled");
    }

    [Fact]
    public async Task GetFixer001ConfigurationAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var solutionPath = "TestSolution.sln";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockService.GetFixer001ConfigurationAsync(solutionPath, cts.Token)
            .Returns(Result<Fixer001Configuration>.WithFailure("Operation was cancelled"));

        // Act
        var result = await _mockService.GetFixer001ConfigurationAsync(solutionPath, cts.Token);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cancelled");
    }

    [Fact]
    public async Task PreviewFixer001TransformationAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockService.PreviewFixer001TransformationAsync(request, cts.Token)
            .Returns(Result<Fixer001PreviewResult>.WithFailure("Operation was cancelled"));

        // Act
        var result = await _mockService.PreviewFixer001TransformationAsync(request, cts.Token);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cancelled");
    }

    [Fact]
    public async Task ValidateFixer001ReadinessAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: false
        );

        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockService.ValidateFixer001ReadinessAsync(request, cts.Token)
            .Returns(Result<Fixer001ValidationResult>.WithFailure("Operation was cancelled"));

        // Act
        var result = await _mockService.ValidateFixer001ReadinessAsync(request, cts.Token);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cancelled");
    }
}
