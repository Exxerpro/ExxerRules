// <copyright file="GetMachineDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Machines.Queries.GetDetail;

/// <summary>
/// Represents the GetMachineDetailQuery.
/// </summary>
public class GetMachineDetailQuery : IMonitorRequest<MachineDto>
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public int Id { get; set; }
}
