// <copyright file="Machine.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using IndTrace.Domain.Enum;
using IndTrace.Domain.Interfaces;
using IndTrace.Domain.Models;

/// <summary>
/// Represents a machine in the production system, including configuration, connectivity, and workflow information.
/// </summary>
public class Machine : IEntityRoot
{
    /// <summary>
    /// Gets or sets the unique identifier for the machine.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the name of the machine.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the machine.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the location of the machine.
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the machine type.
    /// </summary>
    public MachineType MachineType { get; set; } = MachineType.None;

    /// <summary>
    /// Gets or sets the workflow type.
    /// </summary>
    public WorkFlowType WorkFlowType { get; set; } = WorkFlowType.None;

    /// <summary>
    /// Gets or sets a value indicating whether application traceability is enabled.
    /// </summary>
    public int EnableAppTraceability { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether bypass traceability is enabled.
    /// </summary>
    public int EnableBypassTraceability { get; set; }

    /// <summary>
    /// Gets or sets the retry count for the machine.
    /// </summary>
    public int Retry { get; set; }

    /// <summary>
    /// Gets or sets the collection of outgoing edges from this machine.
    /// </summary>
    public ICollection<Edge> FromEdges { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of incoming edges to this machine.
    /// </summary>
    public ICollection<Edge> ToEdges { get; set; } = [];

    /// <summary>
    /// Gets or sets the rule identifier associated with the machine.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the machine is enabled for traceability.
    /// </summary>
    public bool IsEnabled => this.EnableAppTraceability == 1
                             || this.EnableBypassTraceability != 1;

    /// <summary>
    /// Gets the result of the last operation performed on the machine.
    /// </summary>
    public Result Result { get; private set; } = new();

    /// <summary>
    /// Enables the machine for application traceability and disables bypass traceability.
    /// </summary>
    /// <returns>A <see cref="Result"/> indicating the outcome.</returns>
    public Result Enable()
    {
        this.EnableAppTraceability = 1;
        this.EnableBypassTraceability = 0;
        return Result.Success();
    }

    /// <summary>
    /// Disables the machine for application traceability and enables bypass traceability.
    /// </summary>
    /// <returns>A <see cref="Result"/> indicating the outcome.</returns>
    public Result Disable()
    {
        this.EnableAppTraceability = 0;
        this.EnableBypassTraceability = 1;
        return Result.Success();
    }

    /// <summary>
    /// Returns a string representation of the machine.
    /// </summary>
    /// <returns>A string containing the machine ID, name, and location.</returns>
    // [Fix]
    // CLAUDE
    // Date: 23/08/2025
    // Reason: Added ToString() implementation for better debugging and logging experience
    public override string ToString()
    {
        return $"Machine[" +
               $"Id={this.MachineId}, " +
               $"Name='{this.Name}', " +
               $"Location='{this.Location}', " +
               $"Description='{this.Description}', " +
               $"Type={this.MachineType}, " +
               $"Workflow={this.WorkFlowType}, " +
               $"IsEnabled={this.IsEnabled}, " +
               $"RuleId={this.RuleId}, " +
               $"Retry={this.Retry}, " +
               $"FromEdges={this.FromEdges?.Count ?? 0}, " +
               $"ToEdges={this.ToEdges?.Count ?? 0}, " +
               $"Result={this.Result}" +
               "]";
    }
}
