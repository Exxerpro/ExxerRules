// <copyright file="CommandLogger.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Services;

/// <summary>
/// Logs gateway commands for audit and tracking.
/// </summary>
public class CommandLogger : ICommandLogger
{
    private readonly IRepository<TaskGatewayRequest> _commandRepository;
    private readonly IDateTimeMachine _dateTimeMachine;
    private readonly ILogger<CommandLogger> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLogger"/> class.
    /// </summary>
    /// <param name="commandRepository">The command repository.</param>
    /// <param name="dateTimeMachine">The date time service.</param>
    /// <param name="logger">The logger instance.</param>
    public CommandLogger(
        IRepository<TaskGatewayRequest> commandRepository,
        IDateTimeMachine dateTimeMachine,
        ILogger<CommandLogger> logger)
    {
        _commandRepository = commandRepository;
        _dateTimeMachine = dateTimeMachine;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result> LogCommandAsync(
        TaskGatewayRequest command,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("LogCommandAsync cancelled");
            return ResultExtensions.Cancelled();
        }

        if (command is null)
        {
            _logger.LogError("Command is null");
            return ResultExtensions.FailForNullArgument(nameof(command));
        }

        _logger.LogInformation(
            "Logging command: GatewayTask={GatewayTask}, MachineId={MachineId}, BarCodeId={BarCodeId}",
            command.GatewayTask, command.MachineId, command.BarCodeId);

        try
        {
            var result = await _commandRepository
                .AddAsync(command, cancellationToken)
                .ConfigureAwait(false);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Command logged successfully with Id={Id}", result.Value);
            }
            else
            {
                _logger.LogError("Failed to log command: {Error}", result.Error);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception logging command");
            return Result.WithFailure($"Exception logging command: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public TaskGatewayRequest CreateCommand(
        IBarCodeResult barCodeInfo,
        GatewayTask gatewayTask,
        string? comment = null)
    {
        if (barCodeInfo is null)
        {
            throw new ArgumentNullException(nameof(barCodeInfo));
        }

        return new TaskGatewayRequest
        {
            MachineId = barCodeInfo.MachineId,
            ProductId = barCodeInfo.Product.ProductId,
            PartNumber = barCodeInfo.Product.PartNumber,
            BarCodeId = barCodeInfo.BarCodeId,
            BarCode = barCodeInfo.BarCode.Label,
            GatewayTask = gatewayTask,
            TimeStamp = _dateTimeMachine.Now,
            Comment = comment ?? string.Empty,
            IsCompleted = false
        };
    }
}
