using IndTrace.DataStore.ModelsComs;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Models;

namespace IndTrace.DataStore.Services.OEE.Interfaces
{
    /// <summary>
    /// Manages S7 PLC connections and data retrieval for performance monitoring.
    /// Handles initialization of connections, fetching performance data, and lifecycle management of PLC processors.
    /// </summary>
    public interface IPlcManager
    {
        /// <summary>
        /// Initializes PLC connections and sets up performance tag monitoring.
        /// </summary>
        /// <param name="plcsData">Dictionary containing configuration data for each PLC.</param>
        /// <param name="performanceTags">Nested dictionary containing performance tags for each PLC.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task<Result> InitializeAsync(IReadOnlyDictionary<int, PlcData> plcsData, IReadOnlyDictionary<int, IReadOnlyDictionary<string, VariableS7>> performanceTags);

        /// <summary>
        /// Reads performance data from a specific PLC asynchronously.
        /// </summary>
        /// <param name="plcId">The ID of the PLC to read from.</param>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A task representing the asynchronous operation, containing the performance data result.</returns>
        public Task<Result<PerformanceData>> ReadPerformanceDataAsync(int plcId, CancellationToken cancellationToken);
    }
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IPlcManager logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
