namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Source location information.
/// </summary>
/// <param name="FilePath">Path to the source file.</param>
/// <param name="Line">Line number (1-based).</param>
/// <param name="Column">Column number (1-based).</param>
/// <param name="Length">Length of the code snippet.</param>
public record SourceLocation(
    string FilePath,
    int Line,
    int Column,
    int Length
);