// <copyright file="UpdateBarcodeCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Commands.Update;

using IndQuestResults.Operations;

/// <summary>
/// Handles the update of existing barcodes within the IndTrace manufacturing system.
/// </summary>
/// <remarks>
/// This handler processes barcode updates which typically occur when a part moves through
/// different stations in the manufacturing workflow. It manages cycle completion,
/// status updates, and flow progression according to manufacturing business rules.
/// </remarks>
public class UpdateBarCodeCommandHandler(
    IDateTimeMachine dateTimeMachine,
    IRepository<Cycle> repositoryCycle,
    IBarCodeResult barCodeResult) : IGatewayRequestHandler<UpdateBarCodeCommand, TaskGatewayResponse>
{
    /// <summary>
    /// Processes the update barcode command asynchronously.
    /// </summary>
    /// <param name="cmd">The update barcode command containing the request details.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="Result{T}"/> with a <see cref="TaskGatewayResponse"/> if successful,
    /// or error information if the operation fails.
    /// </returns>
    /// <remarks>
    /// This method performs the following operations:
    /// 1. Validates the barcode and retrieves its current details
    /// 2. Creates a new cycle to track the update operation
    /// 3. Updates the barcode status to reflect the new state
    /// 4. Returns a comprehensive response with updated information.
    /// </remarks>
    public async Task<Result<TaskGatewayResponse>> ProcessAsync(UpdateBarCodeCommand cmd, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TaskGatewayResponse>.WithFailure("Operation was canceled.");
        }

        return await Result.Success(cmd)
            .ValidateNotNull(c => (c, nameof(cmd)))
            .ThenAsync(c => GetBarcodeInfo(c.Command, cancellationToken))
            .Ensure(info => info.ResultValidation == ResultValidation.Valid, info => info.Error ?? "Barcode validation failed.")
            .ThenAsync(info => CreateNewCycle(info, cancellationToken))
            .ThenTap(info => UpdateBarcodeState(cmd.Command, info))
            .ThenMap(info => TaskGatewayResponse.ToDto(info).ApplyReferencesValuesResult());
    }

    private async Task<Result<IBarCodeResult>> GetBarcodeInfo(TaskGatewayRequest request, CancellationToken cancellationToken)
    {
        var detailsRequest = new BarCodeDetailsRequest(request.MachineId, request.BarCode, request.PartNumber);
        var barCodeInfo = await barCodeResult.GetBarCodeDetails(detailsRequest, cancellationToken).ConfigureAwait(false);
        return barCodeInfo is not null ? Result.Success(barCodeInfo) : Result<IBarCodeResult>.WithFailure("Failed to retrieve barcode details.");
    }

    private async Task<Result<IBarCodeResult>> CreateNewCycle(IBarCodeResult info, CancellationToken cancellationToken)
    {
        var entity = new Cycle
        {
            MachineId = info.BarCode.MachineId,
            BarCodeId = info.BarCodeId,
            StartedOn = dateTimeMachine.Now.ToLocalTime(),
            FinishedOn = dateTimeMachine.Now.ToLocalTime(),
            CycleTime = 0,
            PartStatus = PartStatus.Ok,
            CycleStatus = CycleStatus.FinishedOk,
            CyclesOk = 0,
        };

        return await repositoryCycle.AddAsync(entity, cancellationToken)
            .ThenMap(cycleId =>
            {
                info.SetCycle(entity with { CycleId = cycleId });
                return info;
            });
    }

    private void UpdateBarcodeState(TaskGatewayRequest request, IBarCodeResult info)
    {
        info.BarCode.PartStatus = PartStatus.Ok;
        info.BarCode.FlowStatus = FlowStatus.Finished;
        info.BarCode.ModifiedOn = dateTimeMachine.Now.ToLocalTime();
        info.BarCode.MachineId = request.MachineId;
        // TODO UPDATE BARCODES
        // TODO ADD PERSISTED COMMAND
    }

    /// <summary>
    /// Attempts to reset the command handler to its initial state.
    /// </summary>
    /// <returns>Always returns <c>true</c> indicating successful reset.</returns>
    /// <remarks>
    /// This implementation always succeeds as the handler is stateless and doesn't require cleanup.
    /// </remarks>
    public bool TryReset()
    {
        return true;
    }
}
