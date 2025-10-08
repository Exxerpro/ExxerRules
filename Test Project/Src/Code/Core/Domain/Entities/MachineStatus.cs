// <copyright file="MachineStatus.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents the status of a machine, including breakdown time and last update timestamp.
/// </summary>
public class MachineStatus : ILookupEntity
{
    /// <summary>
    /// Gets or sets the machine identifier.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the status identifier for the machine.
    /// </summary>
    public int StatusMachineId { get; set; }

    /// <summary>
    /// Gets or sets the breakdown time for the machine.
    /// </summary>
    public decimal BreakDownTime { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the status was last updated.
    /// </summary>
    public DateTime UpdatedOn { get; set; }
}
