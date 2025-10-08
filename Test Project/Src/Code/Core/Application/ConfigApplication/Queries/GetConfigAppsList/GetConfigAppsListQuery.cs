// <copyright file="GetConfigAppsListQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigApplication.Queries.GetConfigAppsList;

/// <summary>
/// Represents the GetConfigAppsListQuery.
/// </summary>
public class GetConfigAppsListQuery : IMonitorRequest<ConfigAppsListVm>
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetConfigAppsListQuery"/> class.
    /// </summary>
    public GetConfigAppsListQuery()
    {
        this.Id = string.Empty;
    }
}
