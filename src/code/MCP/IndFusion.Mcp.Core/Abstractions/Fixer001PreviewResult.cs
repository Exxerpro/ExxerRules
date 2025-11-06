namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of Fixer001 preview operations.
/// </summary>
/// <param name="Success">Whether the preview succeeded.</param>
/// <param name="PreviewDetails">Details about the preview.</param>
/// <param name="EstimatedChanges">Estimated number of changes.</param>
/// <param name="AffectedFiles">Files that would be affected.</param>
/// <param name="PreviewTimeMs">Time taken for the preview.</param>
/// <param name="ErrorDetails">Error details if preview failed.</param>
public record Fixer001PreviewResult(
    bool Success,
    Fixer001PreviewDetails PreviewDetails,
    int EstimatedChanges,
    IEnumerable<string> AffectedFiles,
    long PreviewTimeMs,
    string? ErrorDetails = null
);