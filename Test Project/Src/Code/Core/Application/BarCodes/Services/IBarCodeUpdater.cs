// <copyright file="IBarCodeUpdater.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Services;

/// <summary>
/// Updates BarCode entities with new status and timestamps.
/// Based on CreateCyclesCommandHandler BarCode update logic.
/// </summary>
public interface IBarCodeUpdater
{
    /// <summary>
    /// Updates BarCode with new flow/part status and modified timestamp.
    /// </summary>
    /// <param name="request">Request containing update parameters.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result indicating success or failure with reasons.</returns>
    Task<Result> UpdateAsync(
        BarCodeUpdateRequest request,
        CancellationToken cancellationToken);
}

/// <summary>
/// Request for updating BarCode status.
/// </summary>
/// <param name="BarCodeId">The barcode ID to update.</param>
/// <param name="PartStatus">The new part status value.</param>
/// <param name="FlowStatus">The new flow status value.</param>
/// <param name="MachineId">The machine ID to associate with the barcode.</param>
/// <param name="ModifiedOn">The modification timestamp.</param>
public sealed record BarCodeUpdateRequest(
    int BarCodeId,
    int PartStatus,
    int FlowStatus,
    int MachineId,
    DateTimeOffset ModifiedOn);
