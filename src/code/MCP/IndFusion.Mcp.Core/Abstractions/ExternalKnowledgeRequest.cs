namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for external knowledge integration.
/// </summary>
/// <param name="ExternalService">External service to integrate with.</param>
/// <param name="KnowledgeOperation">Operation to perform on knowledge.</param>
/// <param name="KnowledgeData">Knowledge data to process.</param>
/// <param name="IntegrationSettings">Settings for integration.</param>
public record ExternalKnowledgeRequest(
    string ExternalService,
    string KnowledgeOperation,
    Dictionary<string, object> KnowledgeData,
    IntegrationSettings IntegrationSettings
);