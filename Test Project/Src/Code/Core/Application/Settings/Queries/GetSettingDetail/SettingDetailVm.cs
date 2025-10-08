// <copyright file="SettingDetailVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Settings.Queries.GetSettingDetail;

/// <summary>
/// Represents the SettingDetailVm.
/// </summary>
public class SettingDetailVm
{
    /// <summary>
    /// Gets or sets the SettingId.
    /// </summary>
    public int SettingId { get; set; }

    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Config { get; set; } = null!;

    /// <summary>
    /// Executes ToDto operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToDto.</returns>
    public static IndQuestResults.Result<SettingDetailVm> ToDto(Setting src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<SettingDetailVm>.WithFailure("Setting source cannot be null");
        }

        return IndQuestResults.Result<SettingDetailVm>.Success(new SettingDetailVm
        {
            SettingId = src.SettingId,
            MachineId = src.MachineId,
            Config = src.Config,
        });
    }

    /// <summary>
    /// Executes ToEntity operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToEntity.</returns>
    public static IndQuestResults.Result<Setting> ToEntity(SettingDetailVm src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<Setting>.WithFailure("SettingDetailVm source cannot be null");
        }

        return IndQuestResults.Result<Setting>.Success(new Setting
        {
            SettingId = src.SettingId,
            MachineId = src.MachineId,
            Config = src.Config,
        });
    }
}
