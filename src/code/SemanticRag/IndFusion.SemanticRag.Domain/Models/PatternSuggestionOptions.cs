namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Options for pattern suggestion operations.
/// </summary>
/// <param name="MaxSuggestions">Maximum number of suggestions to return.</param>
/// <param name="MinConfidence">Minimum confidence threshold for suggestions.</param>
/// <param name="Categories">Optional categories to filter by.</param>
/// <param name="IncludeCodeExamples">Whether to include code examples in suggestions.</param>
/// <param name="IncludeEffortEstimate">Whether to include effort estimates.</param>
public readonly record struct PatternSuggestionOptions(
    int MaxSuggestions = 10,
    float MinConfidence = 0.5f,
    IReadOnlyList<string>? Categories = null,
    bool IncludeCodeExamples = true,
    bool IncludeEffortEstimate = true)
{
    /// <summary>
    /// Validates the pattern suggestion options.
    /// </summary>
    /// <returns>A Result indicating whether the options are valid.</returns>
    public Result Validate()
    {
        if (MaxSuggestions <= 0)
            return Result.WithFailure("Max suggestions must be greater than 0");

        if (MinConfidence < 0.0f || MinConfidence > 1.0f)
            return Result.WithFailure("Min confidence must be between 0.0 and 1.0");

        return Result.Success();
    }
}