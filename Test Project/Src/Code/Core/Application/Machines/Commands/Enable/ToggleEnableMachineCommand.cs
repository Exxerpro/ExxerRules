// <copyright file="ToggleEnableMachineCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Machines.Commands.Enable;

using IndTrace.Application.Machines.Queries.GetDetail;

/// <summary>
/// Command to enable or disable a machine by its identifier.
/// </summary>
public class ToggleEnableMachineCommand : IMonitorRequest<MachineDto>
{
    /// <summary>
    /// Gets or sets the unique identifier of the machine to enable or disable.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable the machine.
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to disable the machine.
    /// </summary>
    public bool Disable { get; set; }
}
