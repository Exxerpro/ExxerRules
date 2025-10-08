// <copyright file="GetMachinePlcDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.MachinesPlcs.Queries.GetDetail;

/// <summary>
/// QueryAsync to retrieve the details of a machine PLC by machine and PLC identifiers.
/// </summary>
public class GetMachinePlcDetailQuery : IMonitorRequest<MachinePlcDetailVm>
{
    /// <summary>
    /// Gets or sets the unique identifier of the machine.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the PLC.
    /// </summary>
    public int PlcId { get; set; }
}
