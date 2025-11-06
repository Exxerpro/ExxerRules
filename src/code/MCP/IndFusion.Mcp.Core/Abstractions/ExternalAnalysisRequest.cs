namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for external analysis integration.
/// </summary>
/// <param name="ExternalService">External service to integrate with.</param>
/// <param name="AnalysisType">Type of analysis to perform.</param>
/// <param name="InputData">Input data for analysis.</param>
/// <param name="AnalysisParameters">Parameters for analysis.</param>
/// <param name="IntegrationSettings">Settings for integration.</param>
public record ExternalAnalysisRequest(
    string ExternalService,
    string AnalysisType,
    Dictionary<string, object> InputData,
    Dictionary<string, object> AnalysisParameters,
    IntegrationSettings IntegrationSettings
);