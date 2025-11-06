namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// A validation step between services.
/// </summary>
/// <param name="ValidationName">Name of the validation.</param>
/// <param name="ValidationType">Type of validation.</param>
/// <param name="ValidationRules">Rules for validation.</param>
/// <param name="ErrorAction">Action to take on validation error.</param>
public record ValidationStep(
    string ValidationName,
    string ValidationType,
    IEnumerable<ValidationRule> ValidationRules,
    string ErrorAction = "Stop"
);