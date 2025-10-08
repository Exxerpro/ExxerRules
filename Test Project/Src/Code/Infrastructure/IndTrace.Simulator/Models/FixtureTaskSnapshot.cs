// <copyright file="FixtureTaskSnapshot.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models;

/// <summary>
/// Represents a snapshot of a fixture task, containing validation result information for task operations.
/// </summary>
public class FixtureTaskSnapshot : FixtureValidationResult
{
    /// <summary>
    /// Creates a new fixture task snapshot from the specified parameters.
    /// </summary>
    /// <param name="barcode">The barcode value.</param>
    /// <param name="productId">The product identifier.</param>
    /// <param name="barCodeId">The barcode identifier.</param>
    /// <param name="cycleId">The cycle identifier.</param>
    /// <param name="cycleStatus">The cycle status.</param>
    /// <param name="partStatus">The part status.</param>
    /// <param name="flowStatus">The flow status.</param>
    /// <param name="resultValidation">The result validation status.</param>
    /// <param name="lastMachineId">The last machine identifier.</param>
    /// <param name="nextMachineId">The next machine identifier.</param>
    /// <param name="cycleMachineId">The cycle machine identifier.</param>
    /// <param name="barcodeMachineId">The barcode machine identifier.</param>
    /// <param name="actualStation">The actual station identifier.</param>
    /// <returns>A new fixture task snapshot instance.</returns>
    public static FixtureTaskSnapshot From(
        string barcode,
        int productId,
        int barCodeId,
        int cycleId,
        int cycleStatus,
        int partStatus,
        int flowStatus,
        int resultValidation,
        int lastMachineId,
        int nextMachineId,
        int cycleMachineId,
        int barcodeMachineId,
        int actualStation)
    {
        return new FixtureTaskSnapshot
        {
            Source = "TASK",
            Barcode = barcode,
            ProductId = productId,
            BarCodeId = barCodeId,
            CycleId = cycleId,
            CycleStatus = cycleStatus,
            PartStatus = partStatus,
            FlowStatus = flowStatus,
            ResultValidation = resultValidation,
            LastMachineId = lastMachineId,
            NextMachineId = nextMachineId,
            CycleMachineId = cycleMachineId,
            BarcodeMachineId = barcodeMachineId,
            ActualStation = actualStation,
        };
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate fixture task snapshot logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
