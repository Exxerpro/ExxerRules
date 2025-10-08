// <copyright file="IndTraceUserService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.UserService;

/// <summary>
/// Provides user-related services for the current user context.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate user service logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the IndTraceUserService.
/// </summary>
public class IndTraceUserService : IIndTraceUserService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndTraceUserService"/> class.
    /// </summary>
    public IndTraceUserService()
    {
    }

    // [TODO]
    // this task is a stub, replace with real user context logic
    // we have Identity Framework integrated, so we can get user info from there
    // implementation will depend on how we manage users and authentication in the app
    // we must have a way to get current user info from the request context or session
    // and we must have to had a fallback
    // [ABR]
    // [26/8/25]

    /// <summary>
    /// Gets the current user's unique identifier.
    /// </summary>
    public Task<string> CurrentUserId { get; } = Task.FromResult(Guid.NewGuid().ToString());

    /// <summary>
    /// Gets the current user's name.
    /// </summary>
    public Task<string> CurrentUserName { get; } = Task.FromResult("Admin");

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    public Task<bool> IsAuthenticated { get; } = Task.FromResult(true);
}
