namespace IndTrace.Aggregation.BoundedTests.Services;

/// <summary>
/// Represents current usage statistics.
/// </summary>
public class TestDataUsageStats
{
    /// <summary>
    /// Gets or sets the TotalRegistersAccessed.
    /// </summary>
    public int TotalRegistersAccessed { get; set; }
    /// <summary>
    /// Gets or sets the TotalBarCodesAccessed.
    /// </summary>
    public int TotalBarCodesAccessed { get; set; }
    /// <summary>
    /// Gets or sets the TotalCyclesAccessed.
    /// </summary>
    public int TotalCyclesAccessed { get; set; }
    /// <summary>
    /// Gets or sets the TotalMachinesAccessed.
    /// </summary>
    public int TotalMachinesAccessed { get; set; }
    /// <summary>
    /// Gets or sets the TotalContexts.
    /// </summary>
    public int TotalContexts { get; set; }
}
