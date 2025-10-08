// <copyright file="IIndTraceUser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Interfaces;

public interface IIndTraceUser
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    int UserId { get; set; }

    /// <summary>
    /// Gets or sets the username for system authentication.
    /// </summary>
    string UserName { get; set; }
}
