// <copyright file="IndTraceUser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;
using IndTrace.Domain.Models;

/// <summary>
/// Represents a user entity with system access and identification information.
/// </summary>
public class IndTraceUser : AuditableEntity, IEntityRoot, IIndTraceUser
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the username for system authentication.
    /// </summary>
    public string UserName { get; set; } = string.Empty;
}
