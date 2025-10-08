// <copyright file="StatusConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a status configuration lookup entity for system status definitions.
/// </summary>
public class StatusConfiguration : ILookupEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StatusConfiguration"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public StatusConfiguration()
    {
        this.Message = string.Empty;
    }

    /// <summary>
    /// Gets or sets the machine identifier.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the status code for the machine.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the status was last modified.
    /// </summary>
    public DateTime ModifiedOn { get; set; }
}
