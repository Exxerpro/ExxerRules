namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Warning about a regex pattern.
/// </summary>
/// <param name="WarningId">Unique identifier for the warning.</param>
/// <param name="WarningType">Type of warning.</param>
/// <param name="Message">Description of the warning.</param>
/// <param name="Recommendation">Recommendation for addressing the warning.</param>
public record RegexWarning(
    string WarningId,
    string WarningType,
    string Message,
    string? Recommendation
);