namespace IndFusion.Tools.Cli.Services;

/// <summary>
/// Contains all analysis data
/// </summary>
public class AnalysisData
{
    /// <summary>
    /// Gets or sets the code metrics
    /// </summary>
    public List<FileMetrics> Metrics { get; set; } = [];

    /// <summary>
    /// Gets or sets the complexity issues
    /// </summary>
    public List<ComplexityIssue> ComplexityIssues { get; set; } = [];

    /// <summary>
    /// Gets or sets the refactoring opportunities
    /// </summary>
    public List<RefactoringOpportunity> Opportunities { get; set; } = [];
}