namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Metadata about an analyzer.
/// </summary>
/// <param name="Id">Unique identifier for the analyzer.</param>
/// <param name="Name">Display name of the analyzer.</param>
/// <param name="Description">Description of what the analyzer does.</param>
/// <param name="Version">Version of the analyzer.</param>
/// <param name="SupportedLanguages">Languages supported by the analyzer.</param>
/// <param name="Rules">Rules provided by the analyzer.</param>
public record AnalyzerMetadata(
    string Id,
    string Name,
    string Description,
    string Version,
    IEnumerable<string> SupportedLanguages,
    IEnumerable<string> Rules
);