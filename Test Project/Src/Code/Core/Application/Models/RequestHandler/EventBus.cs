// <copyright file="EventBus.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.RequestHandler;

/// <summary>
/// Provides an event bus for publishing notification events to registered handlers.
/// </summary>
public class EventBus : IEventBus
{
    private readonly IServiceProvider provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBus"/> class.
    /// </summary>
    /// <param name="provider">The service provider for resolving handlers.</param>
    public EventBus(IServiceProvider provider)
    {
        this.provider = provider;
    }

    /// <summary>
    /// Publishes an event to all registered handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="event">The event to publish.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : INotification
    {
        var handlerType = typeof(Interfaces.INotificationHandler<>).MakeGenericType(typeof(TEvent));
        var handlers = this.provider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod("ProcessAsync")!;
            await ((Task)method.Invoke(handler, [@event, cancellationToken])!).ConfigureAwait(false);
        }
    }
}
