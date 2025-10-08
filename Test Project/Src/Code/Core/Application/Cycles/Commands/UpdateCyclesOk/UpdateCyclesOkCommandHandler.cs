// <copyright file="UpdateCyclesOkCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Commands.UpdateCyclesOk;

/// <summary>
/// Represents the UpdateCyclesOkCommandHandler.
/// </summary>
public class LegacyUpdateCyclesOkCommandHandler(
    IRepository<Cycle> cycleRepository,
    IRepository<BarCode> barCodeRepository,
    IRepository<Register> registerRepository,
    IRepository<TaskGatewayRequest> repositoryCommand,
    IShiftService shiftService,
    IDateTimeMachine dateTimeMachine,
    IBarCodeResult barCodeResult,
    ILogger<LegacyUpdateCyclesOkCommandHandler> logger) : IGatewayRequestHandler<UpdateCyclesOkCommand, TaskGatewayResponse>, IResettable
{
    /// <inheritdoc/>
    public async Task<Result<TaskGatewayResponse>> ProcessAsync(UpdateCyclesOkCommand cmd, CancellationToken cancellationToken)
    {
        if (cmd is null)
        {
            return Result<TaskGatewayResponse>.WithFailure("cmd cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TaskGatewayResponse>.WithFailure("Operation was canceled.");
        }

        logger.LogInformation("ProcessAsync: Starting with BarCode={BarCode}, MachineId={MachineId}", cmd.Command.BarCode, cmd.Command.MachineId);

        try
        {
            // Get bar code information
            logger.LogInformation("ProcessAsync: Getting bar code information for BarCode={BarCode}", cmd.Command.BarCode);
            var barCodeInfo = await this.GetBarCodeInformation(cmd, cancellationToken).ConfigureAwait(false);
            logger.LogInformation("ProcessAsync: GetBarCodeInformation completed, barCodeInfo is null: {IsNull}", barCodeInfo is null);

            if (barCodeInfo is null)
            {
                logger.LogError("ProcessAsync: BarCode not found for BarCode={BarCode}", cmd.Command.BarCode);
                return Result<TaskGatewayResponse>.WithFailure($"BarCode {cmd.Command.BarCode} not found");
            }

            logger.LogInformation("ProcessAsync: BarCode found, checking StationCannotUpdateCycles");

            // Check if station can update cycles
            if (StationCannotUpdateCycles(barCodeInfo))
            {
                logger.LogError("ProcessAsync: Station cannot update cycles for MachineType={MachineType}", barCodeInfo.MachineType);
                return Result<TaskGatewayResponse>.WithFailure($"Station cannot update cycles for machine type {barCodeInfo.MachineType}");
            }

            logger.LogInformation("ProcessAsync: Checking StationCannotUpdateCyclesCreatedOnAnotherStation");

            // Check if cycle was created on another station
            if (StationCannotUpdateCyclesCreatedOnAnotherStation(cmd, barCodeInfo))
            {
                logger.LogError(
                    "ProcessAsync: Cannot update cycles created on another station. NextMachineId={NextMachineId}, CommandMachineId={CommandMachineId}",
                    barCodeInfo.NextMachineId, cmd.Command.MachineId);
                return Result<TaskGatewayResponse>.WithFailure("Cannot update cycles created on another station");
            }

            logger.LogInformation("ProcessAsync: Checking StationCannotUpdateCyclesNotCreatedOnThisStation");

            // Check if cycle was not created on this station
            if (this.StationCannotUpdateCyclesNotCreatedOnThisStation(cmd, barCodeInfo))
            {
                logger.LogError(
                    "ProcessAsync: Cannot update cycles not created on this station. CommandMachineId={CommandMachineId}, CycleMachineId={CycleMachineId}",
                    cmd.Command.MachineId, barCodeInfo.Cycle.MachineId);
                return Result<TaskGatewayResponse>.WithFailure("Cannot update cycles not created on this station");
            }

            logger.LogInformation("ProcessAsync: Checking StationHaveCycleUpdatedOnThisStation");

            // Check if cycle has already been updated on this station
            if (this.StationHaveCycleUpdatedOnThisStation(cmd, barCodeInfo))
            {
                logger.LogError(
                    "ProcessAsync: Cycle has already been updated on this station. CommandMachineId={CommandMachineId}, CycleMachineId={CycleMachineId}, CycleStatus={CycleStatus}",
                    cmd.Command.MachineId, barCodeInfo.Cycle.MachineId, barCodeInfo.Cycle.CycleStatus);
                return Result<TaskGatewayResponse>.WithFailure("Cycle has already been updated on this station");
            }

            logger.LogInformation("ProcessAsync: All validations passed, updating cycle and barcode information");

            // Update cycle and barcode information
            var result = await this.UpdateBarCodeInformation(cmd, barCodeInfo, cancellationToken);
            logger.LogInformation("ProcessAsync: UpdateBarCodeInformation completed, result.IsSuccess={IsSuccess}", result.IsSuccess);

            if (!result.IsSuccess)
            {
                logger.LogError("ProcessAsync: UpdateBarCodeInformation failed with errors: {Errors}", string.Join(", ", result.Errors ?? []));
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ProcessAsync: Exception occurred");
            return Result<TaskGatewayResponse>.WithFailure($"Exception occurred: {ex.Message}");
        }
    }

    private bool StationHaveCycleUpdatedOnThisStation(UpdateCyclesOkCommand cmd, IBarCodeResult barCodeInfo)
    {
        var result = cmd.Command.MachineId == barCodeInfo.Cycle.MachineId && barCodeInfo.Cycle.CycleStatus != CycleStatus.Started;
        logger.LogInformation(
            "StationHaveCycleUpdatedOnThisStation: cmd.MachineId={CmdMachineId}, barCodeInfo.Cycle.MachineId={CycleMachineId}, barCodeInfo.Cycle.CycleStatus={CycleStatus}, result={Result}",
            cmd.Command.MachineId, barCodeInfo.Cycle.MachineId, barCodeInfo.Cycle.CycleStatus, result);
        return result;
    }

    private bool StationCannotUpdateCyclesNotCreatedOnThisStation(UpdateCyclesOkCommand cmd, IBarCodeResult barCodeInfo)
    {
        var result = cmd.Command.MachineId != barCodeInfo.Cycle.MachineId;
        logger.LogInformation(
            "StationCannotUpdateCyclesNotCreatedOnThisStation: cmd.MachineId={CmdMachineId}, barCodeInfo.Cycle.MachineId={CycleMachineId}, result={Result}",
            cmd.Command.MachineId, barCodeInfo.Cycle.MachineId, result);
        return result;
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
            GatewayTask = GatewayTask.UpdateCycleOkAsync,
            TimeStamp = dateTimeMachine.Now.ToLocalTime(),
        };

        await repositoryCommand.AddAsync(command, cancellationToken).ConfigureAwait(false);
    }

    private async Task<IBarCodeResult?> GetBarCodeInformation(UpdateCyclesOkCommand cmd, CancellationToken cancellationToken)
    {
        logger.LogInformation("GetBarCodeInformation: Starting with BarCode={BarCode}", cmd.Command.BarCode);

        try
        {
            logger.LogInformation("GetBarCodeInformation: Getting bar code result from service");
            var barCodeDetailsRequest = new BarCodeDetailsRequest(cmd.Command.MachineId, cmd.Command.BarCode, cmd.Command.PartNumber);
            var serviceResult = await barCodeResult.GetBarCodeDetails(barCodeDetailsRequest, cancellationToken);
            logger.LogInformation("GetBarCodeInformation: Service call completed, serviceResult is null: {IsNull}", serviceResult is null);

            if (serviceResult is null)
            {
                logger.LogError("GetBarCodeInformation: Bar code result is null");
                return null;
            }

            logger.LogInformation(
                "GetBarCodeInformation: Bar code result details - MachineId={MachineId}, NextMachineId={NextMachineId}, MachineType={MachineType}",
                serviceResult.MachineId, serviceResult.NextMachineId, serviceResult.MachineType);

            logger.LogInformation("GetBarCodeInformation: Successfully completed");
            return serviceResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetBarCodeInformation: Exception occurred");
            return null;
        }
    }

    private void UpdateCycleInformation(UpdateCyclesOkCommand cmd, IBarCodeResult barCodeInfo)
    {
        var request = cmd.Command;

        var cycle = barCodeInfo.Cycle;
        cycle.PartStatus = request.PartStatus;
        cycle.MachineId = request.MachineId;
        cycle.FinishedOn = dateTimeMachine.Now.ToLocalTime();
        cycle.CycleTime = (int)(cycle.FinishedOn - cycle.StartedOn).TotalSeconds;
        cycle.CycleStatus = request.CycleStatus;
    }

    private async Task<Result<TaskGatewayResponse>> UpdateBarCodeInformation(UpdateCyclesOkCommand cmd, IBarCodeResult barCodeInfo, CancellationToken cancellationToken)
    {
        logger.LogInformation("UpdateBarCodeInformation: Starting with BarCode={BarCode}, MachineId={MachineId}", cmd.Command.BarCode, cmd.Command.MachineId);

        try
        {
            logger.LogInformation("UpdateBarCodeInformation: Getting cycle from barCodeInfo");
            var cycle = barCodeInfo.Cycle;
            logger.LogInformation("UpdateBarCodeInformation: Cycle is null: {IsNull}", cycle is null);

            if (cycle is null)
            {
                logger.LogError("UpdateBarCodeInformation: Cycle is null");
                return Result<TaskGatewayResponse>.WithFailure("Cycle is null");
            }

            logger.LogInformation(
                "UpdateBarCodeInformation: Cycle details - CycleId={CycleId}, MachineId={MachineId}, CycleStatus={CycleStatus}",
                cycle.CycleId, cycle.MachineId, cycle.CycleStatus);

            logger.LogInformation("UpdateBarCodeInformation: Getting barcode from barCodeInfo");
            var barcode = barCodeInfo.BarCode;
            logger.LogInformation("UpdateBarCodeInformation: Barcode is null: {IsNull}", barcode is null);

            if (barcode is null)
            {
                logger.LogError("UpdateBarCodeInformation: Barcode is null");
                return Result<TaskGatewayResponse>.WithFailure("Barcode is null");
            }

            logger.LogInformation(
                "UpdateBarCodeInformation: Barcode details - BarCodeId={BarCodeId}, MachineId={MachineId}",
                barcode.BarCodeId, barcode.MachineId);

            logger.LogInformation("UpdateBarCodeInformation: Updating cycle information");
            this.UpdateCycleInformation(cmd, barCodeInfo);
            logger.LogInformation("UpdateBarCodeInformation: Cycle information updated");

            logger.LogInformation("UpdateBarCodeInformation: Cleaning register values");
            this.CleanRegisterValuesFromNonVisibleCharacters(cmd.Command.Registers, barCodeInfo.CycleId);
            logger.LogInformation("UpdateBarCodeInformation: Register values cleaned");

            logger.LogInformation("UpdateBarCodeInformation: Updating machine ID on registers");
            this.UpdateMachineIdOnRegisters(cmd.Command.Registers, barCodeInfo.MachineId);
            logger.LogInformation("UpdateBarCodeInformation: Machine ID updated on registers");

            logger.LogInformation("UpdateBarCodeInformation: Saving registers");
            var savedRegister = await this.SaveRegisters(cmd.Command.Registers.Values, cancellationToken);
            logger.LogInformation("UpdateBarCodeInformation: Registers saved, result.IsSuccess={IsSuccess}", savedRegister.IsSuccess);

            if (!savedRegister.IsSuccess)
            {
                logger.LogError("UpdateBarCodeInformation: Failed to save registers: {Error}", savedRegister.Errors.FirstOrDefault());
                return Result<TaskGatewayResponse>.WithFailure($"Failed to save registers: {savedRegister.Errors.FirstOrDefault()}");
            }

            logger.LogInformation("UpdateBarCodeInformation: Setting registers saved on barCodeInfo");
            barCodeInfo.SetRegistersSaved(savedRegister.Value);

            logger.LogInformation("UpdateBarCodeInformation: Updating cycle and barcode");
            var updateResult = await this.UpdateCycleAndBarcode(barCodeInfo, cancellationToken);
            logger.LogInformation("UpdateBarCodeInformation: Update cycle and barcode completed, result.IsSuccess={IsSuccess}", updateResult.IsSuccess);

            if (updateResult.IsFailure)
            {
                logger.LogError("UpdateBarCodeInformation: UpdateCycleAndBarcode failed: {Error}", updateResult.Errors.FirstOrDefault());
                return Result<TaskGatewayResponse>.WithFailure(updateResult.Errors);
            }

            logger.LogInformation("UpdateBarCodeInformation: Saving command");
            await this.SaveCommandAsync(cancellationToken, barCodeInfo);
            logger.LogInformation("UpdateBarCodeInformation: Command saved");

            logger.LogInformation("UpdateBarCodeInformation: Creating TaskGatewayResponse DTO");
            var barCodeResult = TaskGatewayResponse.ToDto(barCodeInfo);
            logger.LogInformation("UpdateBarCodeInformation: DTO created, applying reference values");
            _ = barCodeResult.ApplyReferencesValuesResult();
            logger.LogInformation("UpdateBarCodeInformation: Reference values applied");

            logger.LogInformation("UpdateBarCodeInformation: Successfully completed");
            return Result<TaskGatewayResponse>.Success(barCodeResult);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateBarCodeInformation: Exception occurred");
            return Result<TaskGatewayResponse>.WithFailure($"Exception in UpdateBarCodeInformation: {ex.Message}");
        }
    }

    private Task<Result<int>> SaveRegisters(IEnumerable<Register> registers, CancellationToken cancellationToken)
    {
        return registerRepository.AddRangeBulkAsync(registers, cancellationToken);
    }

    private async Task<Result<TaskGatewayResponse>> UpdateCycleAndBarcode(IBarCodeResult barCodeInfo, CancellationToken cancellationToken)
    {
        logger.LogInformation("UpdateCycleAndBarcode: Starting with CycleId={CycleId}", barCodeInfo.CycleId);
        logger.LogInformation("UpdateCycleAndBarcode: barCodeInfo.CycleId type = {Type}", barCodeInfo.CycleId.GetType().Name);

        // Check if CycleId is valid
        if (barCodeInfo.CycleId <= 0)
        {
            logger.LogError("UpdateCycleAndBarcode: barCodeInfo.CycleId is invalid: {CycleId}", barCodeInfo.CycleId);
            return Result<TaskGatewayResponse>.WithFailure($"Invalid CycleId: {barCodeInfo.CycleId}");
        }

        var cycleResult = await cycleRepository.GetByIdAsync(barCodeInfo.CycleId, cancellationToken);
        logger.LogInformation("UpdateCycleAndBarcode: Cycle repository call completed, result.IsSuccess={IsSuccess}", cycleResult.IsSuccess);

        if (cycleResult.IsFailure)
        {
            return Result<TaskGatewayResponse>.WithFailure($"Cycle not Found CycleId: {barCodeInfo.CycleId.ToString()}");
        }

        var cycle = cycleResult.Value;
        if (cycle is null)
        {
            return Result<TaskGatewayResponse>.WithFailure("Cycle entity is null");
        }

        logger.LogInformation("UpdateCycleAndBarcode: Cycle retrieved successfully, CycleId={CycleId}", cycle.CycleId);

        logger.LogInformation("UpdateCycleAndBarcode: About to get barcode, barCodeInfo.BarCode is null: {IsNull}", barCodeInfo.BarCode is null);
        if (barCodeInfo.BarCode is null)
        {
            logger.LogError("UpdateCycleAndBarcode: barCodeInfo.BarCode is null");
            return Result<TaskGatewayResponse>.WithFailure("BarCode is null");
        }

        logger.LogInformation("UpdateCycleAndBarcode: barCodeInfo.BarCode.BarCodeId = {BarCodeId}", barCodeInfo.BarCode.BarCodeId);

        // Check if BarCodeId is null or invalid
        if (barCodeInfo.BarCode.BarCodeId <= 0)
        {
            logger.LogError("UpdateCycleAndBarcode: barCodeInfo.BarCode.BarCodeId is invalid: {BarCodeId}", barCodeInfo.BarCode.BarCodeId);
            return Result<TaskGatewayResponse>.WithFailure($"Invalid BarCodeId: {barCodeInfo.BarCode.BarCodeId}");
        }

        logger.LogInformation("UpdateCycleAndBarcode: About to call barCodeRepository.GetByIdAsync with BarCodeId={BarCodeId}", barCodeInfo.BarCode.BarCodeId);

        var barcodeResult = await barCodeRepository.GetByIdAsync(barCodeInfo.BarCode.BarCodeId, cancellationToken);
        logger.LogInformation("UpdateCycleAndBarcode: barCodeRepository.GetByIdAsync completed, result.IsSuccess={IsSuccess}", barcodeResult.IsSuccess);

        if (barcodeResult.IsFailure)
        {
            return Result<TaskGatewayResponse>.WithFailure($"BarCode not Found BarCodeId: {barCodeInfo.BarCodeId.ToString()}");
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

    private FlowStatus DetermineFlowStatus(UpdateCyclesOkCommand request, IBarCodeResult barCodeInfo)
    {
        // If the part status of the monitorRequest is finished successfully and the machine type is "Process",
        // then update the flow status of the bar code to "InProcess".
        return IsProcessFinishedOkOnFinalStation(request, barCodeInfo) ? FlowStatus.Finished : FlowStatus.InProcess;
    }

    public static bool IsProcessFinishedOkOnFinalStation(UpdateCyclesOkCommand request, IBarCodeResult barCodeInfo)
    {
        return Equals(request.Command.CycleStatus, CycleStatus.FinishedOk) && Equals(barCodeInfo.MachineType, MachineType.Final);
    }

    private static bool StationCannotUpdateCyclesCreatedOnAnotherStation(UpdateCyclesOkCommand request, IBarCodeResult barCodeInfo)
    {
        var result = barCodeInfo.NextMachineId != request.Command.MachineId;

        // Note: Using static method, so we can't use logger here
        return result;
    }

    private static bool IsCycleTimeInvalid(int cycleTime, Recipe? recipe)
    {
        if (recipe is null)
        {
            // Note: Using static method, so we can't use logger here
            return true;
        }

        var result = cycleTime <= recipe.CycleTimeMinimum || cycleTime >= recipe.CycleTimeMaximum;

        // Note: Using static method, so we can't use logger here
        return result;
    }

    private static bool StationCannotUpdateCycles(IBarCodeResult barCodeInfo)
    {
        var result = !Equals(barCodeInfo.MachineType, MachineType.Printer)
            && !Equals(barCodeInfo.MachineType, MachineType.Process)
            && !Equals(barCodeInfo.MachineType, MachineType.Final);

        // Note: Using static method, so we can't use logger here
        return result;
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
