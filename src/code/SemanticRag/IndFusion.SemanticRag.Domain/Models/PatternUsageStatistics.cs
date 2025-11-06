namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents usage statistics for a pattern.
/// </summary>
/// <param name="PatternId">ID of the pattern.</param>
/// <param name="UsageCount">Number of times the pattern is used.</param>
/// <param name="FileCount">Number of files using the pattern.</param>
/// <param name="ProjectCount">Number of projects using the pattern.</param>
/// <param name="LastUsed">When the pattern was last used.</param>
/// <param name="Trend">Usage trend over time.</param>
public readonly record struct PatternUsageStatistics(
    string PatternId,
    int UsageCount,
    int FileCount,
    int ProjectCount,
    DateTimeOffset? LastUsed,
    UsageTrend Trend)
{
    /// <summary>
    /// Gets the average usage per file.
    /// </summary>
    public double AverageUsagePerFile => FileCount > 0 ? (double)UsageCount / FileCount : 0.0;

    /// <summary>
    /// Gets the average usage per project.
    /// </summary>
    public double AverageUsagePerProject => ProjectCount > 0 ? (double)UsageCount / ProjectCount : 0.0;

    /// <summary>
    /// Validates the pattern usage statistics.
    /// </summary>
    /// <returns>A Result indicating whether the statistics are valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(PatternId))
            return Result.WithFailure("Pattern ID cannot be null or empty");

        if (UsageCount < 0)
            return Result.WithFailure("Usage count cannot be negative");

        if (FileCount < 0)
            return Result.WithFailure("File count cannot be negative");

        if (ProjectCount < 0)
            return Result.WithFailure("Project count cannot be negative");

        return Result.Success();
    }
}