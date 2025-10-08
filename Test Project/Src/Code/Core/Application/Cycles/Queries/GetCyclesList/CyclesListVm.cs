// <copyright file="CyclesListVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Queries.GetCyclesList;

/// <summary>
/// Represents the CyclesListVm.
/// </summary>
public class CyclesListVm
{
    /// <summary>
    /// Gets or sets the Cycles.
    /// </summary>
    public IList<CyclesDto> Cycles { get; set; } = [];

    /// <summary>
    /// Gets or sets the Count.
    /// </summary>
    public int Count { get; set; }
}
