// <copyright file="StationMonitor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.UI.Models;

/// <summary>
/// Represents a station monitor with comprehensive monitoring capabilities.
/// </summary>
public class StationMonitor : IMonitorFilter
{
    private readonly IDateTimeMachine dateTimeMachine;

    /// <summary>
    /// Initializes a new instance of the <see cref="StationMonitor"/> class with optional date time machine dependency.
    /// </summary>
    /// <param name="dateTimeMachine">The date time machine to use for timestamp operations. Defaults to new DateTimeMachine() if null.</param>
    public StationMonitor(IDateTimeMachine? dateTimeMachine = null)
    {
        this.dateTimeMachine = dateTimeMachine ?? new DateTimeMachine();

        this.ResultValidation = ResultValidation.None;
        this.CycleStatus = CycleStatus.None;
        this.FlowStatus = FlowStatus.None;
        this.PartStatus = PartStatus.None;
        this.MachineType = MachineType.None;
        this.WorkFlowType = WorkFlowType.None;
        this.GatewayTask = GatewayTask.None;

        // Initialize with default date 2020-01-01 when no dependency provided
        this.TimeStamp = this.dateTimeMachine.Now;
    }

    /// <summary>
    /// Gets or sets the status request message.
    /// </summary>
    public string StatusRequest { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the station.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the label value.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the part number.
    /// </summary>
    public string PartNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the result validation status.
    /// </summary>
    public ResultValidation ResultValidation { get; set; }

    /// <summary>
    /// Gets or sets the cycle status.
    /// </summary>
    public CycleStatus CycleStatus { get; set; }

    /// <summary>
    /// Gets or sets the flow status.
    /// </summary>
    public FlowStatus FlowStatus { get; set; }

    /// <summary>
    /// Gets or sets the part status.
    /// </summary>
    public PartStatus PartStatus { get; set; }

    /// <summary>
    /// Gets or sets the machine type.
    /// </summary>
    public MachineType MachineType { get; set; }

    /// <summary>
    /// Gets or sets the workflow type.
    /// </summary>
    public WorkFlowType WorkFlowType { get; set; }

    /// <summary>
    /// Gets or sets the gateway task.
    /// </summary>
    public GatewayTask GatewayTask { get; set; }

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string RequestTask { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public ProductionData ProductionData { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the parameters dictionary.
    /// </summary>
    public Dictionary<string, string> Parameters { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="StationMonitor"/> class.
    /// </summary>
    public StationMonitor()
        : this(null)
    {
    }

    /// <summary>
    /// Gets or sets the machine identifier.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the PLC identifier.
    /// </summary>
    public int PlcId { get; set; }

    /// <summary>
    /// Gets or sets the barcode identifier.
    /// </summary>
    public int BarCodeId { get; set; }

    /// <summary>
    /// Gets or sets the cycle identifier.
    /// </summary>
    public int CycleId { get; set; }

    /// <summary>
    /// Gets or sets the number of successful cycles.
    /// </summary>
    public int CyclesOk { get; set; }

    /// <summary>
    /// Gets or sets the shift identifier.
    /// </summary>
    public int ShiftId { get; set; }

    /// <summary>
    /// Gets or sets the last machine identifier.
    /// </summary>
    public int LastMachineId { get; set; }

    /// <summary>
    /// Gets or sets the next machine identifier.
    /// </summary>
    public int NextMachineId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the station is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the event status.
    /// </summary>
    public string EventStatus { get; set; } = string.Empty;

    /// <summary>
    /// Ensures the station is valid for rendering and persistence by setting default values for null properties.
    /// </summary>
    /// <returns>True if the station is valid; otherwise, false.</returns>
    public bool EnsureIsValidToRenderAndPersist()
    {
        this.FlowStatus ??= FlowStatus.None;
        this.CycleStatus ??= CycleStatus.None;
        this.ResultValidation ??= ResultValidation.None;
        this.PartStatus ??= PartStatus.None;
        this.MachineType ??= MachineType.None;
        this.WorkFlowType ??= WorkFlowType.None;
        this.GatewayTask ??= GatewayTask.None;
        return true;
    }

    /// <summary>
    /// Creates a new station monitor from a gateway response.
    /// </summary>
    /// <param name="response">The gateway response.</param>
    /// <param name="currentTime">The current time.</param>
    /// <returns>A new station monitor instance.</returns>
    public static StationMonitor CreateStationFromGatewayResponse(TaskGatewayResponse response, DateTime currentTime)
    {
        var newStation = new StationMonitor
        {
            MachineId = response.MachineId,
            TimeStamp = currentTime,
            Name = response.Name,
            CycleStatus = response.CycleStatus,
            ResultValidation = response.ResultValidation,
            Label = response.Label,
            PartNumber = response.PartNumber,
            CyclesOk = response.CyclesOk,
            NextMachineId = response.NextMachineId,
            LastMachineId = response.LastMachineId,
            EventStatus = "Configuring",
            StatusRequest = "Configuring",
        };
        return newStation;
    }

    /// <summary>
    /// Creates a new station monitor from a gateway request.
    /// </summary>
    /// <param name="request">The gateway request.</param>
    /// <param name="currentTime">The current time.</param>
    /// <returns>A new station monitor instance.</returns>
    public static StationMonitor CreateStationFromGatewayRequest(TaskGatewayRequest request, DateTime currentTime)
    {
        var newStation = new StationMonitor
        {
            MachineId = request.MachineId,
            Name = request.Name,
            TimeStamp = currentTime,
            CycleStatus = request.CycleStatus,
            PartStatus = request.PartStatus,
            PartNumber = request.PartNumber,
            GatewayTask = request.GatewayTask,
            Label = request.BarCode,
            ResultValidation = ResultValidation.None,
            FlowStatus = FlowStatus.None,
            StatusRequest = "Requesting...",
        };

        return newStation;
    }

    /// <summary>
    /// Updates the station monitor from a gateway request.
    /// </summary>
    /// <param name="request">The gateway request.</param>
    /// <param name="currentTime">The current time.</param>
    public void UpdateStationFromGatewayRequest(TaskGatewayRequest request, DateTime currentTime)
    {
        this.TimeStamp = currentTime;
        this.CycleStatus = request.CycleStatus;
        this.PartStatus = request.PartStatus;
        this.PartNumber = request.PartNumber;
        this.GatewayTask = request.GatewayTask;
        this.Label = request.BarCode;
        this.ResultValidation = ResultValidation.None;
        this.FlowStatus = FlowStatus.None;

        this.EventStatus = "Requested";
        this.StatusRequest = "Requested";
        this.RequestTask = request.RequestTask;

        this.EnsureIsValidToRenderAndPersist();
    }

    /// <summary>
    /// Updates the station monitor from a gateway response.
    /// </summary>
    /// <param name="response">The gateway response.</param>
    /// <param name="currentTime">The current time.</param>
    public void UpdateStationFromGatewayResponse(TaskGatewayResponse response, DateTime currentTime)
    {
        this.TimeStamp = currentTime;
        this.CycleStatus = response.CycleStatus;
        this.ResultValidation = response.ResultValidation;
        this.Name = response.Name;
        this.Label = response.Label;
        this.PartNumber = response.PartNumber;
        this.CyclesOk = response.CyclesOk;
        this.NextMachineId = response.NextMachineId;
        this.LastMachineId = response.LastMachineId;
        this.Error = response.Error;
        this.EventStatus = "Completed";
        this.StatusRequest = "Completed";
        this.Description = response.Description;

        this.EnsureIsValidToRenderAndPersist();
    }

    /// <summary>
    /// Converts a TaskGatewayResponse to the specified destination type.
    /// </summary>
    /// <typeparam name="TDest">The destination type.</typeparam>
    /// <param name="src">The source TaskGatewayResponse.</param>
    /// <param name="dest">The destination object.</param>
    /// <param name="dateTimeMachine">The date time machine to use for timestamp operations. Defaults to new DateTimeMachine() if null.</param>
    /// <returns>A Result containing the converted destination object or failure information.</returns>
    public static Result<TDest> ToDto<TDest>(TaskGatewayResponse src, TDest dest)
        where TDest : new()
    {
        if (src == null)
        {
            return Result<TDest>.WithFailure($"Parameter '{nameof(src)}' cannot be null");
        }

        if (dest == null)
        {
            dest = new TDest();
        }

        if (dest is StationMonitor monitor)
        {
            monitor.MachineId = src.MachineId;
            monitor.Name = src.Name;
            monitor.Label = src.Label;
            monitor.PartNumber = src.PartNumber;
            monitor.Error = src.Error;
            monitor.Description = src.Description;
            monitor.ResultValidation = src.ResultValidation; // direct assignment
            monitor.CycleStatus = src.CycleStatus; // direct assignment
            monitor.FlowStatus = src.FlowStatus; // direct assignment
            monitor.PartStatus = src.PartStatus; // direct assignment
            monitor.MachineType = src.MachineType;
            monitor.WorkFlowType = src.WorkFlowType;
            monitor.CyclesOk = src.CyclesOk;
            monitor.NextMachineId = src.NextMachineId;
            monitor.LastMachineId = src.LastMachineId;
            monitor.TimeStamp = src.TimeStamp;
            monitor.RequestTask = src.RequestTask;

            // Map other properties as needed
            return Result<TDest>.Success((TDest)(object)monitor);
        }

        return Result<TDest>.Success(dest);
    }

    /// <summary>
    /// Converts the source object to a TaskGatewayResponse entity.
    /// </summary>
    /// <typeparam name="T">The source type.</typeparam>
    /// <param name="src">The source object.</param>
    /// <param name="dest">The destination TaskGatewayResponse.</param>
    /// <returns>The converted TaskGatewayResponse entity.</returns>
    /// <summary>
    /// Converts the source object to a TaskGatewayResponse entity.
    /// </summary>
    /// <typeparam name="T">The source type.</typeparam>
    /// <param name="src">The source object.</param>
    /// <param name="dest">The destination TaskGatewayResponse.</param>
    /// <returns>A Result containing the converted TaskGatewayResponse entity or failure information.</returns>
    public static Result<TaskGatewayResponse> ToEntity<T>(T src, TaskGatewayResponse dest)
    {
        if (src == null)
        {
            return Result<TaskGatewayResponse>.WithFailure($"Parameter '{nameof(src)}' cannot be null");
        }

        if (dest == null)
        {
            dest = new TaskGatewayResponse();
        }

        if (src is StationMonitor monitor)
        {
            dest.MachineId = monitor.MachineId;
            dest.Name = monitor.Name;
            dest.Label = monitor.Label;
            dest.PartNumber = monitor.PartNumber;
            dest.Error = monitor.Error;
            dest.Description = monitor.Description;
            dest.ResultValidation = monitor.ResultValidation; // direct assignment
            dest.CycleStatus = monitor.CycleStatus; // direct assignment
            dest.FlowStatus = monitor.FlowStatus; // direct assignment
            dest.PartStatus = monitor.PartStatus; // direct assignment
            dest.MachineType = monitor.MachineType;
            dest.WorkFlowType = monitor.WorkFlowType;
            dest.CyclesOk = monitor.CyclesOk;
            dest.NextMachineId = monitor.NextMachineId;
            dest.LastMachineId = monitor.LastMachineId;
            dest.TimeStamp = monitor.TimeStamp;
            dest.RequestTask = monitor.RequestTask;

            // Map other properties as needed
            return Result<TaskGatewayResponse>.Success(dest);
        }

        return Result<TaskGatewayResponse>.Success(dest);
    }

    /// <summary>
    /// Converts a TaskGatewayRequest to the specified destination type.
    /// </summary>
    /// <typeparam name="TDest">The destination type.</typeparam>
    /// <param name="src">The source TaskGatewayRequest.</param>
    /// <param name="dest">The destination object.</param>
    /// <returns>The converted destination object.</returns>
    /// <summary>
    /// Converts a TaskGatewayRequest to the specified destination type.
    /// </summary>
    /// <typeparam name="TDest">The destination type.</typeparam>
    /// <param name="src">The source TaskGatewayRequest.</param>
    /// <param name="dest">The destination object.</param>
    /// <param name="dateTimeMachine">The date time machine to use for timestamp operations. Defaults to new DateTimeMachine() if null.</param>
    /// <returns>A Result containing the converted destination object or failure information.</returns>
    public static Result<TDest> ToDto<TDest>(TaskGatewayRequest src, TDest dest, IDateTimeMachine? dateTimeMachine = null)
        where TDest : new()
    {
        if (src == null)
        {
            return Result<TDest>.WithFailure($"Parameter '{nameof(src)}' cannot be null");
        }

        if (dest == null)
        {
            dest = new TDest();
        }

        var dtm = dateTimeMachine ?? new DateTimeMachine();

        if (dest is StationMonitor monitor)
        {
            monitor.MachineId = src.MachineId;
            monitor.Name = src.Name;
            monitor.Label = src.BarCode;
            monitor.PartNumber = src.PartNumber;
            monitor.CycleStatus = src.CycleStatus; // direct assignment
            monitor.PartStatus = src.PartStatus; // direct assignment
            monitor.GatewayTask = src.GatewayTask;
            monitor.RequestTask = src.RequestTask;
            monitor.TimeStamp = dtm.Now;
            monitor.StatusRequest = "Requesting...";

            // Map other properties as needed
            return Result<TDest>.Success((TDest)(object)monitor);
        }

        return Result<TDest>.Success(dest);
    }

    /// <summary>
    /// Converts the source object to a TaskGatewayRequest entity.
    /// </summary>
    /// <typeparam name="T">The source type.</typeparam>
    /// <param name="src">The source object.</param>
    /// <param name="dest">The destination TaskGatewayRequest.</param>
    /// <returns>The converted TaskGatewayRequest entity.</returns>
    /// <summary>
    /// Converts the source object to a TaskGatewayRequest entity.
    /// </summary>
    /// <typeparam name="T">The source type.</typeparam>
    /// <param name="src">The source object.</param>
    /// <param name="dest">The destination TaskGatewayRequest.</param>
    /// <returns>A Result containing the converted TaskGatewayRequest entity or failure information.</returns>

    /// <summary>
    /// Maps properties from TaskGatewayRequest to StationMonitor.
    /// </summary>

    /// <summary>
    /// Maps properties from StationMonitor to TaskGatewayRequest.
    /// </summary>
    public static Result<TaskGatewayRequest> ToEntity<T>(T src, TaskGatewayRequest dest)
    {
        if (src == null)
        {
            return Result<TaskGatewayRequest>.WithFailure($"Parameter '{nameof(src)}' cannot be null");
        }

        if (dest == null)
        {
            dest = new TaskGatewayRequest();
        }

        if (src is StationMonitor monitor)
        {
            dest.MachineId = monitor.MachineId;
            dest.Name = monitor.Name;
            dest.BarCode = monitor.Label;
            dest.PartNumber = monitor.PartNumber;
            dest.CycleStatus = monitor.CycleStatus;
            dest.PartStatus = monitor.PartStatus;
            dest.GatewayTask = monitor.GatewayTask;
            dest.RequestTask = monitor.RequestTask;

            // Map other properties as needed
            return Result<TaskGatewayRequest>.Success(dest);
        }

        return Result<TaskGatewayRequest>.Success(dest);
    }
}
