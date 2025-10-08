// <copyright file="SettingsListVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Settings.Queries.GetSettingsList;

using IndTrace.Application.Settings.DTO;

/// <summary>
/// Represents the SettingsListVm.
/// </summary>
public class SettingsListVm
{
    /// <summary>
    /// Gets or sets the list of Setting DTOs.
    /// </summary>
    public IList<SettingDto> Settings { get; set; } = [];

    /// <summary>
    /// Gets or sets the Count.
    /// </summary>
    public int Count { get; set; }
}
