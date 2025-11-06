namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Information about an external service.
/// </summary>
/// <param name="Name">Name of the service.</param>
/// <param name="Description">Description of the service.</param>
/// <param name="Endpoint">Endpoint URL for the service.</param>
/// <param name="AuthenticationRequired">Whether authentication is required.</param>
/// <param name="IsEnabled">Whether the service is enabled.</param>
public record ExternalServiceInfo(
    string Name,
    string Description,
    string Endpoint,
    bool AuthenticationRequired,
    bool IsEnabled
);