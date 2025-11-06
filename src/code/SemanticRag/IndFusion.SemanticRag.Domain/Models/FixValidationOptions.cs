namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Options for fix validation operations.
/// <param name="ValidateSyntax">Whether to validate syntax.</param>
/// <param name="ValidateSemantics">Whether to validate semantics.</param>
/// <param name="RunAnalyzers">Whether to run analyzers for validation.</param>
/// </summary>
public record FixValidationOptions(
    bool ValidateSyntax = true,
    bool ValidateSemantics = true,
    bool RunAnalyzers = true
);