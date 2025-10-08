// <copyright file="BarCodeScanned.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.CognexComm;

using System;
using IndTrace.Application.Models.Interfaces;

/// <summary>
/// Represents a barcode that has been scanned by a barcode reader.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate BarCodeScanned logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the BarCodeScanned.
/// </summary>
public class BarCodeScanned : INotification, IBarCodeScanned
{
    /// <summary>
    /// Gets or sets the unique identifier for the scanned barcode.
    /// </summary>
    public string Guid { get; set; } = System.Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the label or text content of the scanned barcode.
    /// </summary>
    public string? Label { get; set; }
}
