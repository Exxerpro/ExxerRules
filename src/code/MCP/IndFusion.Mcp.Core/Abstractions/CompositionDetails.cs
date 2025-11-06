namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Details about the composition process.
/// </summary>
/// <param name="ServicesUsed">Services used in the composition.</param>
/// <param name="CompositionStrategy">Strategy used for composition.</param>
/// <param name="Dependencies">Dependencies between services.</param>
/// <param name="PerformanceMetrics">Performance metrics for the composition.</param>
public record CompositionDetails(
    IEnumerable<string> ServicesUsed,
    string CompositionStrategy,
    IEnumerable<ServiceDependency> Dependencies,
    CompositionPerformanceMetrics PerformanceMetrics
);