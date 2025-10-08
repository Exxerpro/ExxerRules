// <copyright file="CycleService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Services;

/// <summary>
/// Service implementation for Cycle-related operations, replacing CycleRepositoryExtensions.
/// Provides proper dependency injection and testability.
/// </summary>
public class CycleService : ICycleService
{
    private readonly IRepository<Cycle> cycleRepository;

    public CycleService(IRepository<Cycle> cycleRepository)
    {
        this.cycleRepository = cycleRepository ?? throw new ArgumentNullException(nameof(cycleRepository));
    }

    /// <inheritdoc/>
    public async Task<Result<List<CycleView>>> GetCyclesByBarCodeIdAsync(
        int barCodeId,
        CancellationToken cancellationToken)
    {
        var spec = new Specification<Cycle>(p => p.BarCodeId == barCodeId);
        var cyclesResult = await this.cycleRepository.ListAsync(spec, cancellationToken).ConfigureAwait(false);

        if (cyclesResult.IsFailure || cyclesResult.Value is null || !cyclesResult.Value.Any())
        {
            return Result<List<CycleView>>.WithFailure("No cycles found for the specified BarCode ID");
        }

        var result = CycleView.ToDtoList(cyclesResult.Value.ToList());
        if (result.IsFailure || result.Value is null)
        {
            return Result<List<CycleView>>.WithFailure(result.Errors);
        }

        return Result<List<CycleView>>.Success(result.Value.ToList());
    }

    /// <inheritdoc/>
    public async Task<Result<int>> GetProductionByShiftAsync(
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
        var result = await this.cycleRepository.CountAsync(spec, cancellationToken).ConfigureAwait(false);
        return result;
    }

    /// <inheritdoc/>
    public async Task<Result<Cycle?>> GetCycleByIdAsync(
        int barCodeId,
        CancellationToken cancellationToken)
    {
        var spec = new Specification<Cycle>(c => c.BarCodeId == barCodeId);
        spec.AddOrderByDescending(c => c.CycleId);

        var result = await this.cycleRepository.FirstOrDefaultAsync(spec, cancellationToken).ConfigureAwait(false);

        return result;
    }
}
