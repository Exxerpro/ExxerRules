namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Individual validation check result.
/// </summary>
/// <param name="CheckType">Type of validation check.</param>
/// <param name="Status">Status of the check (Pass, Fail, Warning).</param>
/// <param name="Message">Message describing the result.</param>
/// <param name="Severity">Severity of any issues found.</param>
public record ValidationCheck(
    string CheckType,
    string Status,
    string Message,
    string Severity
);