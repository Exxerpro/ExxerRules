// <copyright file="IDataContext.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Services;

/// <summary>
/// Defines the context for data operations, distinguishing between production and simulation environments.
/// </summary>
public interface IDataContext
{
    /// <summary>
    /// Gets a value indicating whether this context represents a simulation environment.
    /// </summary>
    bool IsSimulation { get; }

    /// <summary>
    /// Gets the environment name for this data context.
    /// </summary>
    /// <remarks>
    /// Valid values: "Production", "Simulation", "Demo".
    /// </remarks>
    string Environment { get; }

    /// <summary>
    /// Gets a value indicating whether this context allows database writes.
    /// </summary>
    bool AllowsDatabaseWrites { get; }

    /// <summary>
    /// Gets the database connection string identifier for this context.
    /// </summary>
    string DatabaseIdentifier { get; }
}
