// <copyright file="IMachineConfigAssembler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.Machines.Queries.GetMachinesConfig.DataLoaders;

namespace IndTrace.Application.Machines.Queries.GetMachinesConfig.Assemblers;

/// <summary>
/// Assembles MachineConfigVm from loaded data context with proper validation.
/// </summary>
public interface IMachineConfigAssembler
{
    /// <summary>
    /// Assembles machine configuration view model from provided context.
    /// </summary>
    /// <param name="context">Loaded machine configuration context.</param>
    /// <returns>Result containing assembled view model or failure reasons.</returns>
    Result<MachineConfigVm> AssembleConfiguration(MachineConfigContext context);
}
