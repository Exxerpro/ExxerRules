// <copyright file="TaskGatewayResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Entities.BarCodes;
using IndTrace.Domain.Enum;
using IndTrace.Domain.Interfaces;
using IndTrace.Domain.Models;

/// <summary>
/// Represents the response for a gateway task, including machine, barcode, cycle, and status information.
/// </summary>
public class TaskGatewayResponse : IMonitorFilter, IEntityRoot
{
    private readonly IDateTimeMachine dateTimeMachine;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskGatewayResponse"/> class with optional date time machine dependency.
    /// </summary>
    /// <param name="dateTimeMachine">The date time machine to use for timestamp operations. Defaults to new DateTimeMachine() if null.</param>
    public TaskGatewayResponse(IDateTimeMachine? dateTimeMachine = null)
    {
        this.dateTimeMachine = dateTimeMachine ?? new DateTimeMachine();

        this.Name = string.Empty;
        this.RequestTask = string.Empty;
        this.Parameters = new Dictionary<string, string>();

        // Initialize with default date 2020-01-01 when no dependency provided
        this.TimeStamp = this.dateTimeMachine.Now;
    }

    /// <summary>
    /// Converts the source barcode result to a TaskGatewayResponse.
    /// </summary>
    /// <param name="src">The source barcode result.</param>
    /// <returns>The converted TaskGatewayResponse.</returns>
    public static TaskGatewayResponse ToDto(IBarCodeResult src)
    {
        ArgumentNullException.ThrowIfNull(src);
        return new TaskGatewayResponse
        {
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            CycleId = src.CycleId,
            CyclesOk = src.CyclesOk,
            ShiftId = src.ShiftId,
            CommandId = src.CommandId,
            ResultValidation = src.ResultValidation,
            Error = src.Error ?? string.Empty,
            Label = src.Label ?? string.Empty,
            PartNumber = src.PartNumber ?? string.Empty,
            Description = src.Description ?? string.Empty,
            LastMachineId = src.LastMachineId,
            NextMachineId = src.NextMachineId,
            CycleStatus = src.CycleStatus,
            FlowStatus = src.FlowStatus,
            PartStatus = src.PartStatus,
            MachineType = src.MachineType,
            WorkFlowType = src.WorkFlowType,
            Recipe = src.Recipe,
            Cycle = src.Cycle,
            BarCode = src.BarCode,
            MasterLabel = src.MasterLabel,
            References = src.References ?? new Dictionary<string, Register>(),
        };
    }

