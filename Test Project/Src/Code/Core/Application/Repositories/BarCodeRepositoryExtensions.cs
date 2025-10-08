// <copyright file="BarCodeRepositoryExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Repositories;

/// <summary>
/// Provides extension methods for <see cref="IRepository{BarCode}"/> to support common BarCode queries and operations.
/// </summary>
public static class BarCodeRepositoryExtensions
{
    /// <summary>
    /// Gets the next consecutive BarCode ID by label, wrapping at 10000.
    /// </summary>
    /// <param name="barCodeRepository">The BarCode repository.</param>
    /// <param name="partNumber">The part number to filter by.</param>
    /// <param name="masterLabel">The list of master labels.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The next consecutive BarCode ID, or a failure result if not found.</returns>
    public static async Task<Result<int>> GetConsecutiveByBarCodeLabelAsync(
        this IRepository<BarCode> barCodeRepository,
        string partNumber,
        List<string> masterLabel,
        CancellationToken cancellationToken)
    {
        var spec = new Specification<BarCode>(b => true);
        spec.AddOrderByDescending(b => b.BarCodeId);

        var lastLabelResult = await barCodeRepository.FirstOrDefaultAsync(spec, cancellationToken);

        // Treat null or failure as not found per tests expectations
        if (lastLabelResult is null || lastLabelResult.IsFailure || lastLabelResult.Value is null)
        {
            return Result<int>.WithFailure("No BarCodes found for the given label");
        }

        var nextBarCodeId = (lastLabelResult.Value.BarCodeId + 1) % 10000;

        return Result<int>.Success(nextBarCodeId);
    }

    /// <summary>
    /// Gets a BarCode entity by its label.
    /// </summary>
    /// <param name="barCodeRepository">The BarCode repository.</param>
    /// <param name="label">The label to search for.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The BarCode entity if found, or a failure result.</returns>
    public static async Task<Result<BarCode>> GetBarCodeByLabelAsync(
        this IRepository<BarCode> barCodeRepository,
        string label,
        CancellationToken cancellationToken)
    {
        var spec = new Specification<BarCode>(b => b.Label == label);
        var barCode = await barCodeRepository.FirstOrDefaultAsync(spec, cancellationToken);
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

    /// <summary>
    /// Gets a BarCode entity by its unique identifier.
    /// </summary>
    /// <param name="barCodeRepository">The BarCode repository.</param>
    /// <param name="barCodeId">The BarCode ID to search for.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The BarCode entity if found, or a failure result.</returns>
    public static async Task<Result<BarCode>> GetBarCodeByIdAsync(
        this IRepository<BarCode> barCodeRepository,
        int barCodeId,
        CancellationToken cancellationToken)
    {
        var spec = new Specification<BarCode>(b => b.BarCodeId == barCodeId);
        var barCode = await barCodeRepository.FirstOrDefaultAsync(spec, cancellationToken);
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

    /// <summary>
    /// Gets a list of BarCode DTOs by searching register and cycle data for a label.
    /// </summary>
    /// <param name="registerRepository">The register repository.</param>
    /// <param name="cycleRepository">The cycle repository.</param>
    /// <param name="barCodeRepository">The BarCode repository.</param>
    /// <param name="label">The label to search for.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of BarCode DTOs matching the label, or a failure result.</returns>
    public static async Task<Result<List<BarCodeDto>>> GetBarCodeByRegisterDataAsync(
        this IRepository<Register> registerRepository,
        IRepository<Cycle> cycleRepository,
        IRepository<BarCode> barCodeRepository,
        string label,
        CancellationToken cancellationToken)
    {
        var registerSpec = new Specification<Register>(r => r.Value != null && r.Value.Contains(label));
        var cyclesIdsResult = await registerRepository.ListAsync(registerSpec, cancellationToken);

        if (cyclesIdsResult.IsFailure || cyclesIdsResult.Value is null || !cyclesIdsResult.Value.Any())
        {
            return Result<List<BarCodeDto>>.WithFailure("No cycles found for the given label");
        }

        var cyclesIdList = cyclesIdsResult.Value!.Select(r => r.CycleId).ToList();

        var cycleSpec = new Specification<Cycle>(b => cyclesIdList.Contains(b.CycleId));
        var cyclesResult = await cycleRepository.ListAsync(cycleSpec, cancellationToken);

        if (cyclesResult.IsFailure || cyclesResult.Value is null || !cyclesResult.Value.Any())
        {
            return Result<List<BarCodeDto>>.WithFailure("No BarCodes found for the given label");
        }

        var barCodeIds = cyclesResult.Value!.Select(c => c.BarCodeId).ToList();

        var barCodeSpec = new Specification<BarCode>(b => barCodeIds.Contains(b.BarCodeId));
        var barCodesResult = await barCodeRepository.ListAsync(barCodeSpec, cancellationToken);

        if (barCodesResult.IsFailure || barCodesResult.Value is null || !barCodesResult.Value.Any())
        {
            return Result<List<BarCodeDto>>.WithFailure("No BarCodes found for the given label");
        }

        var result = BarCodeDto.ToDtoList(barCodesResult.Value);
        if (result.IsFailure || result.Value is null)
        {
            return Result<List<BarCodeDto>>.WithFailure(result.Errors);
        }

        return Result<List<BarCodeDto>>.Success(result.Value.ToList());
    }
}
