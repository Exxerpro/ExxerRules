// <copyright file="GetBarCodeReportQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetReportsReport;

/// <summary>
/// Represents the GetBarCodeReportQuery.
/// </summary>
public class GetBarCodeReportQuery : IMonitorRequest<List<BarCodeReportVm>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetBarCodeReportQuery"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public GetBarCodeReportQuery()
    {
        this.BarCodesIdList = [];
    }

    /// <summary>
    /// Gets or sets the BarCodesIdList.
    /// </summary>
    public List<int> BarCodesIdList { get; set; }
}
