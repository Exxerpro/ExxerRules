// <copyright file="IPlcDetailDataLoader.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Plcs.Queries.GetDetail.DataLoaders;

/// <summary>
/// Loads complete PLC detail data from multiple repositories.
/// Based on GetPlcDetailQueryHandler data loading pattern with proper filtering.
/// </summary>
public interface IPlcDetailDataLoader
{
    /// <summary>
    /// Loads all related data for a PLC detail view.
    /// </summary>
    /// <param name="plcId">The PLC ID to load data for.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing loaded PLC detail context.</returns>
    Task<Result<PlcDetailContext>> LoadByPlcIdAsync(
        int plcId,
        CancellationToken cancellationToken);
}

/// <summary>
/// Complete PLC detail context with all related data.
/// </summary>
public sealed record PlcDetailContext(
    Plc Plc,
    IReadOnlyList<MachinePlc> MachinePlcs,
    IReadOnlyList<Machine> Machines,
    IReadOnlyList<Variable> Variables,
    IReadOnlyList<VariablesGroup> VariableGroups);
