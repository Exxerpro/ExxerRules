namespace IndFusion.Mcp.Mcp.Core.Abstractions;

/// <summary>
/// Represents the outcome of a refactoring operation, including success state,
/// human-readable messages, updated code snippets and any affected files.
/// </summary>
/// <param name="Success">Indicates whether the operation completed successfully.</param>
/// <param name="Message">Message describing the result or error.</param>
/// <param name="UpdatedCode">Optional updated code returned by the tool, if applicable.</param>
/// <param name="ModifiedFiles">Optional list of files that were modified by the operation.</param>
/// <param name="ErrorDetails">Raw error details, including stack traces, when failures occur.</param>
public record ExxerFactoringResult(
    bool Success,
    string Message,
    string? UpdatedCode = null,
    IEnumerable<string>? ModifiedFiles = null,
    string? ErrorDetails = null
);
