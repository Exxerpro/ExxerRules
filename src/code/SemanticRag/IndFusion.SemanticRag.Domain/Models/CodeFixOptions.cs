namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Options for code fix operations.
/// </summary>
/// <param name="FixIds">List of fix IDs to apply.</param>
/// <param name="DryRun">Whether to perform a dry run without applying fixes.</param>
/// <param name="MaxFixes">Maximum number of fixes to apply.</param>
public record CodeFixOptions(
    IEnumerable<string> FixIds,
    bool DryRun = false,
    int MaxFixes = 10
);