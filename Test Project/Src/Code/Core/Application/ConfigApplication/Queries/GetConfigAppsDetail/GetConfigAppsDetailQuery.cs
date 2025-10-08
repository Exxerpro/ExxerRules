// <copyright file="GetConfigAppsDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigApplication.Queries.GetConfigAppsDetail;

/// <summary>
/// Represents the GetConfigAppsDetailQuery.
/// </summary>
public class GetConfigAppsDetailQuery : IMonitorRequest<ConfigAppDto>
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public int Id { get; set; }
}
