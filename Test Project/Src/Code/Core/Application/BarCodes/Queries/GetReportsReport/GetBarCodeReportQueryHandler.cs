// <copyright file="GetBarCodeReportQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetReportsReport;

/// <summary>
/// Represents the GetBarCodeReportQueryHandler.
/// </summary>
public class GetBarCodeReportQueryHandler(
    IRepository<BarCode> barcodeRepository,
    IRepository<Cycle> cyclesRepository,
    IRepository<Register> registersRepository,
    IRepository<Variable> variableRepository)
    : IMonitorQueryHandler<GetBarCodeReportQuery, List<BarCodeReportVm>>
{
    /// <inheritdoc/>
    public async Task<Result<List<BarCodeReportVm>>> ProcessAsync(GetBarCodeReportQuery request, CancellationToken cancellationToken)
    {
        var barCodesId = request.BarCodesIdList;
        var specBarCode = new Specification<BarCode>(b => barCodesId.Contains(b.BarCodeId));

        var resultBarCodes = await barcodeRepository.ListAsync(specBarCode, cancellationToken).ConfigureAwait(false);

        if (resultBarCodes.IsFailure)
        {
            return Result<List<BarCodeReportVm>>.WithFailure("BarCodes not found");
        }

        var specCycle = new Specification<Cycle>(b => barCodesId.Contains(b.BarCodeId));
        var resultCycles = await cyclesRepository.ListAsync(specCycle, cancellationToken).ConfigureAwait(false);

        if (resultCycles.IsFailure)
        {
            return Result<List<BarCodeReportVm>>.WithFailure("Cycles not found");
        }

        var cycleIds = resultCycles.Value?.Select(c => c.CycleId).ToList() ?? new List<int>();
        var specRegister = new Specification<Register>(b => cycleIds.Contains(b.CycleId));
        var resultRegisters = await registersRepository.ListAsync(specRegister, cancellationToken).ConfigureAwait(false);

        var resultRegister = await registersRepository.
            GetRegistersWithVariablesAsync(
                variableRepository,
                resultCycles.Value?.Select(s => s.CycleId).ToList() ?? new List<int>(),
                cancellationToken).ConfigureAwait(false);

        if (resultRegisters.Value is null)
        {
            return Result<List<BarCodeReportVm>>.WithFailure("Registers not found");
        }

        var result = BarCodeReportVm.ToDtoList(resultBarCodes.Value ?? new List<BarCode>());
        if (result.IsFailure || result.Value is null)
        {
            return Result<List<BarCodeReportVm>>.WithFailure(result.Errors);
        }

        foreach (var item in result.Value)
        {
            if (resultCycles.Value is null)
            {
                continue;
            }

            var cyclesView = CycleView.ToDtoList(resultCycles.Value.Where(c => c.BarCodeId == item.BarCodeId));
            if (cyclesView.IsFailure || cyclesView.Value is null)
            {
                continue;
            }

            if (resultRegisters.IsFailure)
            {
                continue;
            }

            item.Cycles = cyclesView.Value.ToList();

            var registerViews = RegisterView.ToDtoList(resultRegisters.Value
                .Where(r => item.Cycles.Select(c => c.CycleId).Contains(r.CycleId)));
            if (registerViews.IsFailure || registerViews.Value is null)
            {
                continue;
            }

            item.Registers = registerViews.Value.ToList();
        }

        return Result<List<BarCodeReportVm>>.Success(result.Value.ToList());
    }
}
