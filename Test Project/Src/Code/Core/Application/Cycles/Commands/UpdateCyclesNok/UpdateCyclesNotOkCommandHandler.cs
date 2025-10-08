// <copyright file="UpdateCyclesNotOkCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Commands.UpdateCyclesNok;

using IndQuestResults.Operations;

/// <summary>
/// Represents the UpdateCyclesNotOkCommandHandler.
/// </summary>
public class UpdateCyclesNotOkCommandHandler(
    IRepository<Cycle> cycleRepository,
    IRepository<BarCode> barCodeRepository,
    IRepository<Register> registerRepository,
    IRepository<TaskGatewayRequest> repositoryCommand,
    IShiftService shiftService,
    IDateTimeMachine dateTimeMachine,
    IBarCodeResult barCodeResult) : IGatewayRequestHandler<UpdateCyclesNotOkCommand, TaskGatewayResponse>, IResettable
{
    /// <inheritdoc/>
    public async Task<Result<TaskGatewayResponse>> ProcessAsync(UpdateCyclesNotOkCommand cmd, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TaskGatewayResponse>.WithFailure("Operation was canceled.");
        }

        var result = await Result.Success(cmd)
            .ValidateNotNull(c => (c, nameof(cmd)))
            .ThenAsync(c => GetBarCodeInformation(c, cancellationToken).ToResult("BarCode not found."))
            .Ensure(info => !StationCannotUpdateCycles(info), "Just process station can invoke Update cycles.")
            .Ensure(info => !StationCannotUpdateCyclesCreatedOnAnotherStation(cmd, info), "A station cannot update a cycle created in another station.")
            .Ensure(info => !StationCannotUpdateCyclesNotCreatedOnThisStation(cmd, info), "A station cannot update a cycle not created in this station.")
            .Ensure(info => !StationHaveCycleUpdatedOnThisStation(cmd, info), "A station cannot update a cycle Updated Before on the same station.")
            .ThenTap(info =>
            {
                UpdateCycleInformation(cmd, info);
                UpdateBarCodeInformation(cmd, info);
                CleanRegisterValuesFromNonVisibleCharacters(cmd.Command.Registers, info.CycleId);
                UpdateMachineIdOnRegisters(cmd.Command.Registers, info.MachineId);
            })
            .ThenAsync(info => SaveRegisters(cmd.Command.Registers.Values, cancellationToken).Then(_ => Task.FromResult(Result.Success(info))))
            .ThenAsync(info => UpdateCycleAndBarcode(info, cancellationToken))
            .ThenTap(info => SaveCommandAsync(cancellationToken, info, "UpdateCycleNotOk successful"))
            .ThenMap(info => TaskGatewayResponse.ToDto(info).ApplyReferencesValuesResult());

        await result.TapError(async errors =>
        {
            var barCodeInfo = await GetBarCodeInformation(cmd, cancellationToken);
            if (barCodeInfo != null)
            {
                await SaveCommandAsync(cancellationToken, barCodeInfo, string.Join("; ", errors));
            }
        });

        return result;
    }

    private bool StationHaveCycleUpdatedOnThisStation(UpdateCyclesNotOkCommand cmd, IBarCodeResult barCodeInfo)
    {
        return cmd.Command.MachineId == barCodeInfo.Cycle.MachineId && barCodeInfo.Cycle.CycleStatus != CycleStatus.Started;
    }

    private bool StationCannotUpdateCyclesNotCreatedOnThisStation(UpdateCyclesNotOkCommand cmd, IBarCodeResult barCodeInfo)
    {
        return cmd.Command.MachineId != barCodeInfo.Cycle.MachineId;
    }

    private void UpdateMachineIdOnRegisters(IDictionary<string, Register> commandRegisters, int machineId)
    {
        foreach (var variable in commandRegisters.Values)
        {
            variable.MachineId = machineId;
        }
    }

    private async Task SaveCommandAsync(CancellationToken cancellationToken, IBarCodeResult barCodeInfo, string? comment = default)
    {
        var command = new TaskGatewayRequest
        {
            MachineId = barCodeInfo.MachineId,
            BarCodeId = barCodeInfo.BarCodeId,
            CycleId = barCodeInfo.CycleId,
            CycleStatus = barCodeInfo.CycleStatus,
            PartStatus = barCodeInfo.PartStatus,
            FlowStatus = barCodeInfo.FlowStatus,
            ResultValidation = barCodeInfo.ResultValidation,
            Comment = comment ?? barCodeInfo.ResultValidation.Name,
            GatewayTask = GatewayTask.UpdateCycleNotOkAsync,
            TimeStamp = dateTimeMachine.Now.ToLocalTime(),
        };

        await repositoryCommand.AddAsync(command, cancellationToken);
    }

    private async Task<IBarCodeResult?> GetBarCodeInformation(UpdateCyclesNotOkCommand cmd, CancellationToken cancellationToken)
    {
        var request = cmd.Command;
        var barCodeDetailsRequest = new BarCodeDetailsRequest(request.MachineId, request.BarCode, request.PartNumber);
        return await barCodeResult.GetBarCodeDetails(barCodeDetailsRequest, cancellationToken);
    }

    private void UpdateCycleInformation(UpdateCyclesNotOkCommand cmd, IBarCodeResult barCodeInfo)
    {
        var request = cmd.Command;

        var cycle = barCodeInfo.Cycle;
        cycle.PartStatus = request.PartStatus;
        cycle.MachineId = request.MachineId;
        cycle.FinishedOn = dateTimeMachine.Now.ToLocalTime();
        cycle.CycleTime = (int)(cycle.FinishedOn - cycle.StartedOn).TotalSeconds;
        cycle.CycleStatus = request.CycleStatus;
    }

    private void UpdateBarCodeInformation(UpdateCyclesNotOkCommand cmd, IBarCodeResult barCodeInfo)
    {
        var request = cmd.Command;

        var barCode = barCodeInfo.BarCode;
        barCode.PartStatus = request.PartStatus;
        barCode.FlowStatus = this.DetermineFlowStatus(cmd, barCodeInfo);
        barCode.MachineId = request.MachineId;
        barCode.ModifiedOn = dateTimeMachine.Now.ToLocalTime();

        barCodeInfo.UpdateBarCodeInformationOnCycle(barCode.FlowStatus, barCode.PartStatus, request.CycleStatus);
    }

    private Task<Result<int>> SaveRegisters(IEnumerable<Register> registers, CancellationToken cancellationToken)
    {
        return registerRepository.AddRangeBulkAsync(registers, cancellationToken);
    }

    private async Task<Result<TaskGatewayResponse>> UpdateCycleAndBarcode(IBarCodeResult barCodeInfo, CancellationToken cancellationToken)
    {
        var cycleResult = await cycleRepository.GetByIdAsync(barCodeInfo.CycleId, cancellationToken);

        if (cycleResult.IsFailure)
        {
            return Result<TaskGatewayResponse>.WithFailure($"Cycle not Found CycleId: {barCodeInfo.CycleId.ToString()}");
        }

        var cycle = cycleResult.Value;
        if (cycle is null)
        {
            return Result<TaskGatewayResponse>.WithFailure("Cycle entity is null");
        }

        var barcodeResult = await barCodeRepository.GetByIdAsync(barCodeInfo.BarCode.BarCodeId, cancellationToken);

        if (barcodeResult.IsFailure)
        {
            return Result<TaskGatewayResponse>.WithFailure($"BarCode not Found BarCodeId: {barCodeInfo.BarCodeId.ToString()}");
        }
        if (barcodeResult.Value is null)
        {
            return Result<TaskGatewayResponse>.WithFailure("BarCode entity is null");
        }
        var barcode = barcodeResult.Value;
        if (barcode is null)
        {
            return Result<TaskGatewayResponse>.WithFailure("BarCode entity is null");
        }

        var resultShift = await shiftService.CreateOrRetrieveShiftAndCyclesOkAsync(cycle.MachineId, cancellationToken);

        if (resultShift.IsFailure)
        {
            return Result<TaskGatewayResponse>.WithFailure($"Cannot create Shift {dateTimeMachine.Now}");
        }

        var shift = resultShift.Value;
        if (shift is null)
        {
            return Result<TaskGatewayResponse>.WithFailure("Shift entity is null");
        }

        cycle.PartStatus = barCodeInfo.Cycle.PartStatus;
        cycle.FinishedOn = barCodeInfo.Cycle.FinishedOn;
        cycle.CycleTime = barCodeInfo.Cycle.CycleTime;
        cycle.CycleStatus = barCodeInfo.Cycle.CycleStatus;
        cycle.CyclesOk = barCodeInfo.Cycle.CyclesOk;

        barcode.ModifiedOn = barCodeInfo.BarCode.ModifiedOn;
        barcode.MachineId = barCodeInfo.BarCode.MachineId;

        if (Equals(barCodeInfo.MachineType, MachineType.Final) && cycle.CycleStatus.Equals(CycleStatus.FinishedOk))
        {
            barcode.FlowStatus = FlowStatus.Finished;
        }
        else
        {
            barcode.FlowStatus = barCodeInfo.BarCode.FlowStatus;
        }

        cycle.CyclesOk = shift.CyclesOk;

        barCodeInfo.SetCyclesOk(shift.CyclesOk);
        barCodeInfo.SetCycle(cycle);
        barCodeInfo.SetBarCode(barcode);

        // Validate cycle time
        var isCycleTimeInvalid = IsCycleTimeInvalid(barCodeInfo.Cycle.CycleTime, barCodeInfo.Recipe);

        if (isCycleTimeInvalid)
        {
            cycle.CycleStatus = CycleStatus.FinishedNok;
            cycle.PartStatus = PartStatus.NOk;
            barcode.PartStatus = PartStatus.NOk;
        }

        await cycleRepository.UpdateAsync(cycle, cancellationToken);

        await barCodeRepository.UpdateAsync(barcode, cancellationToken);

        var dto = TaskGatewayResponse.ToDto(barCodeInfo);
        return isCycleTimeInvalid
            ? Result<TaskGatewayResponse>.WithFailure("Cycle time is invalid", dto)
            : Result<TaskGatewayResponse>.Success(dto);
    }

    private FlowStatus DetermineFlowStatus(UpdateCyclesNotOkCommand request, IBarCodeResult barCodeInfo)
    {
        // If the part status of the monitorRequest is finished successfully and the machine type is "Process",
        // then update the flow status of the bar code to "InProcess".
        return IsProcessFinishedOkOnFinalStation(request, barCodeInfo) ? FlowStatus.Finished : FlowStatus.InProcess;
    }

    public static bool IsProcessFinishedOkOnFinalStation(UpdateCyclesNotOkCommand request, IBarCodeResult barCodeInfo)
    {
        return Equals(request.Command.CycleStatus, CycleStatus.FinishedOk) && Equals(barCodeInfo.MachineType, MachineType.Final);
    }

    private static bool StationCannotUpdateCyclesCreatedOnAnotherStation(UpdateCyclesNotOkCommand request, IBarCodeResult barCodeInfo)
    {
        return barCodeInfo.NextMachineId != request.Command.MachineId;
    }

    private static bool IsCycleTimeInvalid(int cycleTime, Recipe? recipe)
    {
        if (recipe is null)
        {
            return true;
        }

        return cycleTime <= recipe.CycleTimeMinimum || cycleTime >= recipe.CycleTimeMaximum;
    }

    private static bool StationCannotUpdateCycles(IBarCodeResult barCodeInfo)
    {
        return !Equals(barCodeInfo.MachineType, MachineType.Printer)
               && !Equals(barCodeInfo.MachineType, MachineType.Initial)
               && !Equals(barCodeInfo.MachineType, MachineType.InitialPrinter)
               && !Equals(barCodeInfo.MachineType, MachineType.Process)
               && !Equals(barCodeInfo.MachineType, MachineType.Final);
    }

    /// <summary>
    /// Cleans the register values by trimming and removing invisible characters.
    /// </summary>
    /// <param name="registers">The dictionary of registers to clean.</param>
    private void CleanRegisterValuesFromNonVisibleCharacters(IDictionary<string, Register> registers, int cycleId)
    {
        foreach (var register in registers.Values)
        {
            // Replace any invisible characters and trim whitespace

            // Clean and trim the 'Name' property
            register.Name = register.Name?.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty) ?? string.Empty;

            // Clean and trim the 'Description' property
            register.Description = register.Description?.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty) ?? string.Empty;

            // Clean and trim the 'Value' property
            register.Value = register.Value?.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty) ?? string.Empty;

            // Set CycleId and RegisterId
            register.CycleId = cycleId;
            register.RegisterId = 0;
            register.TimeStamp = dateTimeMachine.Now.ToLocalTime();
        }
    }

    /// <inheritdoc/>
    public bool TryReset()
    {
        return true;
    }
}
