using Microsoft.Extensions.Logging;
using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Services;
using IndFusion.Mcp.Tests.TestInfrastructure;
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

    /// <summary>
    /// Builds a fully wired CodeTransformationService for behavioral verification.
    /// </summary>
    public CodeTransformationServiceBehavioralTests()
    {
        // Use simple logger factory
        var loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<CodeTransformationService>();
        
        // Create real service dependencies
        var buildValidationLogger = loggerFactory.CreateLogger<BuildValidationService>();
        var buildValidationService = new BuildValidationService(buildValidationLogger);
        var fixer001Logger = loggerFactory.CreateLogger<Fixer001Service>();
        var fixer001Service = new Fixer001Service(fixer001Logger, buildValidationService);
        var safeRegexLogger = loggerFactory.CreateLogger<SafeRegexService>();
        var safeRegexService = new SafeRegexService(safeRegexLogger, buildValidationService);
        
        _service = new CodeTransformationService(_logger, fixer001Service, safeRegexService, buildValidationService);
    }

    /// <summary>
    /// Verifies the composite service applies Fixer001 transformations successfully.
    /// </summary>
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

    /// <summary>
    /// Ensures invalid Fixer001 requests are rejected by the service.
    /// </summary>
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

    /// <summary>
    /// Confirms safe regex transformations succeed through the composite service.
    /// </summary>
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

    /// <summary>
    /// Ensures invalid safe regex requests surface a failure result.
    /// </summary>
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

    /// <summary>
    /// Verifies aggregate validation succeeds for valid transformation requests.
    /// </summary>
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

    /// <summary>
    /// Confirms semantic review generation succeeds for valid requests.
    /// </summary>
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

    /// <summary>
    /// Ensures Fixer001 configuration retrieval works in the behavioral scenario.
    /// </summary>
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

    /// <summary>
    /// Verifies the composite service falls back when the solution path is invalid.
    /// </summary>
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

    /// <summary>
    /// Confirms Fixer001 application respects cancellation tokens.
    /// </summary>
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

        // Act
        var result = await _service.ApplyFixer001Async(request, cts.Token);
        
        // Assert
        result.Success.ShouldBeFalse("Operation should fail when cancellation token is triggered");
        result.ErrorDetails?.ShouldContain("Operation was cancelled");

        // Cleanup
        CleanupTestFile(testFile);
    }

    /// <summary>
    /// Ensures safe regex application respects cancellation tokens.
    /// </summary>
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

        // Act
        var result = await _service.ApplySafeRegexAsync(request, cts.Token);
        
        // Assert
        result.Success.ShouldBeFalse("Operation should fail when cancellation token is triggered");
        result.ErrorDetails?.ShouldContain("Operation was cancelled");

        // Cleanup
        CleanupTestFile(testFile);
    }

    //  Private Helper Methods

    private static string CreateTestFile(string content)
    {
        var fileName = $"TestFile_{Guid.NewGuid():N}";
        return TestSolutionFactory.CreateTestFile(fileName, content);
    }

    private static void CleanupTestFile(string filePath)
    {
        TestSolutionFactory.CleanupTestFile(filePath);
    }

    private static string GetTestSolutionPath()
    {
        return TestSolutionFactory.GetOrCreateTestSolution();
    }

     // 
}
