namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Summary statistics for linting results.
/// </summary>
/// <param name="TotalViolations">Total number of violations found.</param>
/// <param name="ErrorCount">Number of error-level violations.</param>
/// <param name="WarningCount">Number of warning-level violations.</param>
/// <param name="InfoCount">Number of info-level violations.</param>
/// <param name="HintCount">Number of hint-level violations.</param>
/// <param name="FilesAnalyzed">Number of files analyzed.</param>
/// <param name="RulesChecked">Number of rules that were checked.</param>
public record LintingSummary(
    int TotalViolations,
    int ErrorCount,
    int WarningCount,
    int InfoCount,
    int HintCount,
    int FilesAnalyzed,
    int RulesChecked
);