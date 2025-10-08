// <copyright file="QrCodeGenerator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.UI.Models.BarCodes;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a QR code generator model with validation for QR code labels.
/// </summary>
public class QrCodeGenerator
{
    /// <summary>
    /// Gets or sets the label for the QR code to be generated.
    /// </summary>
    [Required]
    public string Label { get; set; } = string.Empty;

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate QrCodeGenerator logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
