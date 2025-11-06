namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Composition strategy for services.
/// </summary>
/// <param name="Name">Name of the strategy.</param>
/// <param name="Description">Description of the strategy.</param>
/// <param name="ServiceOrder">Order of services in the composition.</param>
/// <param name="DependencyRules">Rules for service dependencies.</param>
/// <param name="IsDefault">Whether this is the default strategy.</param>
public record CompositionStrategy(
    string Name,
    string Description,
    IEnumerable<string> ServiceOrder,
    IEnumerable<DependencyRule> DependencyRules,
    bool IsDefault
);