// <copyright file="IIndTraceUserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Interfaces;

/// <summary>
/// Defines a contract for user service operations in the IndTrace system.
/// </summary>
public interface IIndTraceUserService
{
    /// <summary>
    /// Gets the current user's unique identifier.
    /// </summary>
    Task<string> CurrentUserId { get; }

    /// <summary>
    /// Gets the current user's display name.
    /// </summary>
    Task<string> CurrentUserName { get; }

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    Task<bool> IsAuthenticated { get; }
}
