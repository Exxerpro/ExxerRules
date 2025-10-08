// <copyright file="BarcodeValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Validation;

/// <summary>
/// Provides static methods for validating barcode formats and sequences in the IndTrace simulation system.
/// </summary>
public static class BarcodeValidator
{
    /// <summary>
    /// Validates that a barcode meets the required format and contains the specified part number.
    /// </summary>
    /// <param name="barcode">The barcode string to validate.</param>
    /// <param name="partNumber">The part number that must be contained in the barcode.</param>
    /// <returns>True if the barcode is valid; otherwise, false.</returns>
    /// <remarks>
    /// A valid barcode must:
    /// - Not be null or whitespace
    /// - Contain the specified part number
    /// - End with a 4-digit numeric suffix separated by a dash.
    /// </remarks>
    public static bool IsValidBarcode(string barcode, string partNumber)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return false;
        }

        if (!barcode.Contains(partNumber))
        {
            return false;
        }

        var suffix = barcode.Split('-').LastOrDefault();
        if (suffix == null || suffix.Length != 4)
        {
            return false;
        }

        return suffix.All(char.IsDigit);
    }

    /// <summary>
    /// Determines if the current barcode represents the next sequential number after the last barcode.
    /// </summary>
    /// <param name="lastBarcode">The previous barcode in the sequence.</param>
    /// <param name="currentBarcode">The current barcode to check for sequential order.</param>
    /// <returns>True if the current barcode is the next sequential number; otherwise, false.</returns>
    /// <remarks>
    /// Sequential validation checks that the numeric suffix of the current barcode is exactly
    /// one number higher than the last barcode's suffix. A reset to 0000 is also considered valid.
    /// </remarks>
    public static bool IsNextSequential(string lastBarcode, string currentBarcode)
    {
        if (string.IsNullOrWhiteSpace(lastBarcode) || string.IsNullOrWhiteSpace(currentBarcode))
        {
            return false;
        }

        var lastSuffix = int.Parse(lastBarcode.Split('-').LastOrDefault() ?? "0");
        var currentSuffix = int.Parse(currentBarcode.Split('-').LastOrDefault() ?? "0");

        return currentSuffix == (lastSuffix + 1) || currentSuffix == 0; // allow reset to 0000
    }
}
