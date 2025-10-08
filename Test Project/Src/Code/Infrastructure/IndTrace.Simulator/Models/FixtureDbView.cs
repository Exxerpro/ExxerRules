// <copyright file="FixtureDbView.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models;

/// <summary>
/// Represents a database view for fixture data, containing barcode and cycle information.
/// </summary>
public class FixtureDbView
{
    /// <summary>
    /// Gets or sets the barcode identifier.
    /// </summary>
    public int BarCodeId { get; set; }

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the barcode value.
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the barcode machine identifier.
    /// </summary>
    public int BarcodeMachineId { get; set; }

    /// <summary>
    /// Gets or sets the barcode part status.
    /// </summary>
    public int BarcodePartStatus { get; set; }

    /// <summary>
    /// Gets or sets the flow status.
    /// </summary>
    public int FlowStatus { get; set; }

    /// <summary>
    /// Gets or sets the cycle identifier.
    /// </summary>
    public int CycleId { get; set; }

    /// <summary>
    /// Gets or sets the cycle machine identifier.
    /// </summary>
    public int CycleMachineId { get; set; }

    /// <summary>
    /// Gets or sets the cycle status.
    /// </summary>
    public int CycleStatus { get; set; }

    /// <summary>
    /// Gets or sets the cycle part status.
    /// </summary>
    public int CyclePartStatus { get; set; }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate fixture DB view logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
