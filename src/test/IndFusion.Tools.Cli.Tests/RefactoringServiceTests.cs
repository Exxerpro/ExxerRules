using IndFusion.Tools.Cli.Core.Models;
using IndFusion.Tools.Cli.Core.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Tools.Cli.Tests;

/// <summary>
/// Unit tests for RefactoringService
/// </summary>
public class RefactoringServiceTests
{
    /// <summary>
    /// Tests that a refactoring service can be created successfully
    /// </summary>
    [Fact]
    public void Should_Create_Refactoring_Service()
    {
        // Arrange
        var logger = Substitute.For<ILogger<RefactoringService>>();

        // Act
        var service = new RefactoringService(logger);

        // Assert
        service.ShouldNotBeNull();
    }

    /// <summary>
    /// Tests that a refactoring request can be executed successfully
    /// </summary>
    [Fact]
    public async Task Should_Execute_Refactoring_Request()
    {
        // Arrange
        var logger = Substitute.For<ILogger<RefactoringService>>();
        var service = new RefactoringService(logger);

        var request = new RefactoringRequest
        {
            ToolName = "extractmethod",
            FilePath = "test.cs",
            Range = "10:5-12:20",
            NewName = "ExtractedMethod"
        };

        // Act
        var result = await service.ExecuteRefactoringAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        (result.Summary ?? "").ShouldContain("Successfully executed extractmethod refactoring");
    }

    /// <summary>
    /// Tests that a dry run refactoring request is handled correctly
    /// </summary>
    [Fact]
    public async Task Should_Handle_Dry_Run_Request()
    {
        // Arrange
        var logger = Substitute.For<ILogger<RefactoringService>>();
        var service = new RefactoringService(logger);

        var request = new RefactoringRequest
        {
            ToolName = "extractmethod",
            FilePath = "test.cs",
            DryRun = true
        };

        // Act
        var result = await service.ExecuteRefactoringAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Summary.ShouldBe("Successfully executed extractmethod refactoring");
        result.Preview.ShouldNotBeNull();
        result.Preview.ShouldContain("Preview of extractmethod refactoring");
    }

    /// <summary>
    /// Tests that a failing tool is handled gracefully
    /// </summary>
    [Fact]
    public async Task Should_Handle_Failing_Tool()
    {
        // Arrange
        var logger = Substitute.For<ILogger<RefactoringService>>();
        var service = new RefactoringService(logger);

        var request = new RefactoringRequest
        {
            ToolName = "fail",
            FilePath = "test.cs"
        };

        // Act
        var result = await service.ExecuteRefactoringAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        (result.Summary ?? "").ShouldContain("Successfully executed fail refactoring");
    }

    /// <summary>
    /// Tests that cancellation is handled correctly
    /// </summary>
    [Fact]
    public async Task Should_Handle_Cancellation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<RefactoringService>>();
        var service = new RefactoringService(logger);

        var request = new RefactoringRequest
        {
            ToolName = "extractmethod",
            FilePath = "test.cs"
        };

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await service.ExecuteRefactoringAsync(request, cancellationTokenSource.Token);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        (result.ErrorMessage ?? "").ShouldContain("Refactoring failed");
    }
}
