// <copyright file="PartStatusEnum.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models.Constants;

/// <summary>
/// Represents the status states for parts in the simulator.
/// </summary>
public enum PartStatusEnum
{
    /// <summary>
    /// No status is assigned to the part.
    /// </summary>
    None = 0,

    /// <summary>
    /// The part is in good condition.
    /// </summary>
    Ok = 1,

    /// <summary>
    /// The part is not in good condition.
    /// </summary>
    NOK = 2,

    /// <summary>
    /// The part has been restored.
    /// </summary>
    Restored = 4,

    /// <summary>
    /// The part has been rejected.
    /// </summary>
    Rejected = 8,

    /// <summary>
    /// The part has been scrapped.
    /// </summary>
    Scrap = 512,
}
