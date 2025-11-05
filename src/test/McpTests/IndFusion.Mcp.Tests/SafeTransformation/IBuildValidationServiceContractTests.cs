using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Models.PatternGraph;
using IndQuestResults;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.SafeTransformation;

/// <summary>
/// Contract tests for IBuildValidationService interface.
/// These tests verify the contract behavior using mocks and should ALWAYS PASS.
/// </summary>
public class IBuildValidationServiceContractTests
{
    private readonly IBuildValidationService _mockService;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    /// <summary>
    /// Initializes the contract tests with a mocked IBuildValidationService.
    /// </summary>
    public IBuildValidationServiceContractTests()
    {
        _mockService = Substitute.For<IBuildValidationService>();
    }

    /// <summary>
    /// Verifies build validation succeeds for a valid transformation request.
    /// </summary>
    [Fact]
    public async Task ValidateTransformationAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new BuildValidationRequest(
            SolutionPath: "TestSolution.sln",
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

        var expectedResult = new BuildValidationResult(
            IsValid: true,
            BuildSuccess: true,
            ValidationChecks: new[]
            {
                new ValidationCheck("BuildCheck", "Pass", "Build succeeded", "Info"),
                new ValidationCheck("AnalyzerCheck", "Pass", "No new issues found", "Info")
            },
            NewIssues: Array.Empty<TransformationIssue>(),
            AnalyzerResults: new[]
            {
                new AnalyzerResult("EXXER001", 0, new Dictionary<string, int> { { "Error", 0 } }, 100)
            },
            ValidationTimeMs: 1500
        );

        _mockService.ValidateTransformationAsync(request, _cancellationToken)
            .Returns(Result<BuildValidationResult>.Success(expectedResult));

        // Act
        var result = await _mockService.ValidateTransformationAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeTrue();
        result.Value.BuildSuccess.ShouldBeTrue();
    }

    /// <summary>
    /// Ensures build validation reports failures when the build cannot succeed.
    /// </summary>
    [Fact]
    public async Task ValidateTransformationAsync_WithBuildFailure_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new BuildValidationRequest(
            SolutionPath: "TestSolution.sln",
            TransformedFiles: new[]
            {
                new TransformedFile(
                    FilePath: "TestFile.cs",
                    OriginalContent: "class Test { }",
                    TransformedContent: "invalid c# code",
                    TransformationType: "SafeRegex"
                )
            },
            ValidationOptions: new TransformationValidationOptions(),
            RunAnalyzers: true,
            BuildValidation: true,
            CheckForNewIssues: true
        );

        var expectedResult = new BuildValidationResult(
            IsValid: false,
            BuildSuccess: false,
            ValidationChecks: new[]
            {
                new ValidationCheck("BuildCheck", "Fail", "Build failed with compilation errors", "Error")
            },
            NewIssues: new[]
            {
                new TransformationIssue(
                    IssueId: "compilation-001",
                    IssueType: "CompilationError",
                    Severity: "Error",
                    Message: "Invalid syntax",
                    Location: new IndFusion.Mcp.Core.Abstractions.SourceLocation("TestFile.cs", 1, 1, 1),
                    SuggestedFix: "Fix syntax error"
                )
            },
            AnalyzerResults: Array.Empty<AnalyzerResult>(),
            ValidationTimeMs: 800
        );

        _mockService.ValidateTransformationAsync(request, _cancellationToken)
            .Returns(Result<BuildValidationResult>.Success(expectedResult));

