namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of MCP tool composition operations.
/// </summary>
/// <param name="Success">Whether the composition succeeded.</param>
/// <param name="ToolName">Name of the composed tool.</param>
/// <param name="Result">Result from the composed tool.</param>
/// <param name="CompositionDetails">Details about the composition process.</param>
/// <param name="ExecutionTimeMs">Time taken for composition and execution.</param>
/// <param name="ErrorDetails">Error details if composition failed.</param>
public record McpToolResult(
    bool Success,
    string ToolName,
    object Result,
    CompositionDetails CompositionDetails,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);