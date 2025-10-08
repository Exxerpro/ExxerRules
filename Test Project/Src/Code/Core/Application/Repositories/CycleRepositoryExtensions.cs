// <copyright file="CycleRepositoryExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Repositories;

using ZXing;

/// <summary>
/// Provides extension methods for <see cref="IRepository{Cycle}"/> to support common cycle queries and operations.
/// </summary>
public static class CycleRepositoryExtensions
{
    /// <summary>
    /// Gets a list of cycle views by BarCode ID.
    /// </summary>
    /// <param name="cycleRepository">The cycle repository.</param>
    /// <param name="barCodeId">The BarCode ID to search for.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of cycle views, or a failure result if not found.</returns>
    public static async Task<Result<List<CycleView>>> GetCyclesByBarCodeIdAsync(
        this IRepository<Cycle> cycleRepository,
        int barCodeId,
        CancellationToken cancellationToken)
    {
        var spec = new Specification<Cycle>(p => p.BarCodeId == barCodeId);
        var cyclesResult = await cycleRepository.ListAsync(spec, cancellationToken).ConfigureAwait(false);

        if (cyclesResult.IsFailure || cyclesResult.Value is null || !cyclesResult.Value.Any())
        {
            return Result<List<CycleView>>.WithFailure("No cycles found for the specified BarCode ID");
        }

        var result = CycleView.ToDtoList(cyclesResult.Value);
        if (result.IsFailure || result.Value is null)
        {
            return Result<List<CycleView>>.WithFailure(result.Errors);
        }

        return Result<List<CycleView>>.Success(result.Value.ToList());
    }

    /// <summary>
    /// Gets the production count by shift for a machine within a time range.
    /// </summary>
    /// <param name="cycleRepository">The cycle repository.</param>
    /// <param name="startBy">The start time of the shift.</param>
    /// <param name="endTime">The end time of the shift.</param>
    /// <param name="machineId">The machine ID to filter by.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The production count, or a failure result if not found.</returns>
    public static async Task<Result<int>> GetProductionByShiftAsync(
        this IRepository<Cycle> cycleRepository,
        DateTime startBy,
        DateTime endTime,
        int machineId,
        CancellationToken cancellationToken)
    {
        var spec = new Specification<Cycle>(c => c.StartedOn >= startBy &&
                                                     c.FinishedOn <= endTime &&
                                                     c.CycleStatus == CycleStatus.FinishedOk &&
                                                     c.PartStatus == PartStatus.Ok &&
                                                     c.MachineId == machineId);
        var result = await cycleRepository.CountAsync(spec, cancellationToken).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// Gets a cycle entity by its BarCode ID, ordered by descending cycle ID.
    /// </summary>
    /// <param name="cycleRepository">The cycle repository.</param>
    /// <param name="barCodeId">The BarCode ID to search for.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The cycle entity if found, or a failure result.</returns>
    public static async Task<Result<Cycle?>> GetCycleByIdAsync(
        this IRepository<Cycle> cycleRepository,
        int barCodeId,
        CancellationToken cancellationToken)
    {
        var spec = new Specification<Cycle>(c => c.BarCodeId == barCodeId);
        spec.AddOrderByDescending(c => c.CycleId);

        var result = await cycleRepository.FirstOrDefaultAsync(spec, cancellationToken).ConfigureAwait(false);

        return result;
    }
}
