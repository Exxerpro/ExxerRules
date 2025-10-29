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

/// <summary>
/// Represents a query for pattern graph operations.
/// </summary>
/// <param name="Query">The query string.</param>
/// <param name="Parameters">Optional parameters for the query.</param>
/// <param name="MaxResults">Maximum number of results to return.</param>
/// <param name="TimeoutMs">Query timeout in milliseconds.</param>
public readonly record struct PatternGraphQuery(
    string Query,
    IReadOnlyDictionary<string, object>? Parameters = null,
    int MaxResults = 100,
    int TimeoutMs = 30000)
{
    /// <summary>
    /// Validates the pattern graph query.
    /// </summary>
    /// <returns>A Result indicating whether the query is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Query))
            return Result.WithFailure("Query cannot be null or empty");

        if (MaxResults <= 0)
            return Result.WithFailure("Max results must be greater than 0");

        if (TimeoutMs <= 0)
            return Result.WithFailure("Timeout must be greater than 0");

        return Result.Success();
    }
}

/// <summary>
/// Represents the result of a pattern graph query.
/// </summary>
/// <param name="Patterns">Patterns found by the query.</param>
/// <param name="Relationships">Relationships found by the query.</param>
/// <param name="ExecutionTimeMs">Time taken to execute the query.</param>
/// <param name="TotalResults">Total number of results found.</param>
/// <param name="HasMoreResults">Whether there are more results available.</param>
public readonly record struct PatternGraphResult(
    IReadOnlyList<PatternDefinition> Patterns,
    IReadOnlyList<PatternRelationship> Relationships,
    long ExecutionTimeMs,
    int TotalResults,
    bool HasMoreResults)
{
    /// <summary>
    /// Gets the number of patterns found.
    /// </summary>
    public int PatternCount => Patterns.Count;

    /// <summary>
    /// Gets the number of relationships found.
    /// </summary>
    public int RelationshipCount => Relationships.Count;

    /// <summary>
    /// Validates the pattern graph result.
    /// </summary>
    /// <returns>A Result indicating whether the result is valid.</returns>
    public Result Validate()
    {
        if (ExecutionTimeMs < 0)
            return Result.WithFailure("Execution time cannot be negative");

        if (TotalResults < 0)
            return Result.WithFailure("Total results cannot be negative");

        return Result.Success();
    }
}

/// <summary>
/// Represents a relationship between patterns.
/// </summary>
/// <param name="Id">Unique identifier for the relationship.</param>
/// <param name="Type">Type of the relationship.</param>
/// <param name="SourcePatternId">ID of the source pattern.</param>
/// <param name="TargetPatternId">ID of the target pattern.</param>
/// <param name="Properties">Properties of the relationship.</param>
/// <param name="Strength">Strength of the relationship (0.0 to 1.0).</param>
public readonly record struct PatternRelationship(
    string Id,
    string Type,
    string SourcePatternId,
    string TargetPatternId,
    IReadOnlyDictionary<string, object> Properties,
    float Strength = 1.0f)
{
    /// <summary>
    /// Validates the pattern relationship.
    /// </summary>
    /// <returns>A Result indicating whether the relationship is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Relationship ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Type))
            return Result.WithFailure("Relationship type cannot be null or empty");

        if (string.IsNullOrWhiteSpace(SourcePatternId))
            return Result.WithFailure("Source pattern ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(TargetPatternId))
            return Result.WithFailure("Target pattern ID cannot be null or empty");

        if (Strength < 0.0f || Strength > 1.0f)
            return Result.WithFailure("Strength must be between 0.0 and 1.0");

        return Result.Success();
    }
}

/// <summary>
/// Represents similarity between two patterns.
/// </summary>
/// <param name="PatternId">ID of the similar pattern.</param>
/// <param name="SimilarityScore">Similarity score (0.0 to 1.0).</param>
/// <param name="SimilarityType">Type of similarity (semantic, structural, etc.).</param>
/// <param name="CommonElements">Elements common between the patterns.</param>
public readonly record struct PatternSimilarity(
    string PatternId,
    float SimilarityScore,
    string SimilarityType,
    IReadOnlyList<string> CommonElements)
{
    /// <summary>
    /// Validates the pattern similarity.
    /// </summary>
    /// <returns>A Result indicating whether the similarity is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(PatternId))
            return Result.WithFailure("Pattern ID cannot be null or empty");

        if (SimilarityScore < 0.0f || SimilarityScore > 1.0f)
            return Result.WithFailure("Similarity score must be between 0.0 and 1.0");

        if (string.IsNullOrWhiteSpace(SimilarityType))
            return Result.WithFailure("Similarity type cannot be null or empty");

        return Result.Success();
    }
}

