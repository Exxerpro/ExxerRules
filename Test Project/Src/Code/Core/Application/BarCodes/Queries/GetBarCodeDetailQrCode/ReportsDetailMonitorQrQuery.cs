// <copyright file="ReportsDetailMonitorQrQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetBarCodeDetailQrCode;

/// <summary>
/// Represents the ReportsDetailMonitorQrQuery.
/// </summary>
public class ReportsDetailMonitorQrQuery : IMonitorRequest<ReportDetailMonitorVm>
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
