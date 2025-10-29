using Microsoft.Extensions.Logging;
using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Services;
using IndQuestResults;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.SafeTransformation;

/// <summary>
/// Behavioral tests for BuildValidationService using real service instances.
/// These tests verify actual behavior rather than mock interactions.
/// </summary>
public class BuildValidationServiceBehavioralTests
{
    private readonly BuildValidationService _service;
    private readonly ILogger<BuildValidationService> _logger;
    private readonly CancellationToken _cancellationToken = Xunit.TestContext.Current.CancellationToken;

    public BuildValidationServiceBehavioralTests()
    {
        // Use simple logger factory
        var loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<BuildValidationService>();
        _service = new BuildValidationService(_logger);
    }

    [Fact]
    public async Task ValidateTransformationAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new BuildValidationRequest(
            SolutionPath: GetTestSolutionPath(),
            TransformedFiles: new[]
            {
                new TransformedFile(
                    FilePath: "TestFile.cs",
                    OriginalContent: "class Test { }",
                    TransformedContent: "public class Test { }",
                    TransformationType: "SafeRegex"
                )
            },
            ValidationOptions: new TransformationValidationOptions(),
            RunAnalyzers: true,
            BuildValidation: true,
            CheckForNewIssues: true
        );

        // Act
        var result = await _service.ValidateTransformationAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Build validation should succeed");
        result.Value.ShouldNotBeNull();
        result.Value.ValidationChecks.ShouldNotBeEmpty("Should have validation checks");
    }

    [Fact]
    public async Task ValidateTransformationAsync_WithInvalidSolutionPath_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new BuildValidationRequest(
            SolutionPath: "NonExistentSolution.sln",
            TransformedFiles: Array.Empty<TransformedFile>(),
            ValidationOptions: new TransformationValidationOptions(),
            RunAnalyzers: true,
            BuildValidation: true,
            CheckForNewIssues: true
        );

        // Act
        var result = await _service.ValidateTransformationAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue("Build validation should fail for non-existent solution");
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("Solution file not found");
    }

    [Fact]
    public async Task ValidateFileTransformationAsync_WithValidFile_ShouldReturnSuccessResult()
    {
        // Arrange
        var filePath = GetTestFilePath();
        var originalContent = "class Test { }";
        var transformedContent = "public class Test { }";
        var validationOptions = new TransformationValidationOptions();

        // Act
        var result = await _service.ValidateFileTransformationAsync(filePath, originalContent, transformedContent, validationOptions, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("File validation should succeed");
        result.Value.ShouldNotBeNull();
        result.Value.FilePath.ShouldBe(filePath);
        result.Value.ValidationChecks.ShouldNotBeEmpty("Should have validation checks");
    }

    [Fact]
    public async Task ValidateFileTransformationAsync_WithInvalidContent_ShouldReturnFailureResult()
    {
        // Arrange
        var filePath = GetTestFilePath();
        var originalContent = "class Test { }";
        var transformedContent = "invalid c# syntax {";
        var validationOptions = new TransformationValidationOptions();

        // Act
        var result = await _service.ValidateFileTransformationAsync(filePath, originalContent, transformedContent, validationOptions, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("File validation should succeed even with invalid content");
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeFalse("Should detect invalid syntax");
        result.Value.NewIssues.ShouldNotBeEmpty("Should have syntax issues");
    }

    [Fact]
    public async Task CreateTemporaryWorkspaceAsync_WithValidSolutionPath_ShouldReturnSuccessResult()
    {
        // Arrange
        var solutionPath = GetTestSolutionPath();

        // Act
        var result = await _service.CreateTemporaryWorkspaceAsync(solutionPath, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Temporary workspace creation should succeed");
        result.Value.ShouldNotBeNull();
        result.Value.WorkspacePath.ShouldNotBeNullOrEmpty();
        result.Value.OriginalSolutionPath.ShouldBe(solutionPath);
        result.Value.CreatedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);

        // Cleanup
        await _service.CleanupTemporaryWorkspaceAsync(result.Value, _cancellationToken);
    }

    [Fact]
    public async Task CreateTemporaryWorkspaceAsync_WithInvalidSolutionPath_ShouldReturnFailureResult()
    {
        // Arrange
        var solutionPath = "NonExistentSolution.sln";

        // Act
        var result = await _service.CreateTemporaryWorkspaceAsync(solutionPath, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue("Temporary workspace creation should fail for non-existent solution");
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("Solution file not found");
    }

    [Fact]
    public async Task CleanupTemporaryWorkspaceAsync_WithValidWorkspace_ShouldReturnSuccessResult()
    {
        // Arrange
        var solutionPath = GetTestSolutionPath();
        var createResult = await _service.CreateTemporaryWorkspaceAsync(solutionPath, _cancellationToken);
        createResult.IsSuccess.ShouldBeTrue("Workspace creation should succeed for cleanup test");

        // Act
        var result = await _service.CleanupTemporaryWorkspaceAsync(createResult.Value!, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Workspace cleanup should succeed");
    }

    [Fact]
    public async Task ValidateTransformationAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var request = new BuildValidationRequest(
            SolutionPath: GetTestSolutionPath(),
            TransformedFiles: Array.Empty<TransformedFile>(),
            ValidationOptions: new TransformationValidationOptions(),
            RunAnalyzers: true,
            BuildValidation: true,
            CheckForNewIssues: true
        );

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await _service.ValidateTransformationAsync(request, cts.Token));
    }

    [Fact]
    public async Task ValidateFileTransformationAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var filePath = GetTestFilePath();
        var originalContent = "class Test { }";
        var transformedContent = "public class Test { }";
        var validationOptions = new TransformationValidationOptions();

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await _service.ValidateFileTransformationAsync(filePath, originalContent, transformedContent, validationOptions, cts.Token));
    }

    #region Private Helper Methods

    private static string GetTestSolutionPath()
    {
        // Return a path to a test solution file
        // In a real implementation, this would create a minimal test solution
        return Path.Combine(Path.GetTempPath(), "TestSolution.sln");
    }

    private static string GetTestFilePath()
    {
        // Return a path to a test file
        return Path.Combine(Path.GetTempPath(), "TestFile.cs");
    }

    #endregion
}
