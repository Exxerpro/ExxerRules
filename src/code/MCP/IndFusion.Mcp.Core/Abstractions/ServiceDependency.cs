namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Dependency between services in a composition.
/// </summary>
/// <param name="SourceService">Source service name.</param>
/// <param name="TargetService">Target service name.</param>
/// <param name="DependencyType">Type of dependency.</param>
/// <param name="DataFlow">Data flow between services.</param>
public record ServiceDependency(
    string SourceService,
    string TargetService,
    string DependencyType,
    string DataFlow
);