namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Configuration for Fixer001.
/// </summary>
/// <param name="SolutionPath">Path to the solution file.</param>
/// <param name="AvailableTransformations">Available transformations.</param>
/// <param name="DefaultSettings">Default settings for transformations.</param>
/// <param name="Version">Version of Fixer001.</param>
/// <param name="LastUpdated">When the configuration was last updated.</param>
public record Fixer001Configuration(
    string SolutionPath,
    IEnumerable<TransformationInfo> AvailableTransformations,
    Dictionary<string, object> DefaultSettings,
    string Version,
    DateTime LastUpdated
);