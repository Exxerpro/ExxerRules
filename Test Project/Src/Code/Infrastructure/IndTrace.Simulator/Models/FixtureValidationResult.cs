// <copyright file="FixtureValidationResult.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models;

using IndTrace.DataStore.Interfaces;

/// <summary>
/// Represents the result of a fixture validation operation, containing barcode, cycle, and machine information.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate fixture validation result logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the FixtureValidationResult.
/// </summary>
public class FixtureValidationResult : IFixtureValidationResult
{
    /// <summary>
    /// Gets the barcode identifier.
    /// </summary>
    public int BarCodeId { get; init; }

    /// <summary>
    /// Gets the barcode value.
    /// </summary>
    public string Barcode { get; init; } = string.Empty;

    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public int ProductId { get; init; }

    /// <summary>
    /// Gets the cycle identifier.
    /// </summary>
    public int CycleId { get; init; }

    /// <summary>
    /// Gets the cycle status.
    /// </summary>
    public int CycleStatus { get; init; }

    /// <summary>
    /// Gets the part status.
    /// </summary>
    public int PartStatus { get; init; }

    /// <summary>
    /// Gets the flow status.
    /// </summary>
    public int FlowStatus { get; init; }

    /// <summary>
    /// Gets the result validation status.
    /// </summary>
    public int ResultValidation { get; init; }

    /// <summary>
    /// Gets the last machine identifier.
    /// </summary>
    public int LastMachineId { get; init; }

    /// <summary>
    /// Gets the next machine identifier.
    /// </summary>
    public int NextMachineId { get; init; }

    /// <summary>
    /// Gets the cycle machine identifier.
    /// </summary>
    public int CycleMachineId { get; init; }

    /// <summary>
    /// Gets the barcode machine identifier.
    /// </summary>
    public int BarcodeMachineId { get; init; }

    /// <summary>
    /// Gets the actual station identifier.
    /// </summary>
    public int ActualStation { get; init; }

    /// <summary>
    /// Gets the source of the validation result.
    /// </summary>
    public string Source { get; init; } = string.Empty;

    /// <summary>
    /// Determines whether this validation result equals another validation result.
    /// </summary>
    /// <param name="other">The other validation result to compare with.</param>
    /// <returns>True if the validation results are equal; otherwise, false.</returns>
    public bool Equals(IFixtureValidationResult? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(this.Barcode, other.Barcode, StringComparison.OrdinalIgnoreCase)
               && this.CycleStatus == other.CycleStatus
               && this.PartStatus == other.PartStatus
               && this.FlowStatus == other.FlowStatus
               && this.NextMachineId == other.NextMachineId
               && this.ActualStation == other.ActualStation;
    }

    /// <summary>
    /// Determines whether this validation result equals the specified object.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return this.Equals(obj as IFixtureValidationResult);
    }

    /// <summary>
    /// Returns the hash code for this validation result.
    /// </summary>
    /// <returns>A hash code for the current validation result.</returns>
    public override int GetHashCode() => HashCode.Combine(
        this.Barcode?.ToLowerInvariant(),
        this.CycleStatus,
        this.PartStatus,
        this.FlowStatus,
        this.NextMachineId,
        this.ActualStation);
}
