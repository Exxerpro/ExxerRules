namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Represents a transformed file.
/// </summary>
/// <param name="FilePath">Path to the file.</param>
/// <param name="OriginalContent">Original content of the file.</param>
/// <param name="TransformedContent">Transformed content of the file.</param>
/// <param name="TransformationType">Type of transformation applied.</param>
public record TransformedFile(
    string FilePath,
    string OriginalContent,
    string TransformedContent,
    string TransformationType
);