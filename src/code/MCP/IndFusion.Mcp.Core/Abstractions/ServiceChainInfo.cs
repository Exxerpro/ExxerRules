namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Information about a service chain.
/// </summary>
/// <param name="Name">Name of the service chain.</param>
/// <param name="Description">Description of the service chain.</param>
/// <param name="Services">Services in the chain.</param>
/// <param name="IsEnabled">Whether the service chain is enabled.</param>
public record ServiceChainInfo(
    string Name,
    string Description,
    IEnumerable<ServiceChainStep> Services,
    bool IsEnabled
);