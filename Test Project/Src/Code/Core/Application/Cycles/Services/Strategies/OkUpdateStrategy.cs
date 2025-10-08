// <copyright file="OkUpdateStrategy.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Services.Strategies;

/// <summary>
/// Strategy for updating cycles with OK status.
/// </summary>
public class OkUpdateStrategy : ICycleUpdateStrategy
{
    private readonly IRegisterCleaner _registerCleaner;
    private readonly IPersistenceOrchestrator _persistenceOrchestrator;
    private readonly IShiftService _shiftService;
    private readonly ICycleTimeValidator _cycleTimeValidator;
    private readonly IFlowStatusCalculator _flowStatusCalculator;
    private readonly IDateTimeMachine _dateTimeMachine;
    private readonly ILogger<OkUpdateStrategy> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OkUpdateStrategy"/> class.
    /// </summary>
    public OkUpdateStrategy(
        IRegisterCleaner registerCleaner,
        IPersistenceOrchestrator persistenceOrchestrator,
        IShiftService shiftService,
        ICycleTimeValidator cycleTimeValidator,
        IFlowStatusCalculator flowStatusCalculator,
        IDateTimeMachine dateTimeMachine,
        ILogger<OkUpdateStrategy> logger)
    {
        _registerCleaner = registerCleaner;
        _persistenceOrchestrator = persistenceOrchestrator;
        _shiftService = shiftService;
        _cycleTimeValidator = cycleTimeValidator;
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
            "Executing OK update strategy for CycleId={CycleId}, MachineId={MachineId}",
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

            // Validate cycle time
            var cycleTimeValidation = _cycleTimeValidator.Validate(cycle.CycleTime, barCodeInfo.Recipe);
            if (cycleTimeValidation.ShouldForceNok)
            {
                _logger.LogWarning("Cycle time validation failed: {Reason}", cycleTimeValidation.FailureReason);
                cycle.CycleStatus = CycleStatus.FinishedNok;
                cycle.PartStatus = PartStatus.NOk;
            }

            // Get or create shift for cycles OK tracking
            var shiftResult = await _shiftService
                .CreateOrRetrieveShiftAndCyclesOkAsync(command.MachineId, cancellationToken)
                .ConfigureAwait(false);

            if (shiftResult.IsFailure)
            {
                _logger.LogError("Failed to get shift: {Error}", shiftResult.Error);
                return Result<CycleUpdateResult>.WithFailure($"Cannot create shift: {shiftResult.Error}");
            }

            var shift = shiftResult.Value;
            cycle.CyclesOk = shift!.CyclesOk;

            // Update barcode information
            var barCode = barCodeInfo.BarCode;
            barCode.ModifiedOn = _dateTimeMachine.Now.ToLocalTime();
            barCode.MachineId = command.MachineId;
            barCode.FlowStatus = _flowStatusCalculator.Calculate(
                barCodeInfo.MachineType,
                cycle.CycleStatus,
                cycle.PartStatus);

            // If cycle time was invalid, update barcode part status
            if (cycleTimeValidation.ShouldForceNok)
            {
                barCode.PartStatus = PartStatus.NOk;
            }

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
                "OK update completed successfully: RegistersSaved={RegistersSaved}, CyclesOk={CyclesOk}",
                result.RegistersSaved, result.CyclesOk);

            return cycleTimeValidation.IsValid
                ? Result<CycleUpdateResult>.Success(result)
                : Result<CycleUpdateResult>.WithFailure("Cycle time is invalid", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in OK update strategy");
            return Result<CycleUpdateResult>.WithFailure($"Exception in OK update: {ex.Message}");
        }
    }
}
