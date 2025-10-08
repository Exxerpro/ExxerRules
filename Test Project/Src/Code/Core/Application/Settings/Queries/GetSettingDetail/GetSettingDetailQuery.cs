// <copyright file="GetSettingDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Settings.Queries.GetSettingDetail;

/// <summary>
/// Represents a query to retrieve the details of a specific setting.
/// </summary>
public class GetSettingDetailQuery : IMonitorRequest<SettingDetailVm>
{
    /// <summary>
    /// Gets or sets the unique identifier of the setting to retrieve.
    /// </summary>
    public int SettingId { get; set; }
}
