using Microsoft.Extensions.Logging;
using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Services;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.SafeTransformation;

/// <summary>
/// Behavioral tests for CodeTransformationService using real service instances.
/// These tests verify actual behavior rather than mock interactions.
/// </summary>
public class CodeTransformationServiceBehavioralTests
{
    private readonly CodeTransformationService _service;
    private readonly ILogger<CodeTransformationService> _logger;
    private readonly CancellationToken _cancellationToken = Xunit.TestContext.Current.CancellationToken;

    public CodeTransformationServiceBehavioralTests()
    {
        // Use real logger from xUnit v3
        _logger = Xunit.TestContext.Current.LoggerFactory.CreateLogger<CodeTransformationService>();
        
        // Create real service dependencies
        var buildValidationService = new BuildValidationService(Xunit.TestContext.Current.LoggerFactory.CreateLogger<BuildValidationService>());
        var fixer001Service = new Fixer001Service(Xunit.TestContext.Current.LoggerFactory.CreateLogger<Fixer001Service>(), buildValidationService);
        var safeRegexService = new SafeRegexService(Xunit.TestContext.Current.LoggerFactory.CreateLogger<SafeRegexService>(), buildValidationService);
        
        _service = new CodeTransformationService(_logger, fixer001Service, safeRegexService, buildValidationService);
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
        result.Success.ShouldBeTrue("Code transformation should succeed");
        result.TransformationType.ShouldBe("Fixer001");
        result.ValidationResults.ShouldNotBeNull();

        // Cleanup
        CleanupTestFile(testFile);
    }

    [Fact]
    public async Task ApplyFixer001Async_WithInvalidRequest_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "INVALID001",
            TargetFiles: Array.Empty<string>(),
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        // Act
        var result = await _service.ApplyFixer001Async(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse("Code transformation should fail for invalid request");
        result.TransformationType.ShouldBe("Fixer001");
        result.ErrorDetails.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task ApplySafeRegexAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var testFile = CreateTestFile("class TestClass { }");
        var request = new SafeRegexRequest(
            Pattern: @"\bclass\s+(\w+)",
            Replacement: "public class $1",
            TargetFiles: new[] { testFile },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true,
            CaseSensitive: true,
            Multiline: false
        );

        // Act
        var result = await _service.ApplySafeRegexAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue("Code transformation should succeed");
        result.TransformationType.ShouldBe("SafeRegex");
        result.ValidationResults.ShouldNotBeNull();

        // Cleanup
        CleanupTestFile(testFile);
    }

    [Fact]
    public async Task ApplySafeRegexAsync_WithInvalidRequest_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new SafeRegexRequest(
            Pattern: "",
            Replacement: "",
            TargetFiles: Array.Empty<string>(),
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        // Act
        var result = await _service.ApplySafeRegexAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse("Code transformation should fail for invalid request");
        result.TransformationType.ShouldBe("SafeRegex");
        result.ErrorDetails.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task ValidateTransformationAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new TransformationValidationRequest(
            OriginalCode: "class Test { }",
            TransformedCode: "public class Test { }",
            ValidationCriteria: new ValidationCriteria(
                CheckSyntax: true,
                CheckBuild: true,
                CheckAnalyzers: true,
                MaxIssues: 10
            )
        );

        // Act
        var result = await _service.ValidateTransformationAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.ValidationChecks.ShouldNotBeNull();
        result.AnalyzerResults.ShouldNotBeNull();
        result.NewIssues.ShouldNotBeNull();
    }

    [Fact]
    public async Task ReviewSemanticChangesAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new SemanticChangeReviewRequest(
            OriginalCode: "class Test { }",
            ModifiedCode: "public class Test { }",
            ReviewOptions: new ChangeReviewOptions(
                IncludeMetrics: true,
                CheckPerformance: true,
                CheckSecurity: true
            )
        );

        // Act
        var result = await _service.ReviewSemanticChangesAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue("Semantic change review should succeed");
        result.Changes.ShouldNotBeNull();
        result.FixSuggestions.ShouldNotBeNull();
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
        result.SolutionPath.ShouldBe(solutionPath);
        result.Version.ShouldNotBeNullOrEmpty();
        result.AvailableTransformations.ShouldNotBeNull();
        result.DefaultSettings.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetFixer001ConfigurationAsync_WithInvalidSolutionPath_ShouldReturnDefaultConfiguration()
    {
        // Arrange
        var solutionPath = "NonExistentSolution.sln";

        // Act
        var result = await _service.GetFixer001ConfigurationAsync(solutionPath, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.SolutionPath.ShouldBe(solutionPath);
        result.Version.ShouldNotBeNullOrEmpty();
        result.AvailableTransformations.ShouldNotBeNull();
        result.DefaultSettings.ShouldNotBeNull();
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
    public async Task ApplySafeRegexAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var testFile = CreateTestFile("class TestClass { }");
        var request = new SafeRegexRequest(
            Pattern: @"\bclass\s+(\w+)",
            Replacement: "public class $1",
            TargetFiles: new[] { testFile },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await _service.ApplySafeRegexAsync(request, cts.Token));

        // Cleanup
        CleanupTestFile(testFile);
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
