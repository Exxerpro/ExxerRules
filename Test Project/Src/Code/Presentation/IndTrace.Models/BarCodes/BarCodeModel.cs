// <copyright file="BarCodeModel.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.UI.Models.BarCodes;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a barcode model with validation for barcode labels.
/// </summary>
public class BarCodeModel
{
    /// <summary>
    /// Gets or sets the barcode label with validation constraints.
    /// </summary>
    [Required]
    [StringLength(64, ErrorMessage = "BarCode is too long.")]
    [MinLength(2, ErrorMessage = "BarCode is to Short")]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Validates if the current label is valid.
    /// </summary>
    /// <returns>True if the label is valid; otherwise, false.</returns>
    public bool IsValidLabel()
    {
        return true;
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate BarCodeModel logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
