namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for applying Fixer001 transformations.
/// </summary>
/// <param name="DiagnosticId">ID of the diagnostic to fix.</param>
/// <param name="TargetFiles">Files to apply the fix to.</param>
/// <param name="ValidationOptions">Options for validating the transformation.</param>
/// <param name="DryRun">Whether to perform a dry run without applying changes.</param>
/// <param name="BackupOriginal">Whether to backup original files.</param>
/// <param name="MaxFixesPerFile">Maximum number of fixes to apply per file.</param>
public record Fixer001Request(
    string DiagnosticId,
    IEnumerable<string> TargetFiles,
    TransformationValidationOptions ValidationOptions,
    bool DryRun = false,
    bool BackupOriginal = true,
    int MaxFixesPerFile = 10
);