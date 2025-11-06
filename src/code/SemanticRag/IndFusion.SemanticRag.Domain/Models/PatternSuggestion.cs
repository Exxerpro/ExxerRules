namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a suggestion for fixing a pattern violation.
/// </summary>
/// <param name="Id">Unique identifier for the suggestion.</param>
/// <param name="ViolationId">ID of the violation this suggestion addresses.</param>
/// <param name="Title">Title of the suggestion.</param>
/// <param name="Description">Detailed description of the suggestion.</param>
/// <param name="CodeExample">Example code showing how to fix the violation.</param>
/// <param name="Confidence">Confidence level of the suggestion (0.0 to 1.0).</param>
/// <param name="Effort">Estimated effort to implement the suggestion.</param>
/// <param name="Impact">Expected impact of implementing the suggestion.</param>
/// <param name="CreatedAt">When the suggestion was generated.</param>
public readonly record struct PatternSuggestion(
    string Id,
    string ViolationId,
    string Title,
    string Description,
    string? CodeExample = null,
    float Confidence = 0.8f,
    SuggestionEffort Effort = SuggestionEffort.Medium,
    SuggestionImpact Impact = SuggestionImpact.Medium,
    DateTimeOffset? CreatedAt = null)
{
    /// <summary>
    /// Validates the pattern suggestion.
    /// </summary>
    /// <returns>A Result indicating whether the suggestion is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Suggestion ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(ViolationId))
            return Result.WithFailure("Violation ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Title))
            return Result.WithFailure("Suggestion title cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Description))
            return Result.WithFailure("Suggestion description cannot be null or empty");

        if (Confidence < 0.0f || Confidence > 1.0f)
            return Result.WithFailure("Confidence must be between 0.0 and 1.0");

        return Result.Success();
    }
}