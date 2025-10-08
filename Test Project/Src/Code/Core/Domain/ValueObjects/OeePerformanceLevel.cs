// <copyright file="OeePerformanceLevel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.ValueObjects;

/// <summary>
/// Represents the performance level classification for OEE values based on industry standards.
/// </summary>
public enum OeePerformanceLevel
{
    /// <summary>
    /// Poor performance - OEE below 40%.
    /// </summary>
    Poor = 0,

    /// <summary>
    /// Fair performance - OEE between 40% and 65%.
    /// </summary>
    Fair = 1,

    /// <summary>
    /// Good performance - OEE between 65% and 85%.
    /// </summary>
    Good = 2,

    /// <summary>
    /// World-class performance - OEE 85% and above.
    /// </summary>
    WorldClass = 3,
}
