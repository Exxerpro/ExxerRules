namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for safe regex transformations.
/// </summary>
/// <param name="Pattern">Regex pattern to match.</param>
/// <param name="Replacement">Replacement text.</param>
/// <param name="TargetFiles">Files to apply the transformation to.</param>
/// <param name="ValidationOptions">Options for validating the transformation.</param>
/// <param name="DryRun">Whether to perform a dry run.</param>
/// <param name="CaseSensitive">Whether the pattern matching is case sensitive.</param>
/// <param name="Multiline">Whether to use multiline matching.</param>
public record SafeRegexRequest(
    string Pattern,
    string Replacement,
    IEnumerable<string> TargetFiles,
    TransformationValidationOptions ValidationOptions,
    bool DryRun = false,
    bool CaseSensitive = true,
    bool Multiline = false
);