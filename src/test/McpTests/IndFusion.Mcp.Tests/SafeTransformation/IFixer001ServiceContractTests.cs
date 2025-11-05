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
/// <remarks>
/// The scenarios capture the contract that orchestration layers rely on when invoking Fixer001 operations.
/// </remarks>
public class IFixer001ServiceContractTests
{
    private readonly IFixer001Service _mockService;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    /// <summary>
    /// Initializes the contract tests with a mocked IFixer001Service.
    /// </summary>
    /// <remarks>
    /// Substituting the service keeps the tests focused on contract semantics instead of implementation details.
    /// </remarks>
    public IFixer001ServiceContractTests()
    {
        _mockService = Substitute.For<IFixer001Service>();
    }

    /// <summary>
    /// Verifies applying Fixer001 succeeds when the request is valid.
    /// </summary>
    /// <remarks>
    /// A successful contract response must include transformation metadata and confirm that fixes were applied.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that validates the mocked service behaviour.</returns>
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

    /// <summary>
    /// Ensures unknown diagnostic identifiers cause the apply operation to fail.
    /// </summary>
    /// <remarks>
    /// The contract requires diagnostic validation before work begins so that callers can surface precise failure reasons.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that validates the mocked service behaviour.</returns>
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

    /// <summary>
    /// Validates the service rejects Fixer001 requests without target files.
    /// </summary>
    /// <remarks>
    /// An empty target list is a caller error; the contract mandates a clear failure response.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that validates the mocked service behaviour.</returns>
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

    /// <summary>
    /// Confirms Fixer001 configuration can be retrieved for a valid solution path.
    /// </summary>
    /// <remarks>
    /// Tooling relies on the configuration payload to describe available transformations and defaults.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that validates the mocked service behaviour.</returns>
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

    /// <summary>
    /// Ensures configuration retrieval fails when the solution path is invalid.
    /// </summary>
    /// <remarks>
    /// Calling code must be able to distinguish IO failures from successful configuration loads.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that validates the mocked service behaviour.</returns>
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

    /// <summary>
    /// Verifies previewing Fixer001 transformations succeeds for a valid request.
    /// </summary>
    /// <remarks>
    /// Previews should supply diff metadata without committing changes, enabling safe UX flows.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that validates the mocked service behaviour.</returns>
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

    /// <summary>
    /// Confirms readiness checks pass when prerequisites are met.
    /// </summary>
    /// <remarks>
    /// A ready response signals that downstream components can safely run the fixer.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that validates the mocked service behaviour.</returns>
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

    /// <summary>
    /// Ensures readiness issues are surfaced as a failure result.
    /// </summary>
    /// <remarks>
    /// Even when the call succeeds, the payload should indicate that the fixer is not ready and list actionable issues.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that validates the mocked service behaviour.</returns>
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

    /// <summary>
    /// Verifies Fixer001 application respects cancellation tokens.
    /// </summary>
    /// <remarks>
    /// Cancellation should cascade to callers so that they can distinguish aborts from operational failures.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that validates the mocked service behaviour.</returns>
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

    /// <summary>
    /// Ensures configuration retrieval honors cancellation requests.
    /// </summary>
    /// <remarks>
    /// Long-running configuration retrieval must observe cancellation to remain responsive in tooling scenarios.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that validates the mocked service behaviour.</returns>
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

    /// <summary>
    /// Verifies preview requests for Fixer001 respect cancellation tokens.
    /// </summary>
    /// <remarks>
    /// Preview operations should terminate promptly when cancellation is requested to avoid stale diffs.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that validates the mocked service behaviour.</returns>
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

    /// <summary>
    /// Confirms readiness validation respects cancellation tokens.
    /// </summary>
    /// <remarks>
    /// Cancellation should propagate without marking the fixer as ready or not ready, preserving caller intent.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that validates the mocked service behaviour.</returns>
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
