// <copyright file="IEventBus.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Interfaces;

/// <summary>
/// Provides methods for publishing events to an event bus.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Ensure interface is open for extension but closed for modification (OCP - SOLID). Consider using default interface methods or extension methods for future-proofing.
public interface IEventBus
{
    /// <summary>
    /// Publishes an event to the event bus.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="event">The event to publish.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : INotification;
}
