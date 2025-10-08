// <copyright file="GetCyclesListQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Queries.GetCyclesList;

/// <summary>
/// Represents the GetCyclesListQuery.
/// </summary>
public class GetCyclesListQuery : IMonitorRequest<CyclesListVm>
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Page.
    /// <summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the PageSize.
    /// <summary>
    public int PageSize { get; set; } = 50;
}
