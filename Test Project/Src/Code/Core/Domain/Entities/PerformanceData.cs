// <copyright file="PerformanceData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;
using IndTrace.Domain.Models;

/// <summary>
/// Represents performance data collected from a machine and PLC, including production metrics and timing information.
/// </summary>
public class PerformanceData : IEntityRoot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceData"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public PerformanceData()
    {
        this.Command = new TaskGatewayRequest();
    }

    /// <summary>
    /// Creates a <see cref="PerformanceData"/> instance from a dictionary of register values.
    /// </summary>
    /// <param name="perf">The dictionary containing register values.</param>
    /// <returns>A new <see cref="PerformanceData"/> instance populated from the dictionary.</returns>

    public static PerformanceData FromPlc(IDictionary<string, Register> perf)
    {
        var result = new PerformanceData
        {
            ApplicationFlag = ParseInt(perf, nameof(ApplicationFlag)),
            EventCounter = ParseInt(perf, nameof(EventCounter)),
            CurrentTime = ParseInt(perf, nameof(CurrentTime)),
            RunningTime = ParseInt(perf, nameof(RunningTime)),
            StoppedTime = ParseInt(perf, nameof(StoppedTime)),
            FaultedTime = ParseInt(perf, nameof(FaultedTime)),
            StatusFaultReason = ParseInt(perf, nameof(StatusFaultReason)),
            TotalProduction = ParseDouble(perf, nameof(TotalProduction)),
            ProductionOk = ParseDouble(perf, nameof(ProductionOk)),
            ProductionNoK = ParseDouble(perf, nameof(ProductionNoK)),
            StatusFaultReject = ParseInt(perf, nameof(StatusFaultReject)),
            RejectEventCounter = ParseInt(perf, nameof(RejectEventCounter)),
            StatusReject = ParseInt(perf, nameof(StatusReject)),
            RejectQuantityUnits = ParseDouble(perf, nameof(RejectQuantityUnits)),
            StandardCycleTime = ParseDouble(perf, nameof(StandardCycleTime)),
            ActualCycleTime = ParseDouble(perf, nameof(ActualCycleTime)),
            PlanedProductionTime = ParseDouble(perf, nameof(PlanedProductionTime)),
        };

        return result;
    }

    // TODO [DRY][CURSOR][20/JUNE/2025] - Repeated ParseInt/ParseDouble pattern in PerformanceData factory. Consider refactoring to reduce repetition and improve maintainability.

    /// <summary>
    /// Gets or sets the unique identifier for the performance data entry.
    /// </summary>
    public long PerformanceDataId { get; set; }

    /// <summary>
    /// Gets or sets the machine identifier associated with the performance data.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the PLC identifier associated with the performance data.
    /// </summary>
    public int PlcId { get; set; }

    /// <summary>
    /// Gets or sets the barcode identifier associated with the performance data.
    /// </summary>
    public int BarCodeId { get; set; }

    /// <summary>
    /// Gets or sets the cycle identifier associated with the performance data.
    /// </summary>
    public int CycleId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp for the performance data entry.
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the application flag from the PLC.
    /// </summary>
    public int ApplicationFlag { get; set; }

    /// <summary>
    /// Gets or sets the event counter from the PLC.
    /// </summary>
    public int EventCounter { get; set; }

    /// <summary>
    /// Gets or sets the current time from the PLC.
    /// </summary>
    public int CurrentTime { get; set; }

    /// <summary>
    /// Gets or sets the running time from the PLC.
    /// </summary>
    public int RunningTime { get; set; }

    /// <summary>
    /// Gets or sets the stopped time from the PLC.
    /// </summary>
    public int StoppedTime { get; set; }

    /// <summary>
    /// Gets or sets the faulted time from the PLC.
    /// </summary>
    public int FaultedTime { get; set; }

    /// <summary>
    /// Gets or sets the status fault reason from the PLC.
    /// </summary>
    public int StatusFaultReason { get; set; }

    /// <summary>
    /// Gets or sets the total production value.
    /// </summary>
    public double TotalProduction { get; set; }

    /// <summary>
    /// Gets or sets the quantity of OK production units.
    /// </summary>
    public double ProductionOk { get; set; }

    /// <summary>
    /// Gets or sets the quantity of NOK production units.
    /// </summary>
    public double ProductionNoK { get; set; }

    /// <summary>
    /// Gets or sets the status fault reject value.
    /// </summary>
    public int StatusFaultReject { get; set; }

    /// <summary>
    /// Gets or sets the reject event counter.
    /// </summary>
    public int RejectEventCounter { get; set; }

    /// <summary>
    /// Gets or sets the status reject value.
    /// </summary>
    public int StatusReject { get; set; }

    /// <summary>
    /// Gets or sets the quantity of rejected units.
    /// </summary>
    public double RejectQuantityUnits { get; set; }

    /// <summary>
    /// Gets or sets the standard cycle time.
    /// </summary>
    public double StandardCycleTime { get; set; }

    /// <summary>
    /// Gets or sets the actual cycle time.
    /// </summary>
    public double ActualCycleTime { get; set; }

    /// <summary>
    /// Gets or sets the planned production time.
    /// </summary>
    public double PlanedProductionTime { get; set; }

    /// <summary>
    /// Gets or sets the command associated with the performance data.
    /// </summary>
    public TaskGatewayRequest Command { get; set; }

    /// <summary>
    /// Returns a string representation of the PerformanceData.
    /// </summary>
    /// <returns>A string containing the performance data ID, machine ID, and total production units.</returns>
    // [Fix]
    // CLAUDE
    // Date: 23/08/2025
    // Reason: Added ToString() implementation for better debugging and logging experience
    public override string ToString() => $"Performance {this.PerformanceDataId} (Machine {this.MachineId}): {this.TotalProduction} units";

    /// <summary>
    /// Updates the performance data fields from a TaskGatewayResponse result.
    /// </summary>
    /// <param name="result">The result containing TaskGatewayResponse data.</param>
    /// <returns>The updated <see cref="PerformanceData"/> instance.</returns>
    public PerformanceData FromResult(Result<TaskGatewayResponse> result)
    {
        if (result.Value is not null)
        {
            this.MachineId = result.Value.MachineId;
            this.PlcId = result.Value.PlcId;
            this.BarCodeId = result.Value.BarCodeId;
            this.CycleId = result.Value.CycleId;
            this.TimeStamp = result.Value.TimeStamp;
        }

        return this;
    }

    /// <summary>
    /// Parses an integer value from a dictionary of registers.
    /// </summary>
    /// <param name="dict">The dictionary containing register values.</param>
    /// <param name="key">The key to look up.</param>
    /// <returns>The parsed integer value, or 0 if parsing fails.</returns>
    private static int ParseInt(IDictionary<string, Register> dict, string key) =>
        dict.TryGetValue(key, out var reg) && reg?.Value is not null && int.TryParse(reg.Value, out var val) ? val : 0;

    /// <summary>
    /// Parses a double value from a dictionary of registers.
    /// </summary>
    /// <param name="dict">The dictionary containing register values.</param>
    /// <param name="key">The key to look up.</param>
    /// <returns>The parsed double value, or 0.0 if parsing fails.</returns>
    private static double ParseDouble(IDictionary<string, Register> dict, string key) =>
        dict.TryGetValue(key, out var reg) && reg?.Value is not null && double.TryParse(reg.Value, out var val) ? val : 0.0;
}
