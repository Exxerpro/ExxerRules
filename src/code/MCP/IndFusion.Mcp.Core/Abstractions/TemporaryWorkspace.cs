namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Represents a temporary workspace for validation.
/// </summary>
/// <param name="WorkspacePath">Path to the temporary workspace.</param>
/// <param name="OriginalSolutionPath">Path to the original solution.</param>
/// <param name="CreatedAt">When the workspace was created.</param>
/// <param name="ExpiresAt">When the workspace expires.</param>
public record TemporaryWorkspace(
    string WorkspacePath,
    string OriginalSolutionPath,
    DateTime CreatedAt,
    DateTime ExpiresAt
);