namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for external linting integration.
/// </summary>
/// <param name="ExternalService">External service to integrate with.</param>
/// <param name="CodeToAnalyze">Code to analyze.</param>
/// <param name="AnalysisOptions">Options for analysis.</param>
/// <param name="IntegrationSettings">Settings for integration.</param>
public record ExternalLintingRequest(
    string ExternalService,
    string CodeToAnalyze,
    Dictionary<string, object> AnalysisOptions,
    IntegrationSettings IntegrationSettings
);