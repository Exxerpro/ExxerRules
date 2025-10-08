using IndQuestResults;

namespace IndTrace.DataStore.Interfaces;

/// <summary>
/// Defines a validator for fixture post-execution state.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IFixtureValidator logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IFixtureValidator
{
    /// <summary>
    /// Validates the post-execution state of a fixture asynchronously.
    /// </summary>
    /// <param name="context">The fixture context to validate.</param>
    /// <returns>A result indicating the outcome of the validation.</returns>
    Task<IndQuestResults.Result> ValidatePostExecutionStateAsync(IFixtureContext context);
}
