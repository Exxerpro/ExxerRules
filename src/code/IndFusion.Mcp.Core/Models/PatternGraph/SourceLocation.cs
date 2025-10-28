namespace IndFusion.Mcp.Core.Models.PatternGraph;

/// <summary>
/// Represents a source location in code with file path and line/column information.
/// </summary>
/// <param name="FilePath">Path to the source file.</param>
/// <param name="StartLine">Starting line number (1-based).</param>
/// <param name="EndLine">Ending line number (1-based).</param>
/// <param name="StartColumn">Starting column number (1-based).</param>
/// <param name="EndColumn">Ending column number (1-based).</param>
public record SourceLocation(
	string FilePath,
	int StartLine,
	int EndLine,
	int StartColumn,
	int EndColumn);
