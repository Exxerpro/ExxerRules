using IndQuestResults;

namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Service for generating and managing pattern suggestions based on code analysis.
/// Provides intelligent recommendations for code improvements and refactoring opportunities.
/// </summary>
public interface IPatternSuggestionService
{
	/// <summary>
	/// Generates pattern suggestions based on the provided request context.
	/// </summary>
	/// <param name="request">The pattern suggestion request containing context and criteria.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result containing the collection of pattern suggestions or failure information.</returns>
	Task<Result<IReadOnlyCollection<PatternSuggestion>>> SuggestAsync(
		PatternSuggestionRequest request, 
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Retrieves a specific pattern suggestion by its unique identifier.
	/// </summary>
	/// <param name="patternId">The unique identifier of the pattern suggestion.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result containing the pattern suggestion or failure information.</returns>
	Task<Result<PatternSuggestion>> GetSuggestionAsync(
		string patternId, 
		CancellationToken cancellationToken = default);
}