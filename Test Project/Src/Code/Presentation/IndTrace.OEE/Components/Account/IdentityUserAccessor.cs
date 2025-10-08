// <copyright file="IdentityUserAccessor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Components.Account;

using IndTrace.OEE.Data;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Provides access to the current user with validation and error handling.
/// </summary>
internal sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager, IdentityRedirectManager redirectManager)
{
    /// <summary>
    /// Gets the current user from the HTTP context and ensures the user exists.
    /// </summary>
    /// <param name="context">The HTTP context containing user information.</param>
    /// <returns>The current application user.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the user cannot be loaded.</exception>
    public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}
