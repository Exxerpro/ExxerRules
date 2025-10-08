// <copyright file="ActiveStatus.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.MachinesPlcs.Commands.Update;

/// <summary>
/// Specifies the active status of a machine or process.
/// </summary>
public enum ActiveStatus
{
    /// <summary>
    /// Indicates the entity is inactive.
    /// </summary>
    Inactive = -1,

    /// <summary>
    /// Indicates the entity has no status.
    /// </summary>
    None = 0,

    /// <summary>
    /// Indicates the entity is active.
    /// </summary>
    Active = 1,
}
