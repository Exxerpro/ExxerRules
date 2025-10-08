// <copyright file="PlcSiemensOptions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

/// <summary>
/// Represents Siemens-specific PLC options, such as rack, slot, and TSAP.
/// </summary>
public class PlcSiemensOptions
{
    /// <summary>
    /// Gets or sets the rack number for the Siemens PLC.
    /// </summary>
    public int Rack { get; set; }

    /// <summary>
    /// Gets or sets the slot number for the Siemens PLC.
    /// </summary>
    public int Slot { get; set; }

    /// <summary>
    /// Gets or sets the TSAP (Transport Service Access Point) for the Siemens PLC.
    /// </summary>
    public string Tsap { get; set; } = string.Empty;
}
