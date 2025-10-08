// <copyright file="PerformanceDataCommandFactory.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Performance.Request.Command.Create;

/// <summary>
/// Represents a command for performance data operations, including creation and conversion utilities.
/// </summary>
public class PerformanceDataCommand : Domain.Entities.PerformanceData, IGatewayRequest<TaskGatewayResponse>, ICommandData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceDataCommand"/> class with a task gateway request.
    /// </summary>
    /// <param name="taskGatewayRequest">The task gateway request to associate with the command.</param>
    public PerformanceDataCommand(TaskGatewayRequest taskGatewayRequest)
    {
        this.Command = taskGatewayRequest;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceDataCommand"/> class.
    /// </summary>
    public PerformanceDataCommand()
    {
    }

    /// <summary>
    /// Creates a new <see cref="PerformanceDataCommand"/> instance from a task gateway request.
    /// </summary>
    /// <param name="taskGatewayRequest">The task gateway request to use for creation.</param>
    /// <returns>A new <see cref="PerformanceDataCommand"/> instance.</returns>
    public ICommandData Create(TaskGatewayRequest taskGatewayRequest)
    {
        return new PerformanceDataCommand(taskGatewayRequest);
    }

    /// <summary>
    /// Gets or sets the unique identifier for the performance data.
    /// </summary>
    public new long PerformanceDataId { get; set; }

    /// <summary>
    /// Gets or sets the machine identifier.
    /// </summary>
    public new int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the PLC identifier.
    /// </summary>
    public new int PlcId { get; set; }

    /// <summary>
    /// Gets or sets the barcode identifier.
    /// </summary>
    public new int BarCodeId { get; set; }

    /// <summary>
    /// Gets or sets the cycle identifier.
    /// </summary>
    public new int CycleId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the performance data.
    /// </summary>
    public new DateTime TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the application flag.
    /// </summary>
    public new int ApplicationFlag { get; set; }

    /// <summary>
    /// Gets or sets the event counter.
    /// </summary>
    public new int EventCounter { get; set; }

    /// <summary>
    /// Gets or sets the current time value.
    /// </summary>
    public new int CurrentTime { get; set; }

    /// <summary>
    /// Gets or sets the running time value.
    /// </summary>
    public new int RunningTime { get; set; }

    /// <summary>
    /// Gets or sets the stopped time value.
    /// </summary>
    public new int StoppedTime { get; set; }

    /// <summary>
    /// Gets or sets the faulted time value.
    /// </summary>
    public new int FaultedTime { get; set; }

    /// <summary>
    /// Gets or sets the status fault reason.
    /// </summary>
    public new int StatusFaultReason { get; set; }

    /// <summary>
    /// Gets or sets the total production value.
    /// </summary>
    public new double TotalProduction { get; set; }

    /// <summary>
    /// Gets or sets the number of OK productions.
    /// </summary>
    public new double ProductionOk { get; set; }

    /// <summary>
    /// Gets or sets the number of NoK productions.
    /// </summary>
    public new double ProductionNoK { get; set; }

    /// <summary>
    /// Gets or sets the status fault reject value.
    /// </summary>
    public new int StatusFaultReject { get; set; }

    /// <summary>
    /// Resets all performance data properties to their default values.
    /// </summary>
    /// <returns>True if the reset was successful; otherwise, false.</returns>
    public bool TryReset()
    {
        this.PerformanceDataId = 0;
        this.MachineId = 0;
        this.PlcId = 0;
        this.BarCodeId = 0;
        this.CycleId = 0;
        this.TimeStamp = DateTime.MinValue;
        this.ApplicationFlag = 0;
        this.EventCounter = 0;
        this.CurrentTime = 0;
        this.RunningTime = 0;
        this.StoppedTime = 0;
        this.FaultedTime = 0;
        this.StatusFaultReason = 0;
        this.TotalProduction = 0.0;
        this.ProductionOk = 0.0;
        this.ProductionNoK = 0.0;
        this.StatusFaultReject = 0;
        return true;
    }

    /// <summary>
    /// Converts a <see cref="PerformanceDataCommand"/> to a <see cref="Domain.Entities.PerformanceData"/> entity.
    /// </summary>
    /// <param name="command">The performance data command to convert.</param>
    /// <returns>A <see cref="Domain.Entities.PerformanceData"/> entity.</returns>
    public static Domain.Entities.PerformanceData ToEntity(PerformanceDataCommand command)
    {
        return new Domain.Entities.PerformanceData
        {
            PerformanceDataId = command.PerformanceDataId,
            MachineId = command.MachineId,
            PlcId = command.PlcId,
            BarCodeId = command.BarCodeId,
            CycleId = command.CycleId,
            TimeStamp = command.TimeStamp,
            ApplicationFlag = command.ApplicationFlag,
            EventCounter = command.EventCounter,
            CurrentTime = command.CurrentTime,
            RunningTime = command.RunningTime,
            StoppedTime = command.StoppedTime,
            FaultedTime = command.FaultedTime,
            StatusFaultReason = command.StatusFaultReason,
            TotalProduction = command.TotalProduction,
            ProductionOk = command.ProductionOk,
            ProductionNoK = command.ProductionNoK,
            StatusFaultReject = command.StatusFaultReject,
        };
    }

    /// <summary>
    /// Updates the command's data from a result object.
    /// </summary>
    /// <param name="result">The result containing updated data.</param>
    public void UpdateDataFromResult(Result<TaskGatewayResponse> result)
    {
        if (result is null || !result.IsSuccess || result.Value is null)
        {
            return;
        }

        var v = result.Value;
        this.Command = new TaskGatewayRequest(v.MachineId, v.PartNumber, v.CycleStatus, v.PartStatus);
        this.BarCodeId = v.BarCodeId;
        this.CycleId = v.CycleId;
        this.TimeStamp = v.TimeStamp;
    }

    /// <summary>
    /// Creates a <see cref="PerformanceDataCommand"/> from a dictionary of performance registers.
    /// </summary>
    /// <param name="perfomances">The dictionary of performance registers.</param>
    /// <returns>A <see cref="PerformanceDataCommand"/> instance.</returns>
    /// <summary>
    /// Creates a <see cref="PerformanceDataCommand"/> from a dictionary of performance registers.
    /// </summary>
    /// <param name="perfomances">The dictionary of performance registers.</param>
    /// <returns>A Result containing the PerformanceDataCommand instance or failure information.</returns>
    public static new Result<PerformanceDataCommand> FromPlc(IDictionary<string, Register> perfomances)
    {
        if (perfomances == null)
        {
            return Result<PerformanceDataCommand>.WithFailure($"Parameter '{nameof(perfomances)}' cannot be null");
        }

        var result = new PerformanceDataCommand();

        if (perfomances.TryGetValue(nameof(ApplicationFlag), out var applicationFlag))
        {
            result.ApplicationFlag = Convert.ToInt32(applicationFlag.Value);
        }

        if (perfomances.TryGetValue(nameof(EventCounter), out var eventCounter))
        {
            result.EventCounter = Convert.ToInt32(eventCounter.Value);
        }

        if (perfomances.TryGetValue(nameof(CurrentTime), out var currentTime))
        {
            result.CurrentTime = Convert.ToInt32(currentTime.Value);
        }

        if (perfomances.TryGetValue(nameof(RunningTime), out var runningTime))
        {
            result.RunningTime = Convert.ToInt32(runningTime.Value);
        }

        if (perfomances.TryGetValue(nameof(StoppedTime), out var stoppedTime))
        {
            result.StoppedTime = Convert.ToInt32(stoppedTime.Value);
        }

        if (perfomances.TryGetValue(nameof(FaultedTime), out var faultedTime))
        {
            result.FaultedTime = Convert.ToInt32(faultedTime.Value);
        }

        if (perfomances.TryGetValue(nameof(StatusFaultReason), out var statusFaultReason))
        {
            result.StatusFaultReason = Convert.ToInt32(statusFaultReason.Value);
        }

        if (perfomances.TryGetValue(nameof(TotalProduction), out var totalProduction))
        {
            result.TotalProduction = Convert.ToDouble(totalProduction.Value);
        }

        if (perfomances.TryGetValue(nameof(ProductionOk), out var productionOk))
        {
            result.ProductionOk = Convert.ToDouble(productionOk.Value);
        }

        if (perfomances.TryGetValue(nameof(ProductionNoK), out var productionNoK))
        {
            result.ProductionNoK = Convert.ToDouble(productionNoK.Value);
        }

        if (perfomances.TryGetValue(nameof(StatusFaultReject), out var statusFaultReject))
        {
            result.StatusFaultReject = Convert.ToInt32(statusFaultReject.Value);
        }

        return Result<PerformanceDataCommand>.Success(result);
    }
}
