// <copyright file="WatchDog.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

/// <summary>
/// Represents the watchdog status enumeration for system monitoring.
/// </summary>
public enum WatchDog
{
    /// <summary>
    /// Indicates that the watchdog is disabled.
    /// </summary>
    Disable = -1,

    /// <summary>
    /// Indicates that the watchdog is enabled.
    /// </summary>
    Enable = 1,
}
