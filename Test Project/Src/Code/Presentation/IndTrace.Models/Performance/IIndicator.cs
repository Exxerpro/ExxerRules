// <copyright file="IIndicator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.UI.Models.Performance;

/// <summary>
/// Defines a performance indicator that can calculate values based on data.
/// </summary>
public interface IIndicator
{
    /// <summary>
    /// Gets or sets the calculated value of the indicator.
    /// </summary>
    double Value { get; set; }

    /// <summary>
    /// Gets or sets the two-dimensional data array used for calculations.
    /// </summary>
    double[,] Data { get; set; }

    /// <summary>
    /// Gets or sets the action that performs the value calculation.
    /// </summary>
    Action CalculateValue { get; set; }
}

// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IIndicator logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
