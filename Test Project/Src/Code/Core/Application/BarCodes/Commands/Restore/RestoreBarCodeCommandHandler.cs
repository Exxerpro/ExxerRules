// <copyright file="RestoreBarCodeCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Commands.Restore;

using IndQuestResults.Operations;

/// <summary>
/// Handles the restoration of bar codes by processing RestoreBarCodeCommand requests.
/// Updates bar code status from rejected back to an active state and creates associated gateway command.
/// </summary>
public class RestoreBarCodeCommandHandler(
    IRepository<BarCode> barCodeRepository,
    IRepository<TaskGatewayRequest> repositoryCommand,
    IRepository<Cycle> repositoryCycles,
    IDateTimeMachine dateTimeMachine)
    : IMonitorRequestHandler<RestoreBarCodeCommand, BarCodeRestoredView>
{
    /// <summary>
    /// Processes a RestoreBarCodeCommand request to restore a bar code from rejected status.
    /// Finds the bar code by label, updates its flow status, and creates a gateway command.
    /// </summary>
    /// <param name="request">The restore bar code command containing the bar code label.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>Result containing BarCodeRestoredView with restoration details or failure information.</returns>
    public async Task<Result<BarCodeRestoredView>> ProcessAsync(RestoreBarCodeCommand request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<BarCodeRestoredView>.WithFailure("Operation was canceled.");
        }

        return await Result.Success(request)
            .ValidateNotNull(req => (req, nameof(req)))
            .ThenAsync(req => GetBarcodeAsync(req.Label, cancellationToken))
            .ThenTap(barcode => UpdateBarcodeStatus(barcode))
            .ThenAsync(barcode => UpdateAndLogCycle(barcode, cancellationToken))
            .ThenMap(barcode => BarCodeRestoredView.ToDto(barcode));
    }

    private async Task<Result<BarCode>> GetBarcodeAsync(string label, CancellationToken cancellationToken)
    {
        var spec = new Specification<BarCode>(b => b.Label == label);
        return await barCodeRepository.FirstOrDefaultAsync(spec, cancellationToken)
            .ToResult($"BarCode not found {label}");
    }

    private async Task UpdateBarcodeStatus(BarCode barcode)
    {
        barcode.FlowStatus = FlowStatus.InProcess; // Restoring to active state
        barcode.ModifiedOn = dateTimeMachine.Now.ToLocalTime();
        await barCodeRepository.UpdateAsync(barcode);
    }

    private async Task<Result<BarCode>> UpdateAndLogCycle(BarCode barcode, CancellationToken cancellationToken)
    {
        var spec = new Specification<Cycle>(c => c.BarCodeId == barcode.BarCodeId)
            .AddOrderByDescending(b => b.CycleId)
            .ApplyPaging(0, 1);

        var cycleResult = await repositoryCycles.FirstOrDefaultAsync(spec, cancellationToken);

        var command = new TaskGatewayRequest
        {
            MachineId = barcode.MachineId,
            BarCodeId = barcode.BarCodeId,
            PartStatus = barcode.PartStatus,
            FlowStatus = barcode.FlowStatus,
            ResultValidation = ResultValidation.None,
            GatewayTask = GatewayTask.RestorePartAsync, // Changed to Restore
            TimeStamp = dateTimeMachine.Now.ToLocalTime(),
        };

        if (cycleResult.IsSuccess && cycleResult.Value != null)
        {
            command.CycleId = cycleResult.Value.CycleId;
            command.CycleStatus = cycleResult.Value.CycleStatus;
        }

        await repositoryCommand.AddAsync(command, cancellationToken);
        return Result.Success(barcode);
    }
}
