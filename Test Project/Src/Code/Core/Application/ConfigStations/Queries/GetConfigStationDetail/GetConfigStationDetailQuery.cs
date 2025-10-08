// <copyright file="GetConfigStationDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigStations.Queries.GetConfigStationDetail;

/// <summary>
/// Represents the GetConfigStationDetailQuery.
/// </summary>
public class GetConfigStationDetailQuery : IMonitorRequest<ConfigStationDetailVm>
{
    /// <summary>
    /// Gets or sets the PartNumber.
    /// </summary>
    public string? PartNumber { get; set; }
}
