namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents an anti-pattern violation.
/// </summary>
/// <param name="Id">Unique identifier for the violation.</param>
/// <param name="AntiPatternId">ID of the anti-pattern.</param>
/// <param name="AntiPatternName">Name of the anti-pattern.</param>
/// <param name="Severity">Severity of the violation.</param>
/// <param name="Message">Description of the violation.</param>
/// <param name="FilePath">Path to the file with the violation.</param>
/// <param name="LineNumber">Line number of the violation.</param>
/// <param name="CodeSnippet">Code snippet that violates the pattern.</param>
/// <param name="SuggestedFix">Suggested fix for the violation.</param>
public readonly record struct AntiPatternViolation(
    string Id,
    string AntiPatternId,
    string AntiPatternName,
    PatternSeverity Severity,
    string Message,
    string? FilePath = null,
    int? LineNumber = null,
    string? CodeSnippet = null,
    string? SuggestedFix = null)
{
    /// <summary>
    /// Gets the location string for the violation.
    /// </summary>
    public string Location => !string.IsNullOrWhiteSpace(FilePath) && LineNumber.HasValue
        ? $"{FilePath}:{LineNumber.Value}"
        : FilePath ?? "Unknown location";

    /// <summary>
    /// Validates the anti-pattern violation.
    /// </summary>
    /// <returns>A Result indicating whether the violation is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Violation ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(AntiPatternId))
            return Result.WithFailure("Anti-pattern ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(AntiPatternName))
            return Result.WithFailure("Anti-pattern name cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Message))
            return Result.WithFailure("Violation message cannot be null or empty");

        return Result.Success();
    }
}