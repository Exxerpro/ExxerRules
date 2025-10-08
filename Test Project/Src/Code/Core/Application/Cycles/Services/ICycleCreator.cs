// <copyright file="ICycleCreator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Services;

/// <summary>
/// Creates and persists new cycles with proper timestamps.
/// Based on CreateCyclesCommandHandler cycle creation logic.
/// </summary>
public interface ICycleCreator
{
    /// <summary>
    /// Creates and persists a new cycle with deterministic timestamps.
    /// </summary>
    /// <param name="request">Request containing cycle creation parameters.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing the created cycle or failure reasons.</returns>
    Task<Result<Cycle>> CreateAsync(
        CycleCreateRequest request,
        CancellationToken cancellationToken);
}

/// <summary>
/// Request for creating a new cycle.
/// </summary>
/// <param name="MachineId">The machine ID where the cycle is executed.</param>
/// <param name="BarCodeId">The barcode ID associated with the cycle.</param>
/// <param name="CycleStatus">The initial cycle status.</param>
/// <param name="PartStatus">The initial part status.</param>
/// <param name="StartedOn">When the cycle started.</param>
/// <param name="FinishedOn">When the cycle finished.</param>
public sealed record CycleCreateRequest(
    int MachineId,
    int BarCodeId,
    CycleStatus CycleStatus,
    PartStatus PartStatus,
    DateTimeOffset StartedOn,
    DateTimeOffset FinishedOn);
