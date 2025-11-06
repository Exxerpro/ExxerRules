namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Information about a transformation.
/// </summary>
/// <param name="Id">Unique identifier for the transformation.</param>
/// <param name="Name">Name of the transformation.</param>
/// <param name="Description">Description of what the transformation does.</param>
/// <param name="SupportedLanguages">Programming languages supported.</param>
/// <param name="IsEnabled">Whether the transformation is enabled.</param>
/// <param name="Parameters">Parameters required for the transformation.</param>
public record TransformationInfo(
    string Id,
    string Name,
    string Description,
    IEnumerable<string> SupportedLanguages,
    bool IsEnabled,
    Dictionary<string, object> Parameters
);