// <copyright file="ReportsDetailMonitorQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetBarCodeDetailMonitor;

/// <summary>
/// Represents the ReportsDetailMonitorQuery.
/// </summary>
public class ReportsDetailMonitorQuery : IMonitorRequest<ReportDetailMonitorVm>
{
    /// <summary>
    /// Gets or sets the BarCode.
    /// </summary>
    public string BarCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the BarCodeId.
    /// </summary>
    public int BarCodeId { get; set; }
}
