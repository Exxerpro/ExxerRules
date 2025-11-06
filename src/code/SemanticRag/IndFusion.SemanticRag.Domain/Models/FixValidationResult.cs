namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Result of fix validation operations.
/// </summary>
/// <param name="Id">Unique identifier for the validation result.</param>
/// <param name="IsValid">Whether the fixes are valid.</param>
/// <param name="NewIssues">New issues introduced by the fixes.</param>
/// <param name="ResolvedIssues">Issues that were resolved by the fixes.</param>
/// <param name="ValidationErrors">Any validation errors.</param>
public record FixValidationResult(
    string Id,
    bool IsValid,
    IEnumerable<AnalyzerDiagnostic> NewIssues,
    IEnumerable<AnalyzerDiagnostic> ResolvedIssues,
    IEnumerable<string> ValidationErrors
);