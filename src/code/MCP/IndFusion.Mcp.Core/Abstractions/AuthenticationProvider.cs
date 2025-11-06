namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Authentication provider for external services.
/// </summary>
/// <param name="Name">Name of the provider.</param>
/// <param name="ProviderType">Type of the provider.</param>
/// <param name="Configuration">Configuration for the provider.</param>
/// <param name="IsEnabled">Whether the provider is enabled.</param>
public record AuthenticationProvider(
    string Name,
    string ProviderType,
    Dictionary<string, object> Configuration,
    bool IsEnabled
);