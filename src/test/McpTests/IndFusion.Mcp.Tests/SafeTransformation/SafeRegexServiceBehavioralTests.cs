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

    /// <summary>
    /// Creates a configured SafeRegexService with real dependencies for behavioral testing.
    /// </summary>
    /// <remarks>
    /// The constructor wires up concrete logging and build-validation services so each test exercises the production pipeline end to end.
    /// </remarks>
    public SafeRegexServiceBehavioralTests()
    {
        // Use simple logger factory
        var loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<SafeRegexService>();
        _buildValidationService = new BuildValidationService(loggerFactory.CreateLogger<BuildValidationService>());
        _service = new SafeRegexService(_logger, _buildValidationService);
    }

    /// <summary>
    /// Verifies that applying a well-formed safe regex request reports success and transformation details.
    /// </summary>
    /// <remarks>
    /// The test mimics a user file on disk and expects the service to acknowledge the pattern, replacement, and overall success flags.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing asynchronous test execution.</returns>
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

    /// <summary>
    /// Ensures invalid regex patterns cause the safe regex operation to return a failure result.
    /// </summary>
    /// <remarks>
    /// The service should validate patterns before executing them and surface the parser error so callers can correct the input.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing asynchronous test execution.</returns>
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

    /// <summary>
    /// Confirms the service rejects transformation requests that do not specify any target files.
    /// </summary>
    /// <remarks>
    /// Target files are required to calculate diffs; the service should therefore stop early and report the configuration problem.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing asynchronous test execution.</returns>
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

    /// <summary>
    /// Ensures valid regex patterns pass validation with a positive safety score.
    /// </summary>
    /// <remarks>
    /// A typical pattern should evaluate as safe, providing callers with confidence to proceed to preview or execution.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing asynchronous test execution.</returns>
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

    /// <summary>
    /// Verifies invalid regex syntax is detected and surfaced with validation issues.
    /// </summary>
    /// <remarks>
    /// Even though validation returns success, it should flag the pattern as invalid and supply detailed issues to the caller.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing asynchronous test execution.</returns>
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

    /// <summary>
    /// Checks that patterns susceptible to ReDoS are flagged with warnings and reduced safety scores.
    /// </summary>
    /// <remarks>
    /// ReDoS-prone patterns must be downgraded in safety so that tooling can discourage unsafe transformations.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing asynchronous test execution.</returns>
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

    /// <summary>
    /// Verifies previewing a valid transformation succeeds and returns estimated changes.
    /// </summary>
    /// <remarks>
    /// Preview results should echo back the original pattern metadata and provide change estimates without modifying files.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing asynchronous test execution.</returns>
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

    /// <summary>
    /// Ensures the preview operation succeeds even when no files match the pattern.
    /// </summary>
    /// <remarks>
    /// The service should communicate that no files were affected while still succeeding, mirroring typical user expectations.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing asynchronous test execution.</returns>
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

    /// <summary>
    /// Confirms the safe regex operation honors cancellation requests and reports the cancellation.
    /// </summary>
    /// <remarks>
    /// Cancelling before execution should prevent any filesystem writes and return a descriptive cancellation error.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing asynchronous test execution.</returns>
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

    /// <summary>
    /// Validates cancellation is respected when checking regex safety.
    /// </summary>
    /// <remarks>
    /// Validation should stop promptly when cancellation is requested so callers can maintain responsive experiences.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing asynchronous test execution.</returns>
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

    //  Private Helper Methods

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

     // 
}
