namespace ExxerFactor.Mcp.Core.Abstractions;

/// <summary>
/// Represents the outcome of a refactoring operation, including success state,
/// human-readable messages, updated code snippets and any affected files.
/// </summary>
public record ExxerFactoringResult(
    /// <summary>Indicates whether the operation completed successfully.</summary>
    bool Success,
    /// <summary>Message describing the result or error.</summary>
    string Message,
    /// <summary>Optional updated code returned by the tool, if applicable.</summary>
    string? UpdatedCode = null,
    /// <summary>Optional list of files that were modified by the operation.</summary>
    IEnumerable<string>? ModifiedFiles = null,
    /// <summary>Raw error details, including stack traces, when failures occur.</summary>
    string? ErrorDetails = null
);