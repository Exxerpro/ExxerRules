// <copyright file="GetBarCodesListQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetBarCodeList;

using IndQuestResults.Operations;

/// <summary>
/// Represents the GetBarCodesListQueryHandler.
/// </summary>
public class GetBarCodesListQueryHandler(
    IReadOnlyRepository<BarCode> barCodeRepository,
    IReadOnlyRepository<MasterLabel> masterLabelRepository,
    IReadOnlyRepository<Cycle> cycleRepository)
    : IMonitorRequestHandler<GetBarCodesListQuery, BarCodesListVm>
{
    /// <inheritdoc/>
    public async Task<Result<BarCodesListVm>> ProcessAsync(GetBarCodesListQuery request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<BarCodesListVm>.WithFailure("Operation was canceled.");
        }

        return await Result.Success(request)
            .ValidateNotNull(req => (req, nameof(request)))
            .ThenAsync(req => req.IsMaster
                ? GetMasterBarcodesAsync(cancellationToken)
                : GetStandardBarcodesAsync(req, cancellationToken));
    }

    private async Task<Result<BarCodesListVm>> GetMasterBarcodesAsync(CancellationToken cancellationToken)
    {
        var masterLabelsResult = await masterLabelRepository.ListAsync(cancellationToken)
            .ThenMap(labels => labels.Select(e => e.MasterLabelCode).Where(c => c != null).ToList());

        if (masterLabelsResult.IsFailure) return Result<BarCodesListVm>.WithFailure(masterLabelsResult.Errors);

        var masterLabels = masterLabelsResult.Value!;

        return await barCodeRepository.ListAsync(cancellationToken)
            .ThenMap(barcodes => barcodes.Where(e => e.Label != null && masterLabels.Contains(e.Label!)).ToList())
            .Then(BarCodeDto.ToDtoList)
            .ThenAsync(dtos => AddCycleCountsAsync(dtos, cancellationToken));
    }

    private async Task<Result<BarCodesListVm>> GetStandardBarcodesAsync(GetBarCodesListQuery request, CancellationToken cancellationToken)
    {
        return await barCodeRepository.ListAsync(cancellationToken)
            .ThenMap(barcodes => barcodes.Where(e => e.ModifiedOn >= request.StartDate && e.ModifiedOn <= request.EndDate).ToList())
            .Then(BarCodeDto.ToDtoList)
            .ThenAsync(dtos => AddCycleCountsAsync(dtos, cancellationToken));
    }

    private async Task<Result<BarCodesListVm>> AddCycleCountsAsync(IEnumerable<BarCodeDto> barCodes, CancellationToken cancellationToken)
    {
        var dtos = barCodes.ToList();
        var barCodeIds = dtos.Select(e => e.BarCodeId).ToList();

        return await cycleRepository.ListAsync(cancellationToken)
            .ThenMap(cycles =>
            {
                var cyclesDictionary = cycles
                    .Where(e => barCodeIds.Contains(e.BarCodeId))
                    .GroupBy(e => e.BarCodeId)
                    .ToDictionary(e => e.Key, e => e.Count());

                foreach (var barCode in dtos)
                {
                    barCode.CycleCount = cyclesDictionary.TryGetValue(barCode.BarCodeId, out var cycleCount) ? cycleCount : 0;
                }

                return new BarCodesListVm
                {
                    BarCodes = dtos,
                    Count = dtos.Count,
                };
            });
    }
}
