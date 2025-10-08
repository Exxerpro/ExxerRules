// <copyright file="GetCyclesDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Queries.GetCiyclesDetail;

/// <summary>
/// Represents the GetCyclesDetailQuery.
/// </summary>
public class GetCyclesDetailQuery : IMonitorRequest<CyclesDetailVm>
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public int Id { get; set; }
}
