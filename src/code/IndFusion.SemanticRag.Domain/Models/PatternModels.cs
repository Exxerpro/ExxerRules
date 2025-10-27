using IndQuestResults;
using System;
using System.Collections.Generic;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a pattern violation found in code analysis.
/// </summary>
/// <param name="Id">Unique identifier for the violation.</param>
/// <param name="PatternId">ID of the pattern that was violated.</param>
/// <param name="PatternName">Name of the violated pattern.</param>
/// <param name="Severity">Severity level of the violation.</param>
/// <param name="Message">Description of the violation.</param>
/// <param name="FilePath">Path to the file where the violation occurred.</param>
/// <param name="LineNumber">Line number where the violation occurred.</param>
/// <param name="ColumnNumber">Column number where the violation occurred.</param>
/// <param name="CodeSnippet">Code snippet that violated the pattern.</param>
/// <param name="Context">Additional context about the violation.</param>
/// <param name="CreatedAt">When the violation was detected.</param>
public readonly record struct PatternViolation(
    string Id,
    string PatternId,
    string PatternName,
    PatternSeverity Severity,
    string Message,
    string? FilePath = null,
    int? LineNumber = null,
    int? ColumnNumber = null,
    string? CodeSnippet = null,
    IReadOnlyDictionary<string, object>? Context = null,
    DateTimeOffset? CreatedAt = null)
{
    /// <summary>
    /// Gets the location string for the violation.
    /// </summary>
    public string Location => !string.IsNullOrWhiteSpace(FilePath) && LineNumber.HasValue
        ? $"{FilePath}:{LineNumber.Value}"
        : FilePath ?? "Unknown location";

    /// <summary>
    /// Checks if the violation has location information.
    /// </summary>
    public bool HasLocation => !string.IsNullOrWhiteSpace(FilePath);

    /// <summary>
    /// Validates the pattern violation.
    /// </summary>
    /// <returns>A Result indicating whether the violation is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Violation ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(PatternId))
            return Result.WithFailure("Pattern ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(PatternName))
            return Result.WithFailure("Pattern name cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Message))
            return Result.WithFailure("Violation message cannot be null or empty");

        if (LineNumber.HasValue && LineNumber.Value < 0)
            return Result.WithFailure("Line number cannot be negative");

        if (ColumnNumber.HasValue && ColumnNumber.Value < 0)
            return Result.WithFailure("Column number cannot be negative");

        return Result.Success();
    }
}

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

/// <summary>
/// Represents a pattern definition for code analysis.
/// </summary>
/// <param name="Id">Unique identifier for the pattern.</param>
/// <param name="Name">Name of the pattern.</param>
/// <param name="Description">Description of what the pattern checks for.</param>
/// <param name="Category">Category of the pattern (e.g., "Performance", "Security", "Maintainability").</param>
/// <param name="Severity">Default severity level for violations of this pattern.</param>
/// <param name="Pattern">The pattern definition (regex, AST pattern, etc.).</param>
/// <param name="Tags">Tags associated with the pattern.</param>
/// <param name="IsEnabled">Whether the pattern is currently enabled.</param>
/// <param name="CreatedAt">When the pattern was created.</param>
/// <param name="UpdatedAt">When the pattern was last updated.</param>
public readonly record struct PatternDefinition(
    string Id,
    string Name,
    string Description,
    string Category,
    PatternSeverity Severity,
    string Pattern,
    IReadOnlyList<string> Tags,
    bool IsEnabled = true,
    DateTimeOffset? CreatedAt = null,
    DateTimeOffset? UpdatedAt = null)
{
    /// <summary>
    /// Validates the pattern definition.
    /// </summary>
    /// <returns>A Result indicating whether the pattern is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Pattern ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Name))
            return Result.WithFailure("Pattern name cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Description))
            return Result.WithFailure("Pattern description cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Category))
            return Result.WithFailure("Pattern category cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Pattern))
            return Result.WithFailure("Pattern definition cannot be null or empty");

        if (Tags is null)
            return Result.WithFailure("Pattern tags cannot be null");

        return Result.Success();
    }

    /// <summary>
    /// Checks if the pattern has a specific tag.
    /// </summary>
    /// <param name="tag">The tag to check for.</param>
    /// <returns>True if the pattern has the tag, otherwise false.</returns>
    public bool HasTag(string tag) => Tags.Contains(tag, StringComparer.OrdinalIgnoreCase);
}

/// <summary>
/// Represents the effort required to implement a suggestion.
/// </summary>
public enum SuggestionEffort
{
    /// <summary>
    /// Low effort - quick fix.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium effort - moderate changes required.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High effort - significant changes required.
    /// </summary>
    High = 2,

    /// <summary>
    /// Very high effort - major refactoring required.
    /// </summary>
    VeryHigh = 3
}

/// <summary>
/// Represents the impact of implementing a suggestion.
/// </summary>
public enum SuggestionImpact
{
    /// <summary>
    /// Low impact - minor improvement.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium impact - moderate improvement.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High impact - significant improvement.
    /// </summary>
    High = 2,

    /// <summary>
    /// Very high impact - major improvement.
    /// </summary>
    VeryHigh = 3
}

/// <summary>
/// Represents a semantic pattern for code analysis.
/// </summary>
/// <param name="Id">Unique identifier for the pattern.</param>
/// <param name="Name">Name of the pattern.</param>
/// <param name="Description">Description of what the pattern matches.</param>
/// <param name="Pattern">The pattern definition (regex, AST pattern, etc.).</param>
/// <param name="Category">Category of the pattern.</param>
/// <param name="Confidence">Confidence level for matches (0.0 to 1.0).</param>
/// <param name="Metadata">Additional metadata about the pattern.</param>
public readonly record struct SemanticPattern(
    string Id,
    string Name,
    string Description,
    string Pattern,
    string Category,
    float Confidence = 0.8f,
    IReadOnlyDictionary<string, object>? Metadata = null)
{
    /// <summary>
    /// Validates the semantic pattern.
    /// </summary>
    /// <returns>A Result indicating whether the pattern is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Pattern ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Name))
            return Result.WithFailure("Pattern name cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Description))
            return Result.WithFailure("Pattern description cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Pattern))
            return Result.WithFailure("Pattern definition cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Category))
            return Result.WithFailure("Pattern category cannot be null or empty");

        if (Confidence < 0.0f || Confidence > 1.0f)
            return Result.WithFailure("Pattern confidence must be between 0.0 and 1.0");

        return Result.Success();
    }
}

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