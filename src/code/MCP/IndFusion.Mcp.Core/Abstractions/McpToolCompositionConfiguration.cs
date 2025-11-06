namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Configuration for MCP tool composition.
/// </summary>
/// <param name="AvailableTools">Available MCP tools.</param>
/// <param name="CompositionStrategies">Available composition strategies.</param>
/// <param name="ServiceMappings">Mappings between tools and services.</param>
/// <param name="PerformanceSettings">Performance settings for composition.</param>
/// <param name="Version">Version of the configuration.</param>
/// <param name="LastUpdated">When the configuration was last updated.</param>
public record McpToolCompositionConfiguration(
    IEnumerable<McpToolInfo> AvailableTools,
    IEnumerable<CompositionStrategy> CompositionStrategies,
    Dictionary<string, IEnumerable<string>> ServiceMappings,
    CompositionPerformanceSettings PerformanceSettings,
    string Version,
    DateTime LastUpdated
);