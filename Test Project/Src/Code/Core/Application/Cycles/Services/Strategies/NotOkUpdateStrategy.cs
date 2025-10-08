// <copyright file="NotOkUpdateStrategy.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Services.Strategies;

/// <summary>
/// Strategy for updating cycles with NOT OK status.
/// </summary>
public class NotOkUpdateStrategy : ICycleUpdateStrategy
{
    private readonly IRegisterCleaner _registerCleaner;
    private readonly IPersistenceOrchestrator _persistenceOrchestrator;
    private readonly IShiftService _shiftService;
    private readonly IFlowStatusCalculator _flowStatusCalculator;
    private readonly IDateTimeMachine _dateTimeMachine;
    private readonly ILogger<NotOkUpdateStrategy> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotOkUpdateStrategy"/> class.
    /// </summary>
    public NotOkUpdateStrategy(
        IRegisterCleaner registerCleaner,
        IPersistenceOrchestrator persistenceOrchestrator,
        IShiftService shiftService,
        IFlowStatusCalculator flowStatusCalculator,
        IDateTimeMachine dateTimeMachine,
        ILogger<NotOkUpdateStrategy> logger)
    {
        _registerCleaner = registerCleaner;
        _persistenceOrchestrator = persistenceOrchestrator;
        _shiftService = shiftService;
        _flowStatusCalculator = flowStatusCalculator;
        _dateTimeMachine = dateTimeMachine;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<CycleUpdateResult>> ExecuteAsync(
        IUpdateCycleCommand command,
        IBarCodeResult barCodeInfo,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("ExecuteAsync cancelled");
            return ResultExtensions.Cancelled<CycleUpdateResult>();
        }

        _logger.LogInformation(
            "Executing NOT OK update strategy for CycleId={CycleId}, MachineId={MachineId}",
            barCodeInfo.CycleId, command.MachineId);

        try
        {
            // Update cycle information
            var cycle = barCodeInfo.Cycle;
            cycle.PartStatus = command.PartStatus;
            cycle.MachineId = command.MachineId;
            cycle.FinishedOn = _dateTimeMachine.Now.ToLocalTime();
            cycle.CycleTime = (int)(cycle.FinishedOn - cycle.StartedOn).TotalSeconds;
            cycle.CycleStatus = command.CycleStatus;

            // Get or create shift (NOK cycles don't increment CyclesOk)
            var shiftResult = await _shiftService
                .CreateOrRetrieveShiftAndCyclesOkAsync(command.MachineId, cancellationToken)
                .ConfigureAwait(false);

            if (shiftResult.IsFailure)
            {
                _logger.LogError("Failed to get shift: {Error}", shiftResult.Error);
                return Result<CycleUpdateResult>.WithFailure($"Cannot create shift: {shiftResult.Error}");
            }

            var shift = shiftResult.Value;
            cycle.CyclesOk = shift!.CyclesOk; // Use existing count, don't increment

            // Update barcode information
            var barCode = barCodeInfo.BarCode;
            barCode.PartStatus = command.PartStatus;
            barCode.ModifiedOn = _dateTimeMachine.Now.ToLocalTime();
            barCode.MachineId = command.MachineId;
            barCode.FlowStatus = _flowStatusCalculator.Calculate(
                barCodeInfo.MachineType,
                cycle.CycleStatus,
                cycle.PartStatus);

            // Update barcode info to reflect changes
            barCodeInfo.UpdateBarCodeInformationOnCycle(
                barCode.FlowStatus,
                barCode.PartStatus,
                command.CycleStatus);

            // Clean and prepare registers
            var cleanResult = _registerCleaner.CleanRegisters(
                command.Registers,
                barCodeInfo.CycleId,
                command.MachineId,
                _dateTimeMachine.Now.ToLocalTime());

            if (cleanResult.IsFailure || cleanResult.Value is null)
            {
                _logger.LogError("Failed to clean registers: {Error}", cleanResult.Error);
                return Result<CycleUpdateResult>.WithFailure(cleanResult.Errors);
            }

            // Persist everything
            var persistResult = await _persistenceOrchestrator
                .PersistAsync(cleanResult.Value, cycle, barCode, cancellationToken)
                .ConfigureAwait(false);

            if (persistResult.IsFailure || persistResult.Value is null)
            {
                _logger.LogError("Failed to persist: {Error}", persistResult.Error);
                return Result<CycleUpdateResult>.WithFailure(persistResult.Errors);
            }

            var result = new CycleUpdateResult(
                UpdatedCycle: cycle,
                UpdatedBarCode: barCode,
                RegistersSaved: persistResult.Value.RegistersSaved,
                CyclesOk: shift.CyclesOk,
                ShiftInfo: new ShiftInfo(shift.ShiftId, shift.CyclesOk));

            _logger.LogInformation(
                "NOT OK update completed successfully: RegistersSaved={RegistersSaved}, CyclesOk={CyclesOk}",
                result.RegistersSaved, result.CyclesOk);

            return Result<CycleUpdateResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in NOT OK update strategy");
            return Result<CycleUpdateResult>.WithFailure($"Exception in NOT OK update: {ex.Message}");
        }
    }
}
