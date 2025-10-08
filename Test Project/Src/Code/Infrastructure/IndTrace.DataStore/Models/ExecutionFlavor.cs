namespace IndTrace.DataStore.Models;

/// <summary>
/// Specifies the execution flavor for a simulation or test run.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate execution flavor logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public enum ExecutionFlavor
{
    /// <summary>
    /// Indicates that the execution completed with full success.
    /// </summary>
    FullSuccess,
    /// <summary>
    /// Indicates that the execution ended in a final failure.
    /// </summary>
    FinalFailure,
    /// <summary>
    /// Indicates that the cycle has started.
    /// </summary>
    CycleStarted,
}
