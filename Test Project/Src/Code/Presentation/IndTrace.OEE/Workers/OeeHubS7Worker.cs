// <copyright file="OeeHubS7Worker.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Workers;

using IndTrace.DataStore.Services.OEE.Interfaces;
using IndTrace.OEE.Infrastructure.Services;

/// <summary>
/// Background worker that manages the ingestion of S7 PLC data via the OEE hub.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate OeeHubS7Worker logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the OeeHubS7Worker.
/// </summary>
public class OeeHubS7Worker(IPerformanceDataCollector processor) : BackgroundService
{
    /// <summary>
    /// Executes the background service, starting the ingestion process and handling end-of-cycle events.
    /// </summary>
    /// <param name="stoppingToken">Token to observe for cancellation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await processor.StartingAsync(stoppingToken);
        await processor.DispatchPerformanceSnapshotAsync(stoppingToken);
    }
}
