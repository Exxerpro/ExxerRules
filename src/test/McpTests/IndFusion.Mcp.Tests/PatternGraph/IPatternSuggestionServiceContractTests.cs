using IndFusion.Mcp.Core.Abstractions;
using IndQuestResults;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.PatternGraph;

/// <summary>
/// Contract tests for IPatternSuggestionService interface.
/// These tests verify the contract behavior using mocks and should ALWAYS PASS.
/// </summary>
public class IPatternSuggestionServiceContractTests
{
	private readonly IPatternSuggestionService _mockService;
	private readonly CancellationToken _cancellationToken = CancellationToken.None;

	public IPatternSuggestionServiceContractTests()
	{
		_mockService = Substitute.For<IPatternSuggestionService>();
	}

	[Fact]
	public async Task SuggestAsync_WithValidRequest_ShouldReturnSuccessResult()
	{
		// Arrange
		var request = new PatternSuggestionRequest(
			ViolationId: "violation-1",
			RuleId: "rule-1",
			CodeSnippet: "public class TestClass { }",
			FilePath: "/test/project/TestClass.cs",
			Context: new Dictionary<string, object> { ["severity"] = "warning" },
			MaxSuggestions: 3,
			ConfidenceThreshold: 0.7);

		var expectedSuggestions = new List<PatternSuggestion>
		{
			new PatternSuggestion(
				Id: "suggestion-1",
				PatternType: "Refactor",
				Description: "Consider using a more descriptive class name",
				CodeExample: "public class DescriptiveClassName { }",
				Confidence: 0.85,
				Effort: "Low",
				Benefits: new[] { "Improved readability", "Better maintainability" },
				Citations: new List<PatternCitation>())
		};

		_mockService.SuggestAsync(request, _cancellationToken)
			.Returns(Result<IReadOnlyCollection<PatternSuggestion>>.Success(expectedSuggestions));

		// Act
		var result = await _mockService.SuggestAsync(request, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsSuccess.ShouldBeTrue();
		result.Value.ShouldNotBeNull();
		result.Value.Count.ShouldBe(1);
		result.Value.First().Id.ShouldBe("suggestion-1");
		result.Value.First().PatternType.ShouldBe("Refactor");
		result.Value.First().Confidence.ShouldBe(0.85);
	}

	[Fact]
	public async Task SuggestAsync_WithNullRequest_ShouldReturnFailureResult()
	{
		// Arrange
		PatternSuggestionRequest? request = null;
		_mockService.SuggestAsync(request!, _cancellationToken)
			.Returns(Result<IReadOnlyCollection<PatternSuggestion>>.WithFailure("Request cannot be null"));

		// Act
		var result = await _mockService.SuggestAsync(request!, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldNotBeNull();
		result.Error.ShouldContain("Request cannot be null");
	}

	[Fact]
	public async Task GetSuggestionAsync_WithValidId_ShouldReturnSuccessResult()
	{
		// Arrange
		var patternId = "suggestion-1";
		var expectedSuggestion = new PatternSuggestion(
			Id: patternId,
			PatternType: "Refactor",
			Description: "Consider using a more descriptive class name",
			CodeExample: "public class DescriptiveClassName { }",
			Confidence: 0.85,
			Effort: "Low",
			Benefits: new[] { "Improved readability", "Better maintainability" },
			Citations: new List<PatternCitation>());

		_mockService.GetSuggestionAsync(patternId, _cancellationToken)
			.Returns(Result<PatternSuggestion>.Success(expectedSuggestion));

		// Act
		var result = await _mockService.GetSuggestionAsync(patternId, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsSuccess.ShouldBeTrue();
		result.Value.ShouldNotBeNull();
		result.Value.Id.ShouldBe(patternId);
		result.Value.PatternType.ShouldBe("Refactor");
	}

	[Fact]
	public async Task GetSuggestionAsync_WithNonExistentId_ShouldReturnFailureResult()
	{
		// Arrange
		var patternId = "non-existent-id";
		_mockService.GetSuggestionAsync(patternId, _cancellationToken)
			.Returns(Result<PatternSuggestion>.WithFailure("Pattern suggestion not found"));

		// Act
		var result = await _mockService.GetSuggestionAsync(patternId, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldNotBeNull();
		result.Error.ShouldContain("not found");
	}

	[Fact]
	public async Task SuggestAsync_WithEmptyRequest_ShouldReturnEmptySuggestions()
	{
		// Arrange
		var request = new PatternSuggestionRequest(
			ViolationId: "",
			RuleId: "",
			CodeSnippet: "",
			FilePath: "",
			MaxSuggestions: 0);

		var expectedSuggestions = new List<PatternSuggestion>();
		_mockService.SuggestAsync(request, _cancellationToken)
			.Returns(Result<IReadOnlyCollection<PatternSuggestion>>.Success(expectedSuggestions));

		// Act
		var result = await _mockService.SuggestAsync(request, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsSuccess.ShouldBeTrue();
		result.Value.ShouldNotBeNull();
		result.Value.Count.ShouldBe(0);
	}

	[Fact]
	public async Task SuggestAsync_WithCancellation_ShouldRespectCancellationToken()
	{
		// Arrange
		var request = new PatternSuggestionRequest(
			ViolationId: "violation-1",
			RuleId: "rule-1",
			CodeSnippet: "test",
			FilePath: "/test.cs");
		var cts = new CancellationTokenSource();
		cts.Cancel();

		_mockService.SuggestAsync(request, cts.Token)
			.Returns(Result<IReadOnlyCollection<PatternSuggestion>>.WithFailure("Operation was cancelled"));

		// Act
		var result = await _mockService.SuggestAsync(request, cts.Token);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldContain("cancelled");
	}
}
