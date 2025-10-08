// <copyright file="IndTraceUser.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Identity.Users;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using IndTrace.Domain.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

/// <summary>
/// Represents the IndTraceUser.
/// </summary>
public class IndTraceUser : IdentityUser, IIndTraceUser
{
    /// <summary>
    /// Gets or sets the UserId.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the UserName, ensuring non-nullable contract.
    /// </summary>
    public override string? UserName
    {
        get => base.UserName;
        set => base.UserName = value;
    }

    /// <summary>
    /// Gets or sets the UserName as non-nullable for the interface contract.
    /// </summary>
    string IIndTraceUser.UserName
    {
        get => base.UserName ?? string.Empty;
        set => base.UserName = value;
    }
}

/// <summary>
/// Represents the IdentityUserAccessor.
/// </summary>
public class IdentityUserAccessor
{
    private readonly UserManager<IndTraceUser> userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityUserAccessor"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="userManager">The userManager.</param>
    public IdentityUserAccessor(UserManager<IndTraceUser> userManager)
    {
        this.userManager = userManager;
    }

    /// <summary>
    /// Executes GetCurrentUserAsync operation.
    /// </summary>
    /// <param name="userId">The userId.</param>
    /// <returns>The result of GetCurrentUserAsync.</returns>
    public async Task<IndTraceUser?> GetCurrentUserAsync(string userId)
    {
        return await this.userManager.FindByIdAsync(userId);
    }

    /// <summary>
    /// Executes FindByIdAsync operation.
    /// </summary>
    /// <param name="userId">The userId.</param>
    /// <returns>The result of FindByIdAsync.</returns>
    public async Task<IndTraceUser?> FindByIdAsync(string userId)
    {
        return await this.userManager.FindByIdAsync(userId);
    }
}

/// <summary>
/// Represents the IdentityRedirectManager.
/// </summary>
public class IdentityRedirectManager
{
    /// <summary>
    /// Executes GetRedirectUrl operation.
    /// </summary>
    /// <param name="returnUrl">The returnUrl.</param>
    /// <returns>The result of GetRedirectUrl.</returns>
    public string GetRedirectUrl(string returnUrl)
    {
        // Logic to determine the redirect URL based on the returnUrl
        return string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
    }
}

/// <summary>
/// Represents the IdentityRevalidatingAuthenticationStateProvider.
/// </summary>
public class IdentityRevalidatingAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly UserManager<IndTraceUser> userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityRevalidatingAuthenticationStateProvider"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="userManager">The userManager.</param>
    public IdentityRevalidatingAuthenticationStateProvider(UserManager<IndTraceUser> userManager)
    {
        this.userManager = userManager;
    }

    /// <summary>
    /// Executes GetAuthenticationStateAsync operation.
    /// </summary>
    /// <returns>The result of GetAuthenticationStateAsync.</returns>
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Implement logic to revalidate the authentication state
        var user = this.userManager.GetUserAsync(new ClaimsPrincipal()).Result;
        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            var identity = new ClaimsIdentity(claims, "IndTraceIdentity");
            var principal = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(principal));
        }
        else
        {
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
        }
    }
}

/// <summary>
/// Represents the IdentityNoOpEmailSender.
/// </summary>
public class IdentityNoOpEmailSender<TUser> : IEmailSender
    where TUser : IIndTraceApplicationUser
{
    /// <summary>
    /// Executes SendEmailAsync operation.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="htmlMessage">The htmlMessage.</param>
    /// <returns>The result of SendEmailAsync.</returns>
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        throw new NotImplementedException();
    }
}
