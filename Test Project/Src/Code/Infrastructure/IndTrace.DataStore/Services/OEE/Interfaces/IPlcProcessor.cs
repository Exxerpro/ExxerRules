using IndTrace.Domain.Entities;
using IndTrace.Domain.Models;

namespace IndTrace.DataStore.Services.OEE.Interfaces;

/// <summary>
/// Defines a contract for processing PLC communications and data retrieval.
/// </summary>
public interface IPlcProcessor
{
    /// <summary>
    /// Gets or sets the PLC controller instance.
    /// </summary>
    IPlc Controller { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the PLC is currently connected.
    /// </summary>
    bool IsConnected { get; set; }

    /// <summary>
    /// Gets or sets the collection of registers for the PLC.
    /// </summary>
    IDictionary<string, Register> Registers { get; set; }

    /// <summary>
    /// Gets the unique identifier for the PLC.
    /// </summary>
    int PlcId { get; }

    /// <summary>
    /// Gets or sets the IP address of the PLC.
    /// </summary>
    string IpAddress { get; set; }

    /// <summary>
    /// Initializes the PLC processor asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Result> InitializeAsync();

    /// <summary>
    /// Reads performance data from the PLC asynchronously.
    /// </summary>
    /// <param name="PlcId">The ID of the PLC to read from.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation, containing the performance data result.</returns>
    Task<Result<PerformanceData>> ReadPerformanceDataFromPlcAsync(int PlcId, CancellationToken cancellationToken);
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IPlcProcessor logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
