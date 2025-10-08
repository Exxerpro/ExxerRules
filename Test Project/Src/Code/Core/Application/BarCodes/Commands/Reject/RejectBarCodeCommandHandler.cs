// <copyright file="RejectBarCodeCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Commands.Reject;

using IndQuestResults.Operations;

/// <summary>
/// Handles the rejection of barcodes by updating their flow status and logging the rejection operation.
/// </summary>
/// <remarks>
/// This handler orchestrates the barcode rejection process including:
/// - Validating the barcode exists
/// - Updating the barcode flow status to rejected
/// - Finding the associated cycle for logging
/// - Creating a gateway request for the rejection operation
/// - Returning a rejection view with the updated barcode information.
/// </remarks>
public class RejectBarCodeCommandHandler(
    IRepository<BarCode> barCodeRepository,
    IRepository<TaskGatewayRequest> repositoryCommand,
    IRepository<Cycle> repositoryCycles,
    IDateTimeMachine dateTimeMachine)
    : IMonitorRequestHandler<RejectBarCodeCommand, BarCodeRejectedView>
{
    /// <summary>
    /// Processes the barcode rejection command asynchronously.
    /// </summary>
    /// <param name="request">The reject barcode command containing the barcode label to reject.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="Result{T}"/> with a <see cref="BarCodeRejectedView"/> if successful,
    /// or error information if the operation fails.
    /// </returns>
    /// <remarks>
    /// This method performs the following steps:
    /// 1. Locates the barcode by its label
    /// 2. Updates the barcode flow status to rejected
    /// 3. Updates the modification timestamp
    /// 4. Finds the most recent cycle associated with the barcode
    /// 5. Creates a gateway request to log the rejection operation
    /// 6. Returns a rejection view with the updated barcode data.
    /// </remarks>
    public async Task<Result<BarCodeRejectedView>> ProcessAsync(RejectBarCodeCommand request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<BarCodeRejectedView>.WithFailure("Operation was canceled.");
        }

        return await Result.Success(request)
            .ValidateNotNull(req => (req, nameof(req)))
            .ThenAsync(req => GetBarcodeAsync(req.Label, cancellationToken))
            .ThenTap(barcode => UpdateBarcodeStatus(barcode))
            .ThenAsync(barcode => UpdateAndLogCycle(barcode, cancellationToken))
            .ThenMap(barcode => BarCodeRejectedView.ToDto(barcode));
    }

    private async Task<Result<BarCode>> GetBarcodeAsync(string label, CancellationToken cancellationToken)
    {
        var spec = new Specification<BarCode>(b => b.Label == label);
        return await barCodeRepository.FirstOrDefaultAsync(spec, cancellationToken)
            .ToResult($"BarCode not found {label}");
    }

    private async Task UpdateBarcodeStatus(BarCode barcode)
    {
        barcode.FlowStatus = FlowStatus.Rejected;
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
            GatewayTask = GatewayTask.RejectPartAsync,
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
