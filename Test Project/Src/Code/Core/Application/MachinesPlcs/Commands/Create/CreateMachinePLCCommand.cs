// <copyright file="CreateMachinePLCCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.MachinesPlcs.Commands.Create;

/// <summary>
/// Represents the CreateMachinePlcCommand.
/// </summary>
public class CreateMachinePlcCommand : IMonitorRequest<MachinePlcCreated>
{
    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the Machine.
    /// </summary>
    public Machine Machine { get; set; } = new();

    /// <summary>
    /// Gets or sets the PlCsId.
    /// </summary>
    public int PlCsId { get; set; }

    /// <summary>
    /// Gets or sets the Plc.
    /// </summary>
    public Plc Plc { get; set; } = new();
}
