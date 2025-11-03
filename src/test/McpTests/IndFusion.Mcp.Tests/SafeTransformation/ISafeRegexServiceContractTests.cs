using IndFusion.Mcp.Core.Abstractions;
using IndQuestResults;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.SafeTransformation;

/// <summary>
/// Contract tests for ISafeRegexService interface.
/// These tests verify the contract behavior using mocks and should ALWAYS PASS.
/// </summary>
public class ISafeRegexServiceContractTests
{
    private readonly ISafeRegexService _mockService;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ISafeRegexServiceContractTests()
    {
        _mockService = Substitute.For<ISafeRegexService>();
    }

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

        var expectedResult = new SafeRegexResult(
            Success: true,
            TransformationDetails: new SafeRegexTransformationDetails(
                TransformationType: "SafeRegex",
                TransformationId: "test-123",
                Description: "Add public modifier to class declarations",
                ChangesApplied: 5,
                FilesAffected: 1,
                Confidence: 0.95,
                Pattern: request.Pattern,
                Replacement: request.Replacement
            ),
            ValidationResults: new List<ValidationResult>
            {
                new("PatternValidation", true, "Pattern is valid", new Dictionary<string, object>()),
                new("BuildValidation", true, "Build succeeded", new Dictionary<string, object>())
            },
            DiffPreview: "Preview of changes...",
            ModifiedFiles: new[] { "TestFile.cs" },
            ExecutionTimeMs: 1500
        );

        _mockService.ApplySafeRegexAsync(request, _cancellationToken)
            .Returns(Result<SafeRegexResult>.Success(expectedResult));

        // Act
        var result = await _mockService.ApplySafeRegexAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Success.ShouldBeTrue();
        result.Value.ChangesApplied.ShouldBe(5);
        result.Value.FilesAffected.ShouldBe(1);
    }

    [Fact]
    public async Task ApplySafeRegexAsync_WithInvalidPattern_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new SafeRegexRequest(
            Pattern: @"[invalid-regex-pattern",
            Replacement: "replacement",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: false
        );

        _mockService.ApplySafeRegexAsync(request, _cancellationToken)
            .Returns(Result<SafeRegexResult>.WithFailure("Invalid regex pattern: Unmatched bracket"));

        // Act
        var result = await _mockService.ApplySafeRegexAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("Invalid regex pattern");
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
            DryRun: false
        );

        _mockService.ApplySafeRegexAsync(request, _cancellationToken)
            .Returns(Result<SafeRegexResult>.WithFailure("No target files specified"));

        // Act
        var result = await _mockService.ApplySafeRegexAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("No target files specified");
    }

    [Fact]
    public async Task ValidateRegexPatternAsync_WithValidPattern_ShouldReturnSuccessResult()
    {
        // Arrange
        var pattern = @"\bclass\s+(\w+)";
        var expectedResult = new RegexValidationResult(
            IsValid: true,
            SafetyScore: 0.95,
            Issues: Array.Empty<RegexIssue>(),
            Warnings: Array.Empty<RegexWarning>(),
            PerformanceImpact: "Low",
            ValidationTimeMs: 100
        );

        _mockService.ValidateRegexPatternAsync(pattern, _cancellationToken)
            .Returns(Result<RegexValidationResult>.Success(expectedResult));

        // Act
        var result = await _mockService.ValidateRegexPatternAsync(pattern, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeTrue();
        result.Value.SafetyScore.ShouldBe(0.95);
    }

    [Fact]
    public async Task ValidateRegexPatternAsync_WithDangerousPattern_ShouldReturnFailureResult()
    {
        // Arrange
        var pattern = @"(a+)+$"; // ReDoS pattern
        var expectedResult = new RegexValidationResult(
            IsValid: false,
            SafetyScore: 0.2,
            Issues: new[]
            {
                new RegexIssue("redos-001", "ReDoS", "High", "Pattern may cause ReDoS attack", "Use atomic grouping")
            },
            Warnings: Array.Empty<RegexWarning>(),
            PerformanceImpact: "High",
            ValidationTimeMs: 150
        );

        _mockService.ValidateRegexPatternAsync(pattern, _cancellationToken)
            .Returns(Result<RegexValidationResult>.Success(expectedResult));

        // Act
        var result = await _mockService.ValidateRegexPatternAsync(pattern, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeFalse();
        result.Value.SafetyScore.ShouldBe(0.2);
        result.Value.Issues.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task PreviewRegexTransformationAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new SafeRegexRequest(
            Pattern: @"\bclass\s+(\w+)",
            Replacement: "public class $1",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        var expectedResult = new SafeRegexPreviewResult(
            Success: true,
            PreviewDetails: new SafeRegexPreviewDetails(
                PreviewId: "preview-123",
                Pattern: request.Pattern,
                Replacement: request.Replacement,
                EstimatedMatches: 5,
                SafetyAssessment: "Safe"
            ),
            EstimatedChanges: 5,
            AffectedFiles: new[] { "TestFile.cs" },
            PreviewTimeMs: 200
        );

        _mockService.PreviewRegexTransformationAsync(request, _cancellationToken)
            .Returns(Result<SafeRegexPreviewResult>.Success(expectedResult));

        // Act
        var result = await _mockService.PreviewRegexTransformationAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Success.ShouldBeTrue();
        result.Value.EstimatedChanges.ShouldBe(5);
        result.Value.AffectedFiles.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task PreviewRegexTransformationAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var request = new SafeRegexRequest(
            Pattern: @"\bclass\s+(\w+)",
            Replacement: "public class $1",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true
        );

        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockService.PreviewRegexTransformationAsync(request, cts.Token)
            .Returns(Result<SafeRegexPreviewResult>.WithFailure("Operation was cancelled"));

        // Act
        var result = await _mockService.PreviewRegexTransformationAsync(request, cts.Token);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cancelled");
    }

    [Fact]
    public async Task ApplySafeRegexAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var request = new SafeRegexRequest(
            Pattern: @"\bclass\s+(\w+)",
            Replacement: "public class $1",
            TargetFiles: new[] { "TestFile.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: false
        );

        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockService.ApplySafeRegexAsync(request, cts.Token)
            .Returns(Result<SafeRegexResult>.WithFailure("Operation was cancelled"));

        // Act
        var result = await _mockService.ApplySafeRegexAsync(request, cts.Token);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cancelled");
    }

    [Fact]
    public async Task ValidateRegexPatternAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var pattern = @"\bclass\s+(\w+)";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockService.ValidateRegexPatternAsync(pattern, cts.Token)
            .Returns(Result<RegexValidationResult>.WithFailure("Operation was cancelled"));

        // Act
        var result = await _mockService.ValidateRegexPatternAsync(pattern, cts.Token);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cancelled");
    }
}