        // Act
        var result = await _mockService.ValidateTransformationAsync(request, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeFalse();
        result.Value.BuildSuccess.ShouldBeFalse();
        result.Value.NewIssues.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Confirms file-level validation succeeds when the transformed file is valid.
    /// </summary>
    [Fact]
    public async Task ValidateFileTransformationAsync_WithValidFile_ShouldReturnSuccessResult()
    {
        // Arrange
        var filePath = "TestFile.cs";
        var originalContent = "class Test { }";
        var transformedContent = "public class Test { }";
        var validationOptions = new TransformationValidationOptions();

        var expectedResult = new FileValidationResult(
            IsValid: true,
            FilePath: filePath,
            ValidationChecks: new[]
            {
                new ValidationCheck("SyntaxCheck", "Pass", "Syntax is valid", "Info"),
                new ValidationCheck("BuildCheck", "Pass", "File compiles successfully", "Info")
            },
            NewIssues: Array.Empty<TransformationIssue>(),
            ValidationTimeMs: 200
        );

        _mockService.ValidateFileTransformationAsync(filePath, originalContent, transformedContent, validationOptions, _cancellationToken)
            .Returns(Result<FileValidationResult>.Success(expectedResult));

        // Act
        var result = await _mockService.ValidateFileTransformationAsync(filePath, originalContent, transformedContent, validationOptions, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeTrue();
        result.Value.FilePath.ShouldBe(filePath);
    }

    /// <summary>
    /// Ensures file validation reports a failure when the file content is invalid.
    /// </summary>
    [Fact]
    public async Task ValidateFileTransformationAsync_WithInvalidFile_ShouldReturnFailureResult()
    {
        // Arrange
        var filePath = "TestFile.cs";
        var originalContent = "class Test { }";
        var transformedContent = "invalid c# code";
        var validationOptions = new TransformationValidationOptions();

        var expectedResult = new FileValidationResult(
            IsValid: false,
            FilePath: filePath,
            ValidationChecks: new[]
            {
                new ValidationCheck("SyntaxCheck", "Fail", "Invalid syntax", "Error")
            },
            NewIssues: new[]
            {
                new TransformationIssue(
                    IssueId: "syntax-001",
                    IssueType: "SyntaxError",
                    Severity: "Error",
                    Message: "Invalid C# syntax",
                    Location: new IndFusion.Mcp.Core.Abstractions.SourceLocation(filePath, 1, 1, 1),
                    SuggestedFix: "Fix syntax error"
                )
            },
            ValidationTimeMs: 150
        );

        _mockService.ValidateFileTransformationAsync(filePath, originalContent, transformedContent, validationOptions, _cancellationToken)
            .Returns(Result<FileValidationResult>.Success(expectedResult));

        // Act
        var result = await _mockService.ValidateFileTransformationAsync(filePath, originalContent, transformedContent, validationOptions, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeFalse();
        result.Value.NewIssues.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task CreateTemporaryWorkspaceAsync_WithValidSolutionPath_ShouldReturnSuccessResult()
    {
        // Arrange
        var solutionPath = "TestSolution.sln";
        var expectedWorkspace = new TemporaryWorkspace(
            WorkspacePath: "/tmp/workspace-123",
            OriginalSolutionPath: solutionPath,
            CreatedAt: DateTime.UtcNow,
            ExpiresAt: DateTime.UtcNow.AddHours(1)
        );

        _mockService.CreateTemporaryWorkspaceAsync(solutionPath, _cancellationToken)
            .Returns(Result<TemporaryWorkspace>.Success(expectedWorkspace));

        // Act
        var result = await _mockService.CreateTemporaryWorkspaceAsync(solutionPath, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.OriginalSolutionPath.ShouldBe(solutionPath);
        result.Value.WorkspacePath.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateTemporaryWorkspaceAsync_WithInvalidSolutionPath_ShouldReturnFailureResult()
    {
        // Arrange
        var solutionPath = "NonExistent.sln";

        _mockService.CreateTemporaryWorkspaceAsync(solutionPath, _cancellationToken)
            .Returns(Result<TemporaryWorkspace>.WithFailure("Solution file not found"));

        // Act
        var result = await _mockService.CreateTemporaryWorkspaceAsync(solutionPath, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("not found");
    }

    [Fact]
    public async Task CleanupTemporaryWorkspaceAsync_WithValidWorkspace_ShouldReturnSuccessResult()
    {
        // Arrange
        var workspace = new TemporaryWorkspace(
            WorkspacePath: "/tmp/workspace-123",
            OriginalSolutionPath: "TestSolution.sln",
            CreatedAt: DateTime.UtcNow.AddHours(-1),
            ExpiresAt: DateTime.UtcNow.AddHours(1)
        );

        _mockService.CleanupTemporaryWorkspaceAsync(workspace, _cancellationToken)
            .Returns(Result.Success());

        // Act
        var result = await _mockService.CleanupTemporaryWorkspaceAsync(workspace, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task CleanupTemporaryWorkspaceAsync_WithNonExistentWorkspace_ShouldReturnSuccessResult()
    {
        // Arrange
        var workspace = new TemporaryWorkspace(
            WorkspacePath: "/tmp/non-existent",
            OriginalSolutionPath: "TestSolution.sln",
            CreatedAt: DateTime.UtcNow.AddHours(-1),
            ExpiresAt: DateTime.UtcNow.AddHours(1)
        );

        _mockService.CleanupTemporaryWorkspaceAsync(workspace, _cancellationToken)
            .Returns(Result.Success());

        // Act
        var result = await _mockService.CleanupTemporaryWorkspaceAsync(workspace, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateTransformationAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var request = new BuildValidationRequest(
            SolutionPath: "TestSolution.sln",
            TransformedFiles: Array.Empty<TransformedFile>(),
            ValidationOptions: new TransformationValidationOptions(),
            RunAnalyzers: true,
            BuildValidation: true,
            CheckForNewIssues: true
        );

        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockService.ValidateTransformationAsync(request, cts.Token)
            .Returns(Result<BuildValidationResult>.WithFailure("Operation was cancelled"));

        // Act
        var result = await _mockService.ValidateTransformationAsync(request, cts.Token);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cancelled");
    }

    [Fact]
    public async Task ValidateFileTransformationAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var filePath = "TestFile.cs";
        var originalContent = "class Test { }";
        var transformedContent = "public class Test { }";
        var validationOptions = new TransformationValidationOptions();

        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockService.ValidateFileTransformationAsync(filePath, originalContent, transformedContent, validationOptions, cts.Token)
            .Returns(Result<FileValidationResult>.WithFailure("Operation was cancelled"));

        // Act
        var result = await _mockService.ValidateFileTransformationAsync(filePath, originalContent, transformedContent, validationOptions, cts.Token);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cancelled");
    }

    [Fact]
    public async Task CreateTemporaryWorkspaceAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var solutionPath = "TestSolution.sln";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockService.CreateTemporaryWorkspaceAsync(solutionPath, cts.Token)
            .Returns(Result<TemporaryWorkspace>.WithFailure("Operation was cancelled"));

        // Act
        var result = await _mockService.CreateTemporaryWorkspaceAsync(solutionPath, cts.Token);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cancelled");
    }

    [Fact]
    public async Task CleanupTemporaryWorkspaceAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var workspace = new TemporaryWorkspace(
            WorkspacePath: "/tmp/workspace-123",
            OriginalSolutionPath: "TestSolution.sln",
            CreatedAt: DateTime.UtcNow.AddHours(-1),
            ExpiresAt: DateTime.UtcNow.AddHours(1)
        );

        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockService.CleanupTemporaryWorkspaceAsync(workspace, cts.Token)
            .Returns(Result.WithFailure("Operation was cancelled"));

        // Act
        var result = await _mockService.CleanupTemporaryWorkspaceAsync(workspace, cts.Token);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cancelled");
    }
}
