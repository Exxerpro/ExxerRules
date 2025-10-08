// <copyright file="GetMachineConfigQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Machines.Queries.GetMachinesConfig;

/// <summary>
/// Represents the GetMachineConfigQuery.
/// </summary>
public class GetMachineConfigQuery : IMonitorRequest<MachineConfigVm>
{
    /// <summary>
    /// Gets or sets the PartNumber.
    /// </summary>
    public string? PartNumber { get; set; }
}