    /// <summary>
    /// Converts the source TaskGatewayRequest to a TaskGatewayResponse.
    /// </summary>
    /// <param name="src">The source TaskGatewayRequest.</param>
    /// <returns>The converted TaskGatewayResponse.</returns>
    public static TaskGatewayResponse ToDto(TaskGatewayRequest src)
    {
        ArgumentNullException.ThrowIfNull(src);
        return new TaskGatewayResponse
        {
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            CycleId = src.CycleId,
            CommandId = src.CommandId,
            PartNumber = src.PartNumber,
            Description = src.Description,
            CycleStatus = src.CycleStatus,
            FlowStatus = src.FlowStatus,
            PartStatus = src.PartStatus,
            MachineType = src.MachineType,
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskGatewayResponse"/> class.
    /// Private parameterless constructor for EF Core entity materialization.
    /// </summary>
    private TaskGatewayResponse()
    {
        this.dateTimeMachine = new DateTimeMachine(); // Default implementation for EF Core

        this.Name = string.Empty;
        this.RequestTask = string.Empty;
        this.Parameters = new Dictionary<string, string>();

        // Initialize with default date when no dependency provided
        this.TimeStamp = this.dateTimeMachine.Now;
    }

    /// <summary>
    /// Gets or sets the execution time for the task.
    /// </summary>
    public TimeSpan ExecutionTime { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the response.
    /// </summary>
    public int ResponseId { get; set; }

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
    /// Gets or sets the command identifier.
    /// </summary>
    public int CommandId { get; set; }

    /// <summary>
    /// Gets or sets the name of the response.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the part number associated with the response.
    /// </summary>
    public string PartNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the response.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the label for the barcode.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message for the response.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last machine identifier.
    /// </summary>
    public int LastMachineId { get; set; }

    /// <summary>
    /// Gets or sets the next machine identifier.
    /// </summary>
    public int NextMachineId { get; set; }

    /// <summary>
    /// Gets or sets the cycle status for the response.
    /// </summary>
    public CycleStatus CycleStatus { get; set; } = CycleStatus.None;

    /// <summary>
    /// Gets or sets the machine type for the response.
    /// </summary>
    public MachineType MachineType { get; set; } = MachineType.None;

    /// <summary>
    /// Gets or sets the part status for the response.
    /// </summary>
    public PartStatus PartStatus { get; set; } = PartStatus.None;

    /// <summary>
    /// Gets or sets the flow status for the response.
    /// </summary>
    public FlowStatus FlowStatus { get; set; } = FlowStatus.None;

    /// <summary>
    /// Gets or sets the result validation status for the response.
    /// </summary>
    public ResultValidation ResultValidation { get; set; } = ResultValidation.None;

    /// <summary>
    /// Gets or sets the request task name.
    /// </summary>
    public string RequestTask { get; set; }

    /// <summary>
    /// Gets or sets the workflow type for the response.
    /// </summary>
    public WorkFlowType WorkFlowType { get; set; } = WorkFlowType.None;

    /// <summary>
    /// Gets or sets the recipe associated with the response.
    /// </summary>
    public Recipe Recipe { get; set; } = new();

    /// <summary>
    /// Gets or sets the cycle associated with the response.
    /// </summary>
    public Cycle Cycle { get; set; } = new();

    /// <summary>
    /// Gets or sets the barcode entity associated with the response.
    /// </summary>
    public BarCode BarCode { get; set; } = new();

    /// <summary>
    /// Gets or sets the master label entity associated with the response.
    /// </summary>
    public MasterLabel MasterLabel { get; set; } = new();

    /// <summary>
    /// Gets or sets the timestamp for the response.
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the dictionary of references associated with the response.
    /// </summary>
    public IDictionary<string, Register> References { get; set; } = new Dictionary<string, Register>();

    /// <summary>
    /// Gets or sets the dictionary of parameters for the response.
    /// </summary>
    public IDictionary<string, string> Parameters { get; set; }

    /// <summary>
    /// Ensures the response is valid for rendering and persisting.
    /// </summary>
    /// <returns>True if valid; otherwise, false.</returns>
    public bool EnsureIsValidToRenderAndPersist()
    {
        this.FlowStatus ??= FlowStatus.None;
        this.CycleStatus ??= CycleStatus.None;
        this.ResultValidation ??= ResultValidation.None;
        this.PartStatus ??= PartStatus.None;
        this.MachineType ??= MachineType.None;
        this.WorkFlowType ??= WorkFlowType.None;
        return true;
    }

    /// <summary>
    /// Sets the machine identifier.
    /// </summary>
    /// <param name="machineId">The machine identifier.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithMachineId(int machineId)
    {
        this.MachineId = machineId;
        return this;
    }

    /// <summary>
    /// Sets the barcode identifier.
    /// </summary>
    /// <param name="barCodeId">The barcode identifier.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithBarCodeId(int barCodeId)
    {
        this.BarCodeId = barCodeId;
        return this;
    }

    /// <summary>
    /// Sets the cycle identifier.
    /// </summary>
    /// <param name="cycleId">The cycle identifier.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithCycleId(int cycleId)
    {
        this.CycleId = cycleId;
        return this;
    }

    /// <summary>
    /// Sets the number of successful cycles.
    /// </summary>
    /// <param name="cyclesOk">The number of successful cycles.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithCyclesOk(int cyclesOk)
    {
        this.CyclesOk = cyclesOk;
        return this;
    }

    /// <summary>
    /// Sets the result validation status.
    /// </summary>
    /// <param name="resultValidation">The result validation status.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithResultValidation(ResultValidation resultValidation)
    {
        this.ResultValidation = resultValidation;
        return this;
    }

    /// <summary>
    /// Sets the part number.
    /// </summary>
    /// <param name="partNumber">The part number.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithPartNumber(string partNumber)
    {
        this.PartNumber = partNumber;
        return this;
    }

    /// <summary>
    /// Sets the last machine identifier.
    /// </summary>
    /// <param name="lastMachineId">The last machine identifier.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithLastMachineId(int lastMachineId)
    {
        this.LastMachineId = lastMachineId;
        return this;
    }

    /// <summary>
    /// Sets the next machine identifier.
    /// </summary>
    /// <param name="nextMachineId">The next machine identifier.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithNextMachineId(int nextMachineId)
    {
        this.NextMachineId = nextMachineId;
        return this;
    }

    /// <summary>
    /// Sets the cycle status.
    /// </summary>
    /// <param name="cycleStatus">The cycle status.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithCycleStatus(CycleStatus cycleStatus)
    {
        this.CycleStatus = cycleStatus;
        return this;
    }

    /// <summary>
    /// Sets the flow status.
    /// </summary>
    /// <param name="flowStatus">The flow status.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithFlowStatus(FlowStatus flowStatus)
    {
        this.FlowStatus = flowStatus;
        return this;
    }

    /// <summary>
    /// Sets the part status.
    /// </summary>
    /// <param name="partStatus">The part status.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithPartStatus(PartStatus partStatus)
    {
        this.PartStatus = partStatus;
        return this;
    }

    /// <summary>
    /// Sets the machine type.
    /// </summary>
    /// <param name="machineType">The machine type.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithMachineType(MachineType machineType)
    {
        this.MachineType = machineType;
        return this;
    }

    /// <summary>
    /// Sets the workflow type.
    /// </summary>
    /// <param name="workFlowType">The workflow type.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithWorkFlowType(WorkFlowType workFlowType)
    {
        this.WorkFlowType = workFlowType;
        return this;
    }

    /// <summary>
    /// Sets the recipe.
    /// </summary>
    /// <param name="recipe">The recipe.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithRecipe(Recipe recipe)
    {
        this.Recipe = recipe;
        return this;
    }

    /// <summary>
    /// Sets the cycle.
    /// </summary>
    /// <param name="cycle">The cycle.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithCycle(Cycle cycle)
    {
        this.Cycle = cycle;
        return this;
    }

    /// <summary>
    /// Sets the barcode entity.
    /// </summary>
    /// <param name="barCode">The barcode entity.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithBarCode(BarCode barCode)
    {
        this.BarCode = barCode;
        this.Label = barCode.Label ?? string.Empty;
        return this;
    }

    /// <summary>
    /// Sets the master label entity.
    /// </summary>
    /// <param name="masterLabel">The master label entity.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithMasterLabel(MasterLabel masterLabel)
    {
        this.MasterLabel = masterLabel;
        return this;
    }

    /// <summary>
    /// Sets the name of the response.
    /// </summary>
    /// <param name="name">The name of the response.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithName(string name)
    {
        this.Name = name;
        return this;
    }

    /// <summary>
    /// Sets the description of the response.
    /// </summary>
    /// <param name="description">The description of the response.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithDescription(string description)
    {
        this.Description = description;
        return this;
    }

    /// <summary>
    /// Sets the references dictionary.
    /// </summary>
    /// <param name="references">The references dictionary.</param>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse WithReferences(IDictionary<string, Register> references)
    {
        this.References = references;
        return this;
    }

    /// <summary>
    /// Applies the values from the references dictionary to the response (legacy API).
    /// Uses functional Result pattern internally and avoids throwing.
    /// </summary>
    /// <returns>The updated response.</returns>
    public TaskGatewayResponse ApplyReferencesValues()
    {
        _ = this.ApplyReferencesValuesResult();
        return this;
    }

    /// <summary>
    /// Applies the values from the references dictionary to the response using the functional Result pattern.
    /// </summary>
    /// <returns>A <see cref="Result"/> indicating success or containing validation errors.</returns>
    public Result ApplyReferencesValuesResult()
    {
        if (this.References is null)
        {
            return Result.WithFailure("references can't be null");
        }

        if (this.References.Count == 0)
        {
            return Result.WithFailure("references can't be empty");
        }

        var referenceValues = new Dictionary<string, string>
        {
            { nameof(this.LastMachineId), this.LastMachineId.ToString() },
            { nameof(this.NextMachineId), this.NextMachineId.ToString() },
            { nameof(this.CycleStatus), this.CycleStatus?.Value.ToString() ?? "0" },
            { nameof(this.FlowStatus), this.FlowStatus?.Value.ToString() ?? "0" },
            { nameof(this.PartStatus), this.PartStatus?.Value.ToString() ?? "0" },
            { nameof(this.MachineType), this.MachineType?.Value.ToString() ?? "0" },
            { nameof(this.WorkFlowType), this.WorkFlowType?.Value.ToString() ?? "0" },
            { nameof(this.BarCodeId), this.BarCodeId.ToString() },
            { nameof(this.CycleId), this.CycleId.ToString() },
            { nameof(this.Label), this.Label ?? string.Empty },
            { nameof(this.CyclesOk), this.CyclesOk.ToString() },
            { nameof(this.ShiftId), this.ShiftId.ToString() },
            { nameof(this.ResultValidation), this.ResultValidation?.Value.ToString() ?? "0" },
        };

        foreach (var keyValue in referenceValues)
        {
            if (this.References.TryGetValue(keyValue.Key, out var reference))
            {
                reference.Value = keyValue.Value;
            }
        }

        return Result.Success();
    }

    /// <summary>
    /// Maps the properties from the source barcode result to the response.
    /// </summary>
    /// <param name="source">The source barcode result.</param>
    public void MapFrom(IBarCodeResult source)
    {
        this.MachineId = source.MachineId;
        this.BarCodeId = source.BarCodeId;
        this.CycleId = source.CycleId;
        this.CyclesOk = source.CyclesOk;
        this.ResultValidation = source.ResultValidation;
        this.PartNumber = source.PartNumber ?? string.Empty;
        this.Label = source.Label ?? string.Empty;
        this.LastMachineId = source.LastMachineId;
        this.NextMachineId = source.NextMachineId;
        this.CycleStatus = source.CycleStatus;
        this.FlowStatus = source.FlowStatus;
        this.PartStatus = source.PartStatus;
        this.MachineType = source.MachineType;
        this.WorkFlowType = source.WorkFlowType;
        this.Recipe = source.Recipe;
        this.Cycle = source.Cycle;
        this.BarCode = source.BarCode;
        this.MasterLabel = source.MasterLabel;
        this.ShiftId = source.ShiftId;
        this.References = source.References;
        this.Description = source.Description;
    }

    /// <summary>
    /// Returns a string representation of the response.
    /// </summary>
    /// <returns>A string representation of the response.</returns>
    public override string ToString()
    {
        string newline = Environment.NewLine;
        return $"BarCode: {this.Label ?? "N/A"}{newline}" +
               $"Result: {this.ResultValidation?.DisplayName ?? "N/A"}{newline}" +
               $"Machine ID: {this.MachineId}{newline}" +
               $"Machine Name: {this.Name}{newline}" +
               $"BarCode ID: {this.BarCodeId}{newline}" +
               $"Cycle ID: {this.CycleId}{newline}" +
               $"Cycles OK: {this.CyclesOk}{newline}" +
               $"Last Machine ID: {this.LastMachineId}{newline}" +
               $"Next Machine ID: {this.NextMachineId}{newline}" +
               $"Cycle Status: {this.CycleStatus?.DisplayName ?? "N/A"}{newline}" +
               $"Flow Status: {this.FlowStatus?.DisplayName ?? "N/A"}{newline}" +
               $"Part Status: {this.PartStatus?.DisplayName ?? "N/A"}{newline}" +
               $"Machine Type: {this.MachineType?.DisplayName ?? "N/A"}{newline}" +
               $"WorkFlow Type: {this.WorkFlowType?.DisplayName ?? "N/A"}{newline}" +
               $"Description: {this.Description?.ToString() ?? "N/A"}{newline}" +
               $"Execution Time: {this.ExecutionTime}{newline}";
    }
}
