// <copyright file="IdentityNoOpEmailSender.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Components.Account;

using IndTrace.OEE.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

// Remove the "else if (EmailSender is IdentityNoOpEmailSender)" block from RegisterConfirmation.razor after updating with a real implementation.

/// <summary>
/// A no-operation email sender implementation that provides basic email functionality for development and testing scenarios.
/// </summary>
internal sealed class IdentityNoOpEmailSender : IEmailSender<ApplicationUser>
{
    private readonly IEmailSender emailSender = new NoOpEmailSender();

    /// <summary>
    /// Sends an email confirmation link to the specified user.
    /// </summary>
    /// <param name="user">The application user.</param>
    /// <param name="email">The email address to send the confirmation link to.</param>
    /// <param name="confirmationLink">The confirmation link URL.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        this.emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

    /// <summary>
    /// Sends a password reset link to the specified user.
    /// </summary>
    /// <param name="user">The application user.</param>
    /// <param name="email">The email address to send the reset link to.</param>
    /// <param name="resetLink">The password reset link URL.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        this.emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

    /// <summary>
    /// Sends a password reset code to the specified user.
    /// </summary>
    /// <param name="user">The application user.</param>
    /// <param name="email">The email address to send the reset code to.</param>
    /// <param name="resetCode">The password reset code.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        this.emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
}
