// <copyright file="StationValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Services;

/// <summary>
/// Validates station capabilities for cycle updates.
/// </summary>
public class StationValidator : IStationValidator
{
    private readonly ILogger<StationValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StationValidator"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public StationValidator(ILogger<StationValidator> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public Result<StationValidationResult> ValidateStation(
        int machineId,
        CycleStatus cycleStatus,
        IBarCodeResult barCodeInfo)
    {
        if (barCodeInfo is null)
        {
            _logger.LogError("BarCodeInfo is null");
            return Result<StationValidationResult>.WithFailure("BarCodeInfo cannot be null");
        }

        _logger.LogInformation(
            "Validating station: MachineId={MachineId}, CycleStatus={CycleStatus}, MachineType={MachineType}",
            machineId, cycleStatus, barCodeInfo.MachineType);

        // Check if station can update cycles
        if (!CanStationUpdateCycles(barCodeInfo.MachineType))
        {
            var reason = $"Station cannot update cycles for machine type {barCodeInfo.MachineType}";
            _logger.LogError(reason);
            return Result<StationValidationResult>.Success(
                new StationValidationResult(false, reason, ResultValidation.WorkFlowNotValid));
        }

        // Check if cycle was created on another station
        if (barCodeInfo.NextMachineId != machineId)
        {
            var reason = $"Cannot update cycles created on another station. NextMachineId={barCodeInfo.NextMachineId}, CommandMachineId={machineId}";
            _logger.LogError(reason);
            return Result<StationValidationResult>.Success(
                new StationValidationResult(false, reason, ResultValidation.DestinationNotValid));
        }

        // Check if cycle was not created on this station
        if (machineId != barCodeInfo.Cycle.MachineId)
        {
            var reason = $"Cannot update cycles not created on this station. CommandMachineId={machineId}, CycleMachineId={barCodeInfo.Cycle.MachineId}";
            _logger.LogError(reason);
            return Result<StationValidationResult>.Success(
                new StationValidationResult(false, reason, ResultValidation.Invalid));
        }

        // Check if cycle has already been updated on this station
        if (machineId == barCodeInfo.Cycle.MachineId && barCodeInfo.Cycle.CycleStatus != CycleStatus.Started)
        {
            var reason = $"Cycle has already been updated on this station. CycleStatus={barCodeInfo.Cycle.CycleStatus}";
            _logger.LogError(reason);
            return Result<StationValidationResult>.Success(
                new StationValidationResult(false, reason, ResultValidation.WorkFlowNotValid));
        }

        _logger.LogInformation("Station validation passed");
        return Result<StationValidationResult>.Success(
            new StationValidationResult(true, null, ResultValidation.Valid));
    }

    private bool CanStationUpdateCycles(MachineType machineType)
    {
        var canUpdate = machineType == MachineType.Printer
            || machineType == MachineType.InitialPrinter
            || machineType == MachineType.Process
            || machineType == MachineType.Final
            || machineType == MachineType.Initial;

        _logger.LogInformation(
            "CanStationUpdateCycles: MachineType={MachineType}, CanUpdate={CanUpdate}",
            machineType, canUpdate);

        return canUpdate;
    }
}
