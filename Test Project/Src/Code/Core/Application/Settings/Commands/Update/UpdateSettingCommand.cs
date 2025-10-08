// <copyright file="UpdateSettingCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Settings.Commands.Update;

using IndTrace.Application.Settings.Queries.GetSettingDetail;

/// <summary>
/// Represents the UpdateSettingCommand.
/// </summary>
public class UpdateSettingCommand : IMonitorRequest<SettingDetailVm>
{
    /// <summary>
    /// Gets or sets the SettingId.
    /// </summary>
    public int? SettingId { get; set; }

    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int? MachineId { get; set; }

    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    public string Config { get; set; } = string.Empty;
}
