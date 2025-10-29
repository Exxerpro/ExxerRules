using Microsoft.Extensions.Logging;
using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Services;
using IndQuestResults;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.SafeTransformation;

/// <summary>
/// Behavioral tests for Fixer001Service using real service instances.
/// These tests verify actual behavior rather than mock interactions.
/// </summary>
public class Fixer001ServiceBehavioralTests
{
    private readonly Fixer001Service _service;
    private readonly ILogger<Fixer001Service> _logger;
    private readonly IBuildValidationService _buildValidationService;
    private readonly CancellationToken _cancellationToken = Xunit.TestContext.Current.CancellationToken;

    public Fixer001ServiceBehavioralTests()
    {
        // Use real logger from xUnit v3
        _logger = Xunit.TestContext.Current.LoggerFactory.CreateLogger<Fixer001Service>();
        _buildValidationService = new BuildValidationService(Xunit.TestContext.Current.LoggerFactory.CreateLogger<BuildValidationService>());
        _service = new Fixer001Service(_logger, _buildValidationService);
    }

    [Fact]
    public async Task ApplyFixer001Async_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var testFile = CreateTestFile("class TestClass { }");
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: new[] { testFile },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true,
            BackupOriginal: true,
            MaxFixesPerFile: 10
        );

        // Act
        var result = await _service.ApplyFixer001Async(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Fixer001 transformation should succeed");
        result.Value.ShouldNotBeNull();
        result.Value.Success.ShouldBeTrue();
        result.Value.TransformationDetails.ShouldNotBeNull();
        result.Value.TransformationDetails.DiagnosticId.ShouldBe(request.DiagnosticId);
        result.Value.TransformationDetails.TransformationType.ShouldBe("Fixer001");

        // Cleanup
        CleanupTestFile(testFile);
    }

    [Fact]
    public async Task ApplyFixer001Async_WithInvalidDiagnosticId_ShouldReturnFailureResult()
    {
        // Arrange
        var testFile = CreateTestFile("class TestClass { }");
        var request = new Fixer001Request(
            DiagnosticId: "INVALID001",
            TargetFiles: new[] { testFile },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        // Act
        var result = await _service.ApplyFixer001Async(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue("Fixer001 transformation should fail for invalid diagnostic ID");
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("Unknown diagnostic ID");

        // Cleanup
        CleanupTestFile(testFile);
    }

    [Fact]
    public async Task ApplyFixer001Async_WithEmptyTargetFiles_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: Array.Empty<string>(),
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        // Act
        var result = await _service.ApplyFixer001Async(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue("Fixer001 transformation should fail with no target files");
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("No target files specified");
    }

    [Fact]
    public async Task GetFixer001ConfigurationAsync_WithValidSolutionPath_ShouldReturnSuccessResult()
    {
        // Arrange
        var solutionPath = GetTestSolutionPath();

        // Act
        var result = await _service.GetFixer001ConfigurationAsync(solutionPath, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Fixer001 configuration should succeed");
        result.Value.ShouldNotBeNull();
        result.Value.SolutionPath.ShouldBe(solutionPath);
        result.Value.Version.ShouldNotBeNullOrEmpty();
        result.Value.AvailableTransformations.ShouldNotBeNull();
        result.Value.DefaultSettings.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetFixer001ConfigurationAsync_WithInvalidSolutionPath_ShouldReturnFailureResult()
    {
        // Arrange
        var solutionPath = "NonExistentSolution.sln";

        // Act
        var result = await _service.GetFixer001ConfigurationAsync(solutionPath, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue("Fixer001 configuration should fail for non-existent solution");
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("Solution file not found");
    }

    [Fact]
    public async Task PreviewFixer001TransformationAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var testFile = CreateTestFile("class TestClass { }");
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: new[] { testFile },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        // Act
        var result = await _service.PreviewFixer001TransformationAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Fixer001 transformation preview should succeed");
        result.Value.ShouldNotBeNull();
        result.Value.Success.ShouldBeTrue();
        result.Value.PreviewDetails.ShouldNotBeNull();
        result.Value.PreviewDetails.DiagnosticId.ShouldBe(request.DiagnosticId);

        // Cleanup
        CleanupTestFile(testFile);
    }

    [Fact]
    public async Task PreviewFixer001TransformationAsync_WithInvalidDiagnosticId_ShouldReturnFailureResult()
    {
        // Arrange
        var testFile = CreateTestFile("class TestClass { }");
        var request = new Fixer001Request(
            DiagnosticId: "INVALID001",
            TargetFiles: new[] { testFile },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        // Act
        var result = await _service.PreviewFixer001TransformationAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue("Fixer001 transformation preview should fail for invalid diagnostic ID");
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("Unknown diagnostic ID");

        // Cleanup
        CleanupTestFile(testFile);
    }

    [Fact]
    public async Task ValidateFixer001ReadinessAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var testFile = CreateTestFile("class TestClass { }");
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: new[] { testFile },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        // Act
        var result = await _service.ValidateFixer001ReadinessAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Fixer001 readiness validation should succeed");
        result.Value.ShouldNotBeNull();
        result.Value.Issues.ShouldNotBeNull();
        result.Value.Warnings.ShouldNotBeNull();
        result.Value.ReadinessScore.ShouldBeGreaterThanOrEqualTo(0.0);
        result.Value.ReadinessScore.ShouldBeLessThanOrEqualTo(1.0);

        // Cleanup
        CleanupTestFile(testFile);
    }

    [Fact]
    public async Task ValidateFixer001ReadinessAsync_WithInvalidDiagnosticId_ShouldReturnFailureResult()
    {
        // Arrange
        var testFile = CreateTestFile("class TestClass { }");
        var request = new Fixer001Request(
            DiagnosticId: "INVALID001",
            TargetFiles: new[] { testFile },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        // Act
        var result = await _service.ValidateFixer001ReadinessAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Fixer001 readiness validation should succeed even for invalid diagnostic ID");
        result.Value.ShouldNotBeNull();
        result.Value.IsReady.ShouldBeFalse("Should not be ready for invalid diagnostic ID");
        result.Value.Issues.ShouldNotBeEmpty("Should have issues for invalid diagnostic ID");

        // Cleanup
        CleanupTestFile(testFile);
    }

    [Fact]
    public async Task ValidateFixer001ReadinessAsync_WithEmptyTargetFiles_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: Array.Empty<string>(),
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        // Act
        var result = await _service.ValidateFixer001ReadinessAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Fixer001 readiness validation should succeed");
        result.Value.ShouldNotBeNull();
        result.Value.IsReady.ShouldBeFalse("Should not be ready with no target files");
        result.Value.Issues.ShouldNotBeEmpty("Should have issues with no target files");
    }

    [Fact]
    public async Task ApplyFixer001Async_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var testFile = CreateTestFile("class TestClass { }");
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: new[] { testFile },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await _service.ApplyFixer001Async(request, cts.Token));

        // Cleanup
        CleanupTestFile(testFile);
    }

    [Fact]
    public async Task GetFixer001ConfigurationAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var solutionPath = GetTestSolutionPath();

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await _service.GetFixer001ConfigurationAsync(solutionPath, cts.Token));
    }

    #region Private Helper Methods

    private static string CreateTestFile(string content)
    {
        var tempFile = Path.GetTempFileName();
        var testFile = tempFile + ".cs";
        File.Move(tempFile, testFile);
        File.WriteAllText(testFile, content);
        return testFile;
    }

    private static void CleanupTestFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private static string GetTestSolutionPath()
    {
        // Return a path to a test solution file
        // In a real implementation, this would create a minimal test solution
        return Path.Combine(Path.GetTempPath(), "TestSolution.sln");
    }

    #endregion
}
