namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Options for analyzer execution.
/// </summary>
/// <param name="Analyzers">List of analyzer IDs to run.</param>
/// <param name="Severity">Minimum severity level to report.</param>
/// <param name="IncludeWarnings">Whether to include warnings.</param>
/// <param name="MaxConcurrency">Maximum number of concurrent analyzers.</param>
public record AnalyzerExecutionOptions(
    IEnumerable<string> Analyzers,
    string Severity = "Error",
    bool IncludeWarnings = true,
    int MaxConcurrency = 4
);