namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Diagnostic result from an analyzer.
/// </summary>
/// <param name="Id">Unique identifier for the diagnostic.</param>
/// <param name="RuleId">ID of the rule that generated this diagnostic.</param>
/// <param name="Severity">Severity level of the diagnostic.</param>
/// <param name="Message">Diagnostic message.</param>
/// <param name="FilePath">Path to the file where the issue was found.</param>
/// <param name="LineNumber">Line number where the issue was found.</param>
/// <param name="ColumnNumber">Column number where the issue was found.</param>
/// <param name="Properties">Additional diagnostic properties.</param>
public record AnalyzerDiagnostic(
    string Id,
    string RuleId,
    string Severity,
    string Message,
    string FilePath,
    int LineNumber,
    int ColumnNumber,
    Dictionary<string, object> Properties
);
