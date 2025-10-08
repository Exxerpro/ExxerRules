// <copyright file="Cycle.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Enum;
using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a manufacturing production cycle in an industrial environment, tracking the complete lifecycle
/// of a production operation from initiation to completion, including status, timing metrics, and related
/// machine and barcode information for quality control and performance analysis.
/// </summary>
public class Cycle : IEntityRoot
{
    /// <summary>
    /// Gets or sets the unique identifier for the cycle.
    /// Used as the primary key in the data store and for referencing this cycle in other entities.
    /// </summary>
    public int CycleId { get; set; }

    /// <summary>
    /// Gets or sets the machine identifier associated with the cycle.
    /// References the specific manufacturing equipment (e.g., robot, assembly machine, CNC) that performed this cycle.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the barcode identifier associated with the cycle.
    /// References the product or part being manufactured or processed during this cycle for traceability.
    /// </summary>
    public int BarCodeId { get; set; }

    /// <summary>
    /// Gets or sets the status of the cycle.
    /// Corresponds to the <see cref="CycleStatus"/> enum values: None (0), NotStarted (1), Started (2),
    /// FinishedOk (4), FinishedNok (8), EndOfProcess (16), Rejected (32), Canceled (64), Invalid (-1).
    /// Represents the current state of the manufacturing cycle in the production workflow.
    /// </summary>
    public CycleStatus CycleStatus { get; set; } = CycleStatus.None;

    /// <summary>
    /// Gets or sets the count of successful cycles completed.
    /// Used for tracking productivity and performance metrics in a manufacturing context.
    /// </summary>
    public int CyclesOk { get; set; }

    /// <summary>
    /// Gets or sets the part status for the cycle.
    /// Corresponds to the <see cref="PartStatus"/> enum values: None (0), Ok (1), NOk (2), Restored (3), Invalid (-1).
    /// Indicates the quality status of the part produced during this cycle.
    /// </summary>
    public PartStatus PartStatus { get; set; } = PartStatus.None;

    /// <summary>
    /// Gets or sets the actual cycle time in seconds or milliseconds.
    /// Measures the time taken to complete this specific manufacturing operation,
    /// used for performance analysis and optimization.
    /// </summary>
    public int CycleTime { get; set; }

    /// <summary>
    /// Gets or sets the target takt time for the cycle.
    /// Takt time is the manufacturing term for the maximum time allowed to produce one product
    /// to meet customer demand. Used for comparing actual performance against expected standards.
    /// </summary>
    public int TaktTime { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the cycle was started.
    /// Used for cycle time calculations, production scheduling, and historical analysis.
    /// </summary>
    public DateTime StartedOn { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the cycle was finished.
    /// Used along with StartedOn to calculate actual production times and analyze performance.
    /// </summary>
    public DateTime FinishedOn { get; set; }

    /// <summary>
    /// Returns a string representation of the cycle.
    /// </summary>
    /// <returns>A string containing the cycle ID, machine ID, cycle status, and part status.</returns>
    // [Fix]
    // CLAUDE
    // Date: 23/08/2025
    // Reason: Added ToString() implementation for better debugging and logging experience
    public override string ToString() => $"Cycle {this.CycleId} (Machine {this.MachineId}): {this.CycleStatus}/{this.PartStatus}";
}
