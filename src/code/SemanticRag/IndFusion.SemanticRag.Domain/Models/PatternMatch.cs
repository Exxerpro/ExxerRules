namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a pattern match result.
/// </summary>
/// <param name="Pattern">The pattern that was matched.</param>
/// <param name="Match">The matched text.</param>
/// <param name="StartIndex">Starting position of the match.</param>
/// <param name="EndIndex">Ending position of the match.</param>
/// <param name="Confidence">Confidence score for the match.</param>
public readonly record struct PatternMatch(
    SemanticPattern Pattern,
    string Match,
    int StartIndex,
    int EndIndex,
    float Confidence)
{
    /// <summary>
    /// Gets the length of the match.
    /// </summary>
    public int Length => EndIndex - StartIndex;

    /// <summary>
    /// Validates the pattern match.
    /// </summary>
    /// <returns>A Result indicating whether the match is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Match))
            return Result.WithFailure("Match text cannot be null or empty");

        if (StartIndex < 0)
            return Result.WithFailure("Start index cannot be negative");

        if (EndIndex < StartIndex)
            return Result.WithFailure("End index cannot be less than start index");

        if (Confidence < 0.0f || Confidence > 1.0f)
            return Result.WithFailure("Confidence must be between 0.0 and 1.0");

        return Result.Success();
    }
}