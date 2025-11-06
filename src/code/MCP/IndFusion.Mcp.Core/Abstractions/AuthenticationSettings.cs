namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Authentication settings for external services.
/// </summary>
/// <param name="Provider">Authentication provider.</param>
/// <param name="Credentials">Authentication credentials.</param>
/// <param name="TokenExpiry">Token expiry time.</param>
/// <param name="RefreshToken">Refresh token if available.</param>
public record AuthenticationSettings(
    string Provider,
    Dictionary<string, object> Credentials,
    DateTime? TokenExpiry = null,
    string? RefreshToken = null
);