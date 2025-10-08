using IndTrace.Domain.Entities;

namespace IndTrace.DataStore.Services.OEE.Interfaces;

/// <summary>
/// Defines a contract for a data sink that stores KPI (Key Performance Indicator) data.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IKpiDataSink logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IKpiDataSink
{
    /// <summary>
    /// Writes performance data to the data sink asynchronously.
    /// </summary>
    /// <param name="data">The performance data to write.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteAsync(PerformanceData data, CancellationToken cancellationToken);

    /// <summary>
    /// Writes OEE KPI data to the data sink asynchronously.
    /// </summary>
    /// <param name="data">The OEE KPI data to write.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteAsync(KpiOee data, CancellationToken cancellationToken);
}
