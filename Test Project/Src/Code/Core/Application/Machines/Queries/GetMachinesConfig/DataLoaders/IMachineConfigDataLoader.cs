// <copyright file="IMachineConfigDataLoader.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Machines.Queries.GetMachinesConfig.DataLoaders;

/// <summary>
/// Loads and filters machine configuration data with industrial validation patterns.
/// </summary>
public interface IMachineConfigDataLoader
{
    /// <summary>
    /// Loads all related machine configuration data for a specific part number.
    /// </summary>
    /// <param name="partNumber">Product part number for configuration lookup.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing loaded configuration context.</returns>
    Task<Result<MachineConfigContext>> LoadByPartNumberAsync(
        string partNumber,
        CancellationToken cancellationToken);
}

/// <summary>
/// Contains all related data for machine configuration assembly.
/// </summary>
public sealed record MachineConfigContext(
    Product Product,
    IReadOnlyList<WorkFlow> WorkFlows,
    IReadOnlyList<Machine> Machines,
    IReadOnlyList<MachinePlc> MachinePlcs,
    IReadOnlyList<Plc> Plcs,
    IReadOnlyList<Variable> Variables)
{
    /// <summary>
    /// Gets machine IDs from workflows for filtering operations.
    /// </summary>
    public IReadOnlyList<int> MachineIds => WorkFlows
        .Select(wf => wf.NextMachineId)
        .Distinct()
        .ToList();
}
