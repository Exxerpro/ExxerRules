// <copyright file="OeeQuestWorker.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Workers;

using IndTrace.DataStore.Services.OEE.Interfaces;
using IndTrace.OEE.Infrastructure.Services;

/// <summary>
/// Background worker that processes OEE register data using a reactive event service.
/// </summary>
public class OeeQuestWorker(IReactiveEventService processor) : BackgroundService
{
    /// <summary>
    /// Executes the background service, processing OEE register data reactively.
    /// </summary>
    /// <param name="stoppingToken">Token to observe for cancellation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate OeeQuestWorker logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
        return processor.ProcessOeeRegisterAsync(stoppingToken);
    }
}
