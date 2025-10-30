using Microsoft.Extensions.Logging;
using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Services;
using IndQuestResults;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.SafeTransformation;

/// <summary>
/// Behavioral tests for SafeRegexService using real service instances.
/// These tests verify actual behavior rather than mock interactions.
/// </summary>
public class SafeRegexServiceBehavioralTests
{
    private readonly SafeRegexService _service;
    private readonly ILogger<SafeRegexService> _logger;
    private readonly IBuildValidationService _buildValidationService;
    private readonly CancellationToken _cancellationToken = Xunit.TestContext.Current.CancellationToken;

    public SafeRegexServiceBehavioralTests()
    {
        // Use simple logger factory
        var loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<SafeRegexService>();
        _buildValidationService = new BuildValidationService(loggerFactory.CreateLogger<BuildValidationService>());
        _service = new SafeRegexService(_logger, _buildValidationService);
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
        result.IsSuccess.ShouldBeTrue("Safe regex transformation should succeed");
        result.Value.ShouldNotBeNull();
        result.Value.Success.ShouldBeTrue();
        result.Value.TransformationDetails.ShouldNotBeNull();
        result.Value.TransformationDetails.Pattern.ShouldBe(request.Pattern);
        result.Value.TransformationDetails.Replacement.ShouldBe(request.Replacement);

        // Cleanup
        CleanupTestFile(testFile);
    }

    [Fact]
    public async Task ApplySafeRegexAsync_WithInvalidPattern_ShouldReturnFailureResult()
    {
        // Arrange
        var testFile = CreateTestFile("class TestClass { }");
        var request = new SafeRegexRequest(
            Pattern: @"[invalid-regex-pattern",
            Replacement: "replacement",
            TargetFiles: new[] { testFile },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        // Act
        var result = await _service.ApplySafeRegexAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue("Safe regex transformation should fail for invalid pattern");
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("Invalid regex pattern");

        // Cleanup
        CleanupTestFile(testFile);
    }

    [Fact]
    public async Task ApplySafeRegexAsync_WithEmptyTargetFiles_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new SafeRegexRequest(
            Pattern: @"\bclass\s+(\w+)",
            Replacement: "public class $1",
            TargetFiles: Array.Empty<string>(),
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        // Act
        var result = await _service.ApplySafeRegexAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue("Safe regex transformation should fail with no target files");
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("No target files specified");
    }

    [Fact]
    public async Task ValidateRegexPatternAsync_WithValidPattern_ShouldReturnSuccessResult()
    {
        // Arrange
        var pattern = @"\bclass\s+(\w+)";

        // Act
        var result = await _service.ValidateRegexPatternAsync(pattern, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Regex pattern validation should succeed");
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeTrue();
        result.Value.SafetyScore.ShouldBeGreaterThan(0.0);
        result.Value.Issues.ShouldBeEmpty("Valid pattern should have no issues");
    }

    [Fact]
    public async Task ValidateRegexPatternAsync_WithInvalidPattern_ShouldReturnFailureResult()
    {
        // Arrange
        var pattern = @"[invalid-regex-pattern";

        // Act
        var result = await _service.ValidateRegexPatternAsync(pattern, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Regex pattern validation should succeed even for invalid patterns");
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeFalse("Invalid pattern should not be valid");
        result.Value.SafetyScore.ShouldBe(0.0);
        result.Value.Issues.ShouldNotBeEmpty("Invalid pattern should have issues");
    }

    [Fact]
    public async Task ValidateRegexPatternAsync_WithReDoSPattern_ShouldReturnWarningResult()
    {
        // Arrange
        var pattern = @"(a+)+"; // ReDoS vulnerable pattern

        // Act
        var result = await _service.ValidateRegexPatternAsync(pattern, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Regex pattern validation should succeed");
        result.Value.ShouldNotBeNull();
        result.Value.SafetyScore.ShouldBeLessThan(1.0, "ReDoS vulnerable pattern should have lower safety score");
        result.Value.Issues.ShouldNotBeEmpty("ReDoS vulnerable pattern should have issues");
    }

    [Fact]
    public async Task PreviewRegexTransformationAsync_WithValidRequest_ShouldReturnSuccessResult()
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

        // Act
        var result = await _service.PreviewRegexTransformationAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Regex transformation preview should succeed");
        result.Value.ShouldNotBeNull();
        result.Value.Success.ShouldBeTrue();
        result.Value.PreviewDetails.ShouldNotBeNull();
        result.Value.PreviewDetails.Pattern.ShouldBe(request.Pattern);
        result.Value.PreviewDetails.Replacement.ShouldBe(request.Replacement);
        result.Value.EstimatedChanges.ShouldBeGreaterThan(0, "Should estimate changes for matching pattern");

        // Cleanup
        CleanupTestFile(testFile);
    }

    [Fact]
    public async Task PreviewRegexTransformationAsync_WithNoMatches_ShouldReturnSuccessResult()
    {
        // Arrange
        var testFile = CreateTestFile("interface ITest { }");
        var request = new SafeRegexRequest(
            Pattern: @"\bclass\s+(\w+)",
            Replacement: "public class $1",
            TargetFiles: new[] { testFile },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        // Act
        var result = await _service.PreviewRegexTransformationAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("Regex transformation preview should succeed");
        result.Value.ShouldNotBeNull();
        result.Value.Success.ShouldBeTrue();
        result.Value.EstimatedChanges.ShouldBe(0, "Should have no changes for non-matching pattern");
        result.Value.AffectedFiles.ShouldBeEmpty("Should have no affected files for non-matching pattern");

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
        
        Console.WriteLine($"Test: Cancellation token is cancelled: {cts.Token.IsCancellationRequested}");

        // Act
        var result = await _service.ApplySafeRegexAsync(request, cts.Token);
        
        // Assert
        result.IsFailure.ShouldBeTrue("Operation should fail when cancellation token is triggered");
        result.Error.ShouldContain("Operation was cancelled");

        // Cleanup
        CleanupTestFile(testFile);
    }

    [Fact]
    public async Task ValidateRegexPatternAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var pattern = @"\bclass\s+(\w+)";

        using var cts = new CancellationTokenSource();
        cts.Cancel();
        
        Console.WriteLine($"Test: Cancellation token is cancelled: {cts.Token.IsCancellationRequested}");

        // Act
        var result = await _service.ValidateRegexPatternAsync(pattern, cts.Token);
        
        // Assert
        result.IsFailure.ShouldBeTrue("Operation should fail when cancellation token is triggered");
        result.Error.ShouldContain("Operation was cancelled");
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

    #endregion
}
