// <copyright file="BarCodeService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Services;

/// <summary>
/// Service implementation for BarCode-related operations, replacing BarCodeRepositoryExtensions.
/// Provides proper dependency injection and testability.
/// </summary>
public class BarCodeService : IBarCodeService
{
    private readonly IRepository<BarCode> barCodeRepository;
    private readonly IRepository<Register> registerRepository;
    private readonly IRepository<Cycle> cycleRepository;

    public BarCodeService(
        IRepository<BarCode> barCodeRepository,
        IRepository<Register> registerRepository,
        IRepository<Cycle> cycleRepository)
    {
        this.barCodeRepository = barCodeRepository ?? throw new ArgumentNullException(nameof(barCodeRepository));
        this.registerRepository = registerRepository ?? throw new ArgumentNullException(nameof(registerRepository));
        this.cycleRepository = cycleRepository ?? throw new ArgumentNullException(nameof(cycleRepository));
    }

    /// <inheritdoc/>
    public async Task<Result<int>> GetConsecutiveByBarCodeLabelAsync(
        string partNumber,
        List<string> masterLabel,
        CancellationToken cancellationToken)
    {
        var spec = new Specification<BarCode>(b => true);
        spec.AddOrderByDescending(b => b.BarCodeId);

        var lastLabelResult = await this.barCodeRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (lastLabelResult is null || lastLabelResult.IsFailure)
        {
            return Result<int>.WithFailure("No BarCodes found for the given label");
        }

        var nextBarCodeId = lastLabelResult.IsSuccess && lastLabelResult.Value is not null
            ? (lastLabelResult.Value.BarCodeId + 1) % 10000
            : 1;

        return Result<int>.Success(nextBarCodeId);
    }

    /// <inheritdoc/>
    public async Task<Result<BarCode>> GetBarCodeByLabelAsync(
        string label,
        CancellationToken cancellationToken)
    {
        var spec = new Specification<BarCode>(b => b.Label == label);
        var barCode = await this.barCodeRepository.FirstOrDefaultAsync(spec, cancellationToken);
        if (barCode.IsFailure)
        {
            return Result<BarCode>.WithFailure(barCode.Errors);
        }

        if (barCode.Value is null)
        {
            return Result<BarCode>.WithFailure("BarCode not found");
        }

        return Result<BarCode>.Success(barCode.Value);
    }

    /// <inheritdoc/>
    public async Task<Result<BarCode>> GetBarCodeByIdAsync(
        int barCodeId,
        CancellationToken cancellationToken)
    {
        var spec = new Specification<BarCode>(b => b.BarCodeId == barCodeId);
        var barCode = await this.barCodeRepository.FirstOrDefaultAsync(spec, cancellationToken);
        if (barCode.IsFailure)
        {
            return Result<BarCode>.WithFailure(barCode.Errors);
        }

        if (barCode.Value is null)
        {
            return Result<BarCode>.WithFailure("BarCode not found");
        }

        return Result<BarCode>.Success(barCode.Value);
    }

    /// <inheritdoc/>
    public async Task<Result<List<BarCodeDto>>> GetBarCodeByRegisterDataAsync(
        string label,
        CancellationToken cancellationToken)
    {
        var registerSpec = new Specification<Register>(r => r.Value != null && r.Value.Contains(label));
        var cyclesIdsResult = await this.registerRepository.ListAsync(registerSpec, cancellationToken);

        if (cyclesIdsResult.IsFailure || cyclesIdsResult.Value is null || !cyclesIdsResult.Value.Any())
        {
            return Result<List<BarCodeDto>>.WithFailure("No cycles found for the given label");
        }

        var cyclesIdList = cyclesIdsResult.Value!.Select(r => r.CycleId).ToList();

        var cycleSpec = new Specification<Cycle>(b => cyclesIdList.Contains(b.CycleId));
        var cyclesResult = await this.cycleRepository.ListAsync(cycleSpec, cancellationToken);

        if (cyclesResult.IsFailure || cyclesResult.Value is null || !cyclesResult.Value.Any())
        {
            return Result<List<BarCodeDto>>.WithFailure("No BarCodes found for the given label");
        }

        var barCodeIds = cyclesResult.Value!.Select(c => c.BarCodeId).ToList();

        var barCodeSpec = new Specification<BarCode>(b => barCodeIds.Contains(b.BarCodeId));
        var barCodesResult = await this.barCodeRepository.ListAsync(barCodeSpec, cancellationToken);

        if (barCodesResult.IsFailure || barCodesResult.Value is null || !barCodesResult.Value.Any())
        {
            return Result<List<BarCodeDto>>.WithFailure("No BarCodes found for the given label");
        }

        var result = BarCodeDto.ToDtoList(barCodesResult.Value.ToList());
        if (result.IsFailure || result.Value is null)
        {
            return Result<List<BarCodeDto>>.WithFailure(result.Errors);
        }

        return Result<List<BarCodeDto>>.Success(result.Value.ToList());
    }
}
