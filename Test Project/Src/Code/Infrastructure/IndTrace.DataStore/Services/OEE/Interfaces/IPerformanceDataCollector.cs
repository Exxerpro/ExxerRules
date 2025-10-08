namespace IndTrace.DataStore.Services.OEE.Interfaces;
//// Alternate naming options considered:
// // - TelemetryDispatcher
// // - PlcTelemetryAgent
// // - CycleDataCollector
// // Retained for historical context and possible future specialization.

/// <summary>
/// Defines a contract for collecting and dispatching performance data from PLCs.
/// </summary>
public interface IPerformanceDataCollector
{
    /// <summary>
    /// Starts the performance data collection process asynchronously.
    /// </summary>
    /// <param name="stoppingToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StartingAsync(CancellationToken stoppingToken);

    /// <summary>
    /// Dispatches a snapshot of performance data asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DispatchPerformanceSnapshotAsync(CancellationToken cancellationToken);
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IPerformanceDataCollector logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
