namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Result of code fix operations.
/// </summary>
/// <param name="Id">Unique identifier for the fix result.</param>
/// <param name="OriginalCode">Original code before fixes.</param>
/// <param name="FixedCode">Code after applying fixes.</param>
/// <param name="AppliedFixes">List of fixes that were applied.</param>
/// <param name="Success">Whether the fixes were applied successfully.</param>
/// <param name="Errors">Any errors that occurred during fixing.</param>
public record CodeFixResult(
    string Id,
    string OriginalCode,
    string FixedCode,
    IEnumerable<string> AppliedFixes,
    bool Success,
    IEnumerable<string> Errors
);