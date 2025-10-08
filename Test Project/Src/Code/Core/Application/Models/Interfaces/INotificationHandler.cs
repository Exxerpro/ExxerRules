// <copyright file="INotificationHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Interfaces;

/// <summary>
/// Defines a handler for processing notification events.
/// </summary>
/// <typeparam name="TEvent">The type of the notification event.</typeparam>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Ensure interface is open for extension but closed for modification (OCP - SOLID). Consider using default interface methods or extension methods for future-proofing.
public interface INotificationHandler<in TEvent> where TEvent : INotification
{
    /// <summary>
    /// Processes the specified notification event.
    /// </summary>
    /// <param name="event">The notification event to process.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<Result> Process(TEvent @event, CancellationToken cancellationToken);
}
