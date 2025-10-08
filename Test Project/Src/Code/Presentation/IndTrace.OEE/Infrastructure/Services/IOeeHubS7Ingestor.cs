// <copyright file="IOeeHubS7Ingestor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Infrastructure.Services;

using System;

/// <summary>
/// Defines the contract for ingesting S7 data from OEE Hub into the system.
/// </summary>
public interface IOeeHubS7Ingestor
{
    /// <summary>
    /// Starts the ingestion process.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token to stop the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StartingAsync(CancellationToken stoppingToken);

    /// <summary>
    /// Ingests end-of-cycle events asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task IngestEndOfCycleEventsAsync(CancellationToken cancellationToken);
}
