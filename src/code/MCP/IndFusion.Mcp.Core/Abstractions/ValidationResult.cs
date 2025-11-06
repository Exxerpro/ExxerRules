namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of a validation check.
/// </summary>
/// <param name="CheckName">Name of the validation check.</param>
/// <param name="Passed">Whether the check passed.</param>
/// <param name="Message">Message describing the result.</param>
/// <param name="Details">Additional details about the check.</param>
public record ValidationResult(
    string CheckName,
    bool Passed,
    string Message,
    Dictionary<string, object> Details
);