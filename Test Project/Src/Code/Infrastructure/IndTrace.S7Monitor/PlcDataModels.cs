// <copyright file="PlcDataModels.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.S7Monitor;

/// <summary>
/// Represents a PLC device with an ID, IP address, and a list of associated tags.
/// </summary>
public class PlcDataModels
{
    /// <summary>
    /// Gets or sets the ID of the PLC.
    /// </summary>
    public int PlcId { get; set; }

    /// <summary>
    /// Gets or sets the PLC interface instance.
    /// </summary>
    public IPlc Plc { get; set; } = default!; // CS8618: set by composition before use

    /// <summary>
    /// Gets or sets the IP address of the PLC.
    /// </summary>
    /// <summary>
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string IpAddress { get; set; } = null!;

    /// <summary>
    /// Gets or sets the list of tags associated with the PLC.
    /// </summary>
    public List<TagMonitor> Tags { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcDataModels"/> class.
    /// </summary>
    public PlcDataModels()
    {
        this.Tags = [];
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate PLC data models logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
