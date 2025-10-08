// <copyright file="ConfigAppsListVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigApplication.Queries.GetConfigAppsList;

/// <summary>
/// Represents the ConfigAppsListVm.
/// </summary>
public class ConfigAppsListVm
{
    /// <summary>
    /// Gets or sets the list of ConfigApp DTOs.
    /// </summary>
    public IList<ConfigAppsDto> ConfigApp { get; set; } = [];

    /// <summary>
    /// Gets or sets the Count.
    /// </summary>
    public int Count { get; set; }
}
