// <copyright file="MachinePlc.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;
using IndTrace.Domain.Models;

/// <summary>
/// Represents the association between a machine and a PLC, including activation status.
/// </summary>
public class MachinePlc : AuditableEntity, IEntityRoot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MachinePlc"/> class.
    /// </summary>
    public MachinePlc()
    {
        this.IsActive = 1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MachinePlc"/> class with specified machine, PLC, and activation status.
    /// </summary>
    /// <param name="machineId">The machine identifier.</param>
    /// <param name="plcId">The PLC identifier.</param>
    /// <param name="isActive">The activation status.</param>
    public MachinePlc(int machineId, int plcId, int isActive)
    {
        this.MachineId = machineId;
        this.PlcId = plcId;
        this.IsActive = isActive;
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
    /// Gets or sets a value indicating whether the association is active.
    /// </summary>
    public int IsActive { get; set; }
}
