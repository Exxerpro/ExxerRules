// <copyright file="MachineUpdateCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Machines.Commands.Update;

using IndTrace.Application.Machines.Queries.GetDetail;

/// <summary>
/// Represents the MachineUpdateCommand.
/// </summary>
public class MachineUpdateCommand : IMonitorRequest<MachineDto>
{
    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the Name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Location.
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MachineType.
    /// </summary>
    public MachineType? MachineType { get; set; }

    /// <summary>
    /// Gets or sets the WorkFlowType.
    /// </summary>
    public WorkFlowType? WorkFlowType { get; set; }
}
