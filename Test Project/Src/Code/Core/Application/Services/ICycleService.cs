// <copyright file="ICycleService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Services;

/// <summary>
/// Service interface for Cycle-related operations, replacing CycleRepositoryExtensions.
/// Enables proper dependency injection and testability.
/// </summary>
public interface ICycleService
{
    /// <summary>
    /// Gets a list of cycle views by BarCode ID.
    /// </summary>
    /// <param name="barCodeId">The BarCode ID to search for.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of cycle views, or a failure result if not found.</returns>
    Task<Result<List<CycleView>>> GetCyclesByBarCodeIdAsync(
        int barCodeId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the production count by shift for a machine within a time range.
    /// </summary>
    /// <param name="startBy">The start time of the shift.</param>
    /// <param name="endTime">The end time of the shift.</param>
    /// <param name="machineId">The machine ID to filter by.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The production count, or a failure result if not found.</returns>
    Task<Result<int>> GetProductionByShiftAsync(
        DateTime startBy,
        DateTime endTime,
        int machineId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a cycle entity by its BarCode ID, ordered by descending cycle ID.
    /// </summary>
    /// <param name="barCodeId">The BarCode ID to search for.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The cycle entity if found, or a failure result.</returns>
    Task<Result<Cycle?>> GetCycleByIdAsync(
        int barCodeId,
        CancellationToken cancellationToken);
}
