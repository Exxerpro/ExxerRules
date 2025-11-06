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
