// <copyright file="GetReportsFilterInfoQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetReportsList.FiltersInfo;

/// <summary>
/// Represents the GetReportsFilterInfoQuery.
/// </summary>
public class GetReportsFilterInfoQuery(bool isMaster, DateTime startDate, DateTime endDate)
    : IMonitorRequest<ReportsFilterInfoVm>
{
    public bool IsMaster { get; set; } = isMaster;

    public DateTime StartDate { get; set; } = startDate;

    public DateTime EndDate { get; set; } = endDate;
}
