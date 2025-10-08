// <copyright file="DataFromPlc.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Enum;

/// <summary>
/// Represents data received from a PLC, including machine, barcode, part, and status information.
/// </summary>
public class DataFromPlc
{
    /// <summary>
    /// Gets or sets the machine identifier from which the data was received.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the command code received from the PLC.
    /// </summary>
    public int Command { get; set; }

    /// <summary>
    /// Gets or sets the barcode value received from the PLC.
    /// </summary>
    public string BarCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the part number received from the PLC.
    /// </summary>
    public string PartNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cycle status received from the PLC.
    /// </summary>
    public CycleStatus CycleStatus { get; set; } = CycleStatus.None;

    /// <summary>
    /// Gets or sets the part status received from the PLC.
    /// </summary>
    public PartStatus PartStatus { get; set; } = PartStatus.None;

    /// <summary>
    /// Gets or sets the watchdog timer status.
    /// </summary>
    public WatchDog WatchDogTime { get; set; } = WatchDog.Enable;
}