/// <summary>
/// Represents usage statistics for a pattern.
/// </summary>
/// <param name="PatternId">ID of the pattern.</param>
/// <param name="UsageCount">Number of times the pattern is used.</param>
/// <param name="FileCount">Number of files using the pattern.</param>
/// <param name="ProjectCount">Number of projects using the pattern.</param>
/// <param name="LastUsed">When the pattern was last used.</param>
/// <param name="Trend">Usage trend over time.</param>
public readonly record struct PatternUsageStatistics(
    string PatternId,
    int UsageCount,
    int FileCount,
    int ProjectCount,
    DateTimeOffset? LastUsed,
    UsageTrend Trend)
{
    /// <summary>
    /// Gets the average usage per file.
    /// </summary>
    public double AverageUsagePerFile => FileCount > 0 ? (double)UsageCount / FileCount : 0.0;

    /// <summary>
    /// Gets the average usage per project.
    /// </summary>
    public double AverageUsagePerProject => ProjectCount > 0 ? (double)UsageCount / ProjectCount : 0.0;

    /// <summary>
    /// Validates the pattern usage statistics.
    /// </summary>
    /// <returns>A Result indicating whether the statistics are valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(PatternId))
            return Result.WithFailure("Pattern ID cannot be null or empty");

        if (UsageCount < 0)
            return Result.WithFailure("Usage count cannot be negative");

        if (FileCount < 0)
            return Result.WithFailure("File count cannot be negative");

        if (ProjectCount < 0)
            return Result.WithFailure("Project count cannot be negative");

        return Result.Success();
    }
}

/// <summary>
/// Represents usage trend over time.
/// </summary>
public enum UsageTrend
{
    /// <summary>
    /// Usage is increasing.
    /// </summary>
    Increasing,

    /// <summary>
    /// Usage is decreasing.
    /// </summary>
    Decreasing,

    /// <summary>
    /// Usage is stable.
    /// </summary>
    Stable,

    /// <summary>
    /// Usage trend is unknown.
    /// </summary>
    Unknown
}

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

/// <summary>
/// Represents the evolution of a pattern over time.
/// </summary>
/// <param name="PatternId">ID of the pattern.</param>
/// <param name="Version">Version of the pattern.</param>
/// <param name="ChangeType">Type of change made.</param>
/// <param name="ChangeDescription">Description of the change.</param>
/// <param name="ChangedAt">When the change was made.</param>
/// <param name="ChangedBy">Who made the change.</param>
public readonly record struct PatternEvolution(
    string PatternId,
    string Version,
    PatternChangeType ChangeType,
    string ChangeDescription,
    DateTimeOffset ChangedAt,
    string? ChangedBy = null)
{
    /// <summary>
    /// Validates the pattern evolution.
    /// </summary>
    /// <returns>A Result indicating whether the evolution is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(PatternId))
            return Result.WithFailure("Pattern ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Version))
            return Result.WithFailure("Version cannot be null or empty");

        if (string.IsNullOrWhiteSpace(ChangeDescription))
            return Result.WithFailure("Change description cannot be null or empty");

        return Result.Success();
    }
}

/// <summary>
/// Represents the type of change made to a pattern.
/// </summary>
public enum PatternChangeType
{
    /// <summary>
    /// Pattern was created.
    /// </summary>
    Created,

    /// <summary>
    /// Pattern was updated.
    /// </summary>
    Updated,

    /// <summary>
    /// Pattern was deprecated.
    /// </summary>
    Deprecated,

    /// <summary>
    /// Pattern was removed.
    /// </summary>
    Removed,

    /// <summary>
    /// Pattern severity was changed.
    /// </summary>
    SeverityChanged,

    /// <summary>
    /// Pattern category was changed.
    /// </summary>
    CategoryChanged
}