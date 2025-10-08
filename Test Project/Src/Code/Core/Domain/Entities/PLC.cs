// <copyright file="PLC.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a PLC (Programmable Logic Controller) entity, including network and configuration details.
/// </summary>
public class Plc : IEntityRoot
{
    /// <summary>
    /// Gets or sets the unique identifier for the PLC.
    /// </summary>
    public int PlcId { get; set; }

    /// <summary>
    /// Gets or sets the machine identifier associated with the PLC.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the PLC is enabled.
    /// </summary>
    public int Enabled { get; set; }

    /// <summary>
    /// Gets or sets the name of the PLC.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the IP address of the PLC.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the PLC.
    /// </summary>
    public string PlcType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the brand of the PLC.
    /// </summary>
    public string PlcBrand { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional options for the PLC.
    /// </summary>
    public string Options { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the communication library used by the PLC.
    /// </summary>
    public string CommLibrary { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the brand owner of the PLC.
    /// </summary>
    public string BrandOwner { get; set; } = string.Empty;

    /// <summary>
    /// Returns a string representation of the PLC.
    /// </summary>
    /// <returns>A string containing the PLC ID, name, and IP address.</returns>
    // [Fix]
    // CLAUDE
    // Date: 23/08/2025
    // Reason: Added ToString() implementation for better debugging and logging experience
    public override string ToString() => $"PLC {this.PlcId}: {this.Name} @ {this.IpAddress}";
}
