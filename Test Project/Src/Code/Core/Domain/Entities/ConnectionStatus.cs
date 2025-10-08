// <copyright file="ConnectionStatus.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;
using IndTrace.Domain.Models;

/// <summary>
/// Represents a connection status lookup entity for system connectivity monitoring.
/// </summary>
public class ConnectionStatus : AuditableEntity, ILookupEntity
{
    /// <summary>
    /// Gets or sets the machine identifier.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the status code of the connection.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the status was last modified.
    /// </summary>
    public new DateTime ModifiedOn { get; set; }
}
