// <copyright file="UpdateCyclesCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Commands.UpdateCycles;

/// <summary>
/// Unified handler for cycle update commands using SRP services.
/// </summary>
public class UpdateCyclesCommandHandler :
    IGatewayRequestHandler<UpdateCyclesOkCommand, TaskGatewayResponse>,
    IGatewayRequestHandler<UpdateCyclesNotOkCommand, TaskGatewayResponse>
{
    private readonly IBarCodeInfoProvider _barCodeInfoProvider;
    private readonly IStationValidator _stationValidator;
    private readonly ICycleUpdateStrategyFactory _strategyFactory;
    private readonly ICommandLogger _commandLogger;
    private readonly ILogger<UpdateCyclesCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCyclesCommandHandler"/> class.
    /// </summary>
    public UpdateCyclesCommandHandler(
        IBarCodeInfoProvider barCodeInfoProvider,
        IStationValidator stationValidator,
        ICycleUpdateStrategyFactory strategyFactory,
        ICommandLogger commandLogger,
        ILogger<UpdateCyclesCommandHandler> logger)
    {
        _barCodeInfoProvider = barCodeInfoProvider;
        _stationValidator = stationValidator;
        _strategyFactory = strategyFactory;
        _commandLogger = commandLogger;
        _logger = logger;
    }

    /// <summary>
    /// Processes an OK cycle update command.
    /// </summary>
    public Task<Result<TaskGatewayResponse>> ProcessAsync(
        UpdateCyclesOkCommand cmd,
        CancellationToken cancellationToken)
    {
        return ProcessInternalAsync(
            new UpdateCycleCommandAdapter(cmd),
            CycleStatus.FinishedOk,
            GatewayTask.UpdateCycleOkAsync,
            cancellationToken);
    }

    /// <summary>
    /// Processes a NOT OK cycle update command.
    /// </summary>
    public Task<Result<TaskGatewayResponse>> ProcessAsync(
        UpdateCyclesNotOkCommand cmd,
        CancellationToken cancellationToken)
    {
        return ProcessInternalAsync(
            new UpdateCycleCommandAdapter(cmd),
            CycleStatus.FinishedNok,
            GatewayTask.UpdateCycleNotOkAsync,
            cancellationToken);
    }

    private async Task<Result<TaskGatewayResponse>> ProcessInternalAsync(
        IUpdateCycleCommand command,
        CycleStatus targetStatus,
        GatewayTask gatewayTask,
        CancellationToken cancellationToken)
    {
        if (command is null)
        {
            return Result<TaskGatewayResponse>.WithFailure("Command cannot be null");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return ResultExtensions.Cancelled<TaskGatewayResponse>();
        }

        _logger.LogInformation(
            "Processing cycle update: BarCode={BarCode}, MachineId={MachineId}, TargetStatus={TargetStatus}",
            command.BarCode, command.MachineId, targetStatus);

        try
        {
            // Get barcode information
            var barCodeInfoResult = await _barCodeInfoProvider
                .GetBarCodeInfoAsync(command.MachineId, command.BarCode, command.PartNumber, cancellationToken)
                .ConfigureAwait(false);

            if (barCodeInfoResult.IsFailure || barCodeInfoResult.Value is null)
            {
                _logger.LogError("Failed to get barcode info: {Errors}", string.Join(", ", barCodeInfoResult.Errors));
                return Result<TaskGatewayResponse>.WithFailure(barCodeInfoResult.Errors);
            }

            var barCodeInfo = barCodeInfoResult.Value;

            // Validate station capabilities
            var validationResult = _stationValidator.ValidateStation(command.MachineId, targetStatus, barCodeInfo);
            if (validationResult.IsFailure)
            {
                _logger.LogError("Station validation failed: {Errors}", string.Join(", ", validationResult.Errors));
                return Result<TaskGatewayResponse>.WithFailure(validationResult.Errors);
            }

            if (validationResult.Value is null)
            {
                _logger.LogError("Station validation returned null result");
                return Result<TaskGatewayResponse>.WithFailure("Station validation returned null result");
            }

            if (!validationResult.Value.CanUpdate)
            {
                _logger.LogError("Station cannot update: {Reason}", validationResult.Value.FailureReason);
                return Result<TaskGatewayResponse>.WithFailure(validationResult.Value.FailureReason ?? "Station validation failed");
            }

            // Execute strategy
            var strategy = _strategyFactory.CreateStrategy(targetStatus);
            var updateResult = await strategy.ExecuteAsync(command, barCodeInfo, cancellationToken)
                .ConfigureAwait(false);

            if (updateResult.IsFailure || updateResult.Value is null)
            {
                _logger.LogError("Strategy execution failed: {Errors}", string.Join(", ", updateResult.Errors));
                return Result<TaskGatewayResponse>.WithFailure(updateResult.Errors);
            }

            // Update barcode info with results
            var result = updateResult.Value;
            barCodeInfo.SetCycle(result.UpdatedCycle);
            barCodeInfo.SetBarCode(result.UpdatedBarCode);
            barCodeInfo.SetRegistersSaved(result.RegistersSaved);
            if (result.CyclesOk.HasValue)
            {
                barCodeInfo.SetCyclesOk(result.CyclesOk.Value);
            }

            // Log command
            var logCommand = _commandLogger.CreateCommand(barCodeInfo, gatewayTask);
            await _commandLogger.LogCommandAsync(logCommand, cancellationToken)
                .ConfigureAwait(false);

            // Create response
            var response = TaskGatewayResponse.ToDto(barCodeInfo);
            response.ApplyReferencesValuesResult();

            _logger.LogInformation(
                "Cycle update completed successfully: RegistersSaved={RegistersSaved}, CyclesOk={CyclesOk}",
                result.RegistersSaved, result.CyclesOk);

            return Result<TaskGatewayResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception processing cycle update");
            return Result<TaskGatewayResponse>.WithFailure($"Exception occurred: {ex.Message}");
        }
    }
}
