namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// A data flow step between services.
/// </summary>
/// <param name="SourceService">Source service name.</param>
/// <param name="TargetService">Target service name.</param>
/// <param name="DataMapping">Mapping of data between services.</param>
/// <param name="Transformation">Transformation to apply to data.</param>
public record DataFlowStep(
    string SourceService,
    string TargetService,
    Dictionary<string, string> DataMapping,
    string? Transformation = null
);