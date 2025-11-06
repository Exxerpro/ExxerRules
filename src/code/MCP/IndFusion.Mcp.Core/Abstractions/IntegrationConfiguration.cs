namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Configuration for external service integration.
/// </summary>
/// <param name="ExternalServices">Available external services.</param>
/// <param name="IntegrationTemplates">Templates for integration.</param>
/// <param name="AuthenticationProviders">Available authentication providers.</param>
/// <param name="RateLimitingSettings">Rate limiting settings.</param>
/// <param name="Version">Version of the configuration.</param>
/// <param name="LastUpdated">When the configuration was last updated.</param>
public record IntegrationConfiguration(
    IEnumerable<ExternalServiceInfo> ExternalServices,
    IEnumerable<IntegrationTemplate> IntegrationTemplates,
    IEnumerable<AuthenticationProvider> AuthenticationProviders,
    RateLimitingSettings RateLimitingSettings,
    string Version,
    DateTime LastUpdated
);