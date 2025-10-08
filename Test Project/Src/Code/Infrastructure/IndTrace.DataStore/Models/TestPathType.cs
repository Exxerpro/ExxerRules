namespace IndTrace.DataStore.Models;

/// <summary>
/// Specifies the type of test path for simulation or testing scenarios.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate test path type logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public enum TestPathType
{
    /// <summary>
    /// Indicates a test path that completes with full success.
    /// </summary>
    FullSuccess,
    /// <summary>
    /// Indicates a test path that ends in full failure.
    /// </summary>
    FullFailure,
    /// <summary>
    /// Indicates a test path that fails midway through execution.
    /// </summary>
    MidwayFailure,
    /// <summary>
    /// Indicates a test path that enters a retry loop.
    /// </summary>
    RetryLoop,
    /// <summary>
    /// Indicates a test path that results in an incomplete cycle.
    /// </summary>
    IncompleteCycle,
}
