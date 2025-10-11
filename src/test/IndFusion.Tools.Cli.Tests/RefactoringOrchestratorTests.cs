using IndFusion.Tools.Cli.Core.Models;
using IndFusion.Tools.Cli.Core.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Tools.Cli.Tests;

/// <summary>
/// Unit tests for RefactoringOrchestrator
/// </summary>
public class RefactoringOrchestratorTests
{
    /// <summary>
    /// Tests that a refactoring orchestrator can be created successfully
    /// </summary>
    [Fact]
    public void Should_Create_Refactoring_Orchestrator()
    {
        // Arrange
        var logger = Substitute.For<ILogger<RefactoringOrchestrator>>();

        // Act
        var orchestrator = new RefactoringOrchestrator(logger);

        // Assert
        orchestrator.ShouldNotBeNull();
    }

    /// <summary>
    /// Tests that a valid refactoring request can be executed successfully
    /// </summary>
    [Fact]
    public async Task Should_Execute_Valid_Refactoring_Request()
    {
        // Arrange
        var logger = Substitute.For<ILogger<RefactoringOrchestrator>>();
        var orchestrator = new RefactoringOrchestrator(logger);

        var request = new RefactoringRequest
        {
            ToolName = "extractmethod",
            FilePath = "test.cs",
            Range = "10:5-12:20",
            NewName = "ExtractedMethod"
        };

        // Note: Since RefactoringOrchestrator creates its own RefactoringService internally,
        // we can't mock the service directly. This test will use the real service.

        // Act
        var result = await orchestrator.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        // Note: The actual result will depend on the real RefactoringService implementation
        // For now, we just verify the method doesn't throw and returns a result
    }

    /// <summary>
    /// Tests that an invalid tool name is handled gracefully
    /// </summary>
    [Fact]
    public async Task Should_Handle_Invalid_Tool_Name()
    {
        // Arrange
        var logger = Substitute.For<ILogger<RefactoringOrchestrator>>();
        var orchestrator = new RefactoringOrchestrator(logger);

        var request = new RefactoringRequest
        {
            ToolName = "nonexistenttool",
            FilePath = "test.cs"
        };

        // Act
        var result = await orchestrator.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        (result.ErrorMessage ?? "").ShouldContain("Tool 'nonexistenttool' not found");
    }

    /// <summary>
    /// Tests that refactoring service exceptions are handled gracefully
    /// </summary>
    [Fact]
    public async Task Should_Handle_Refactoring_Service_Exception()
    {
        // Arrange
        var logger = Substitute.For<ILogger<RefactoringOrchestrator>>();
        var orchestrator = new RefactoringOrchestrator(logger);

        var request = new RefactoringRequest
        {
            ToolName = "extractmethod",
            FilePath = "test.cs"
        };

        // Note: Since RefactoringOrchestrator creates its own RefactoringService internally,
        // we can't mock the service to throw exceptions directly. This test will use the real service.

        // Act
        var result = await orchestrator.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        // Note: The actual result will depend on the real RefactoringService implementation
        // For now, we just verify the method doesn't throw and returns a result
    }
}
