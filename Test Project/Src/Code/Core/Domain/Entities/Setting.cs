// <copyright file="Setting.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a configuration setting entity.
/// </summary>
public class Setting : IEntityRoot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Setting"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public Setting()
    {
        this.Config = string.Empty;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the setting.
    /// </summary>
    public int SettingId { get; set; }

    /// <summary>
    /// Gets or sets the machine identifier this setting applies to.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the configuration value or content.
    /// </summary>
    public string Config { get; set; }

    /// <summary>
    /// Returns a string representation of the Setting.
    /// </summary>
    /// <returns>A string containing the setting ID and machine ID.</returns>
    // [Fix]
    // CLAUDE
    // Date: 23/08/2025
    // Reason: Added ToString() implementation for better debugging and logging experience
    public override string ToString() => $"Setting {this.SettingId} (Machine {this.MachineId})";
}
