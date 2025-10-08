// <copyright file="IMachineStateEvaluator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models;

using IndTrace.DataStore.DataAccess;
using IndTrace.DataStore.Interfaces;

/// <summary>
/// Defines a contract for evaluating machine states in the simulator.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate machine state evaluator interface logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IMachineStateEvaluator
{
    /// <summary>
    /// Builds the next expected state based on the current snapshot and static configuration.
    /// </summary>
    /// <param name="snapshot">The current fixture context snapshot.</param>
    /// <param name="staticSnapshot">The static snapshot containing configuration data.</param>
    /// <returns>A fixture task snapshot representing the next expected state.</returns>
    FixtureTaskSnapshot BuildNextExpectedState(IFixtureContext snapshot, FixtureStaticSnapshot staticSnapshot);
}
