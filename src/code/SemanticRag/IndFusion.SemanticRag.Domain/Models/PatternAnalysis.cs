namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents the result of pattern analysis.
/// </summary>
/// <param name="PatternType">The type of pattern analyzed.</param>
/// <param name="Matches">Pattern matches found.</param>
/// <param name="Violations">Pattern violations found.</param>
/// <param name="Suggestions">Suggestions for improvement.</param>
/// <param name="Confidence">Overall confidence in the analysis.</param>
/// <param name="AnalysisTimeMs">Time taken to perform the analysis.</param>
public readonly record struct PatternAnalysis(
    string PatternType,
    IReadOnlyList<PatternMatch> Matches,
    IReadOnlyList<PatternViolation> Violations,
    IReadOnlyList<PatternSuggestion> Suggestions,
    float Confidence,
    long AnalysisTimeMs)
{
    /// <summary>
    /// Gets the total number of matches found.
    /// </summary>
    public int MatchCount => Matches.Count;

    /// <summary>
    /// Gets the total number of violations found.
    /// </summary>
    public int ViolationCount => Violations.Count;

    /// <summary>
    /// Gets the total number of suggestions generated.
    /// </summary>
    public int SuggestionCount => Suggestions.Count;

    /// <summary>
    /// Checks if any violations were found.
    /// </summary>
    public bool HasViolations => Violations.Count > 0;

    /// <summary>
    /// Validates the pattern analysis.
    /// </summary>
    /// <returns>A Result indicating whether the analysis is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(PatternType))
            return Result.WithFailure("Pattern type cannot be null or empty");

        if (Confidence < 0.0f || Confidence > 1.0f)
            return Result.WithFailure("Confidence must be between 0.0 and 1.0");

        if (AnalysisTimeMs < 0)
            return Result.WithFailure("Analysis time cannot be negative");

        return Result.Success();
    }
}