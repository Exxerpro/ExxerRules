// <copyright file="IManualStart.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Gateway.Interfaces;

/// <summary>
/// Provides a contract for manually starting a process or service asynchronously.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate manual start interface logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IManualStart
{
    /// <summary>
    /// Starts the process or service asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StartAsync(CancellationToken cancellationToken);
}
