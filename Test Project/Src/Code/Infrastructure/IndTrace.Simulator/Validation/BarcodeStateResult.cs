// <copyright file="BarcodeStateResult.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Validation;

/// <summary>
/// Represents the BarcodeStateResult.
/// </summary>
public class BarcodeStateResult
{
    /// <summary>
    /// Gets or sets the CurrentState.
    /// </summary>
    public string CurrentState { get; set; } = string.Empty;

    /// <summary>
    /// Executes MatchesExpectedState operation.
    /// </summary>
    /// <param name="expected)">The expected).</param>
    /// <param name="expected">The expected.</param>
    /// <param name="StringComparison.OrdinalIgnoreCase">The StringComparison.OrdinalIgnoreCase.</param>
    /// <returns>The result of MatchesExpectedState.</returns>
    public bool MatchesExpectedState(string expected) => string.Equals(this.CurrentState, expected, StringComparison.OrdinalIgnoreCase);
}
