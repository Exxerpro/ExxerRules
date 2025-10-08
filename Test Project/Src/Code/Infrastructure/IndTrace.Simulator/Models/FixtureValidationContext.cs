// <copyright file="FixtureValidationContext.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models;

using IndTrace.DataStore.DataAccess;
using IndTrace.DataStore.Interfaces;

/// <summary>
/// Represents the context for fixture validation operations, containing PLC, database, and task results.
/// </summary>
public class FixtureValidationContext
{
    /// <summary>
    /// Gets the PLC results snapshot.
    /// </summary>
    public required FixturePlcSnapshot PlcResults { get; init; }

    /// <summary>
    /// Gets the database results snapshot.
    /// </summary>
    public required FixtureDbSnapshot DbResults { get; init; }

    /// <summary>
    /// Gets or sets the expected task results.
    /// </summary>
    public required FixtureTaskSnapshot ExpectedResults { get; set; }

    /// <summary>
    /// Gets the configuration data snapshot.
    /// </summary>
    public required FixtureStaticSnapshot ConfigDataSnapShot { get; init; }

    // Optional unified access as interface if needed generically

    /// <summary>
    /// Gets the PLC results as a validation result interface.
    /// </summary>
    public IFixtureValidationResult PlcResult => this.PlcResults;

    /// <summary>
    /// Gets the database results as a validation result interface.
    /// </summary>
    public IFixtureValidationResult DbResult => this.DbResults;

    /// <summary>
    /// Gets the task results as a validation result interface.
    /// </summary>
    public IFixtureValidationResult TaskResult => this.ExpectedResults;

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate fixture validation context logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
