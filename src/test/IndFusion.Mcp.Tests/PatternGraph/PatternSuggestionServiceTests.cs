using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Models.PatternGraph;
using IndFusion.Mcp.Core.Services;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.PatternGraph;

/// <summary>
/// TDD implementation tests for PatternSuggestionService.
/// These tests call the REAL service class and should FAIL with NotImplementedException initially.
/// </summary>
public class PatternSuggestionServiceTests
{
    private readonly PatternSuggestionService _service;
    private readonly ILogger<PatternSuggestionService> _logger;

    /// <summary>
    /// Initializes a new instance of the PatternSuggestionServiceTests class.
    /// </summary>
    public PatternSuggestionServiceTests()
    {
        _logger = Substitute.For<ILogger<PatternSuggestionService>>();
        _service = new PatternSuggestionService(_logger);
    }

    /// <summary>
    /// Verifies that SuggestAsync with valid request returns suggestions.
    /// </summary>
    [Fact]
    public async Task SuggestAsync_WithValidRequest_ShouldReturnSuggestions()
    {
        // Arrange
        var request = new PatternSuggestionRequest(
            ViolationId: "violation-1",
            RuleId: "async-rule",
            CodeSnippet: "public async class Test { }",
            FilePath: "/test/Test.cs");
        
        // Act
        var result = await _service.SuggestAsync(request, CancellationToken.None);
        
        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBeGreaterThan(0);
    }

    /// <summary>
    /// Verifies that SuggestAsync with null request returns failure.
    /// </summary>
    [Fact]
    public async Task SuggestAsync_WithNullRequest_ShouldReturnFailure()
    {
        // Arrange
        PatternSuggestionRequest? request = null;
        
        // Act
        var result = await _service.SuggestAsync(request!, CancellationToken.None);
        
        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Pattern suggestion request cannot be null");
    }

    /// <summary>
    /// Verifies that SuggestAsync with cancelled token throws OperationCanceledException.
    /// </summary>
    [Fact]
    public async Task SuggestAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var request = new PatternSuggestionRequest(
            ViolationId: "violation-2",
            RuleId: "rule-2",
            CodeSnippet: "public class Test2 { }",
            FilePath: "/test/Test2.cs");
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        
        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(
            () => _service.SuggestAsync(request, cts.Token));
    }

    /// <summary>
    /// Verifies that GetSuggestionAsync with valid ID returns suggestion.
    /// </summary>
    [Fact]
    public async Task GetSuggestionAsync_WithValidId_ShouldReturnSuggestion()
    {
        // Arrange
        var patternId = "pattern-123";
        
        // Act
        var result = await _service.GetSuggestionAsync(patternId, CancellationToken.None);
        
        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(patternId);
    }

    /// <summary>
    /// Verifies that GetSuggestionAsync with null ID returns failure.
    /// </summary>
    [Fact]
    public async Task GetSuggestionAsync_WithNullId_ShouldReturnFailure()
    {
        // Arrange
        string? patternId = null;
        
        // Act
        var result = await _service.GetSuggestionAsync(patternId!, CancellationToken.None);
        
        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Pattern ID cannot be null or empty");
    }
}
