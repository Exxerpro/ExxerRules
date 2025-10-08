// <copyright file="INotificationService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Interfaces;

/// <summary>
/// Provides methods for sending notifications.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Ensure interface is open for extension but closed for modification (OCP - SOLID). Consider using default interface methods or extension methods for future-proofing.
public interface INotificationService
{
    /// <summary>
    /// Sends a notification message asynchronously.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Result> SendAsync(MessageDto message, CancellationToken cancellationToken = default);
}
