namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Error handling strategy.
/// </summary>
/// <param name="Name">Name of the strategy.</param>
/// <param name="Description">Description of the strategy.</param>
/// <param name="ErrorActions">Actions to take on different error types.</param>
/// <param name="IsDefault">Whether this is the default strategy.</param>
public record ErrorHandlingStrategy(
    string Name,
    string Description,
    Dictionary<string, string> ErrorActions,
    bool IsDefault
);