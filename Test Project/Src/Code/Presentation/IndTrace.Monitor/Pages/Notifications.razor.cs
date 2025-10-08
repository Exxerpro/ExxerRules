using EventStore.Client;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Threading;

namespace IndTrace.Monitor.Pages;

/// <summary>
/// Represents the Notifications page component that handles event notifications using EventStore.
/// </summary>
public partial class Notifications : ComponentBase
{
    //[Inject]
    //private EventStoreClient _eventStoreClient { get; set; }

    /// <summary>
    /// Sends an event to the EventStore.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task SendEvent()
    {
        //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate Notifications page logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

        var settings = EventStoreClientSettings.Create(connectionString);

        var client = new EventStoreClient(settings);

        var evt = new TestEvent()
        {
            EntityId = Guid.CreateVersion7().ToString("N"),
            ImportantData = "CreateCodeTaskAsync",
        };

        var eventData = new EventData(
            Uuid.NewUuid(),
                            "Command",
                            JsonSerializer.SerializeToUtf8Bytes(evt));

        _ = client.AppendToStreamAsync(
            "IndTrace",
                            StreamState.Any,
                            new[] { eventData });

        //var result = client.ReadStreamAsync(Direction.Forwards,
        //                 "some-stream",
        //                 StreamPosition.Start);

        // NOTE: For now, no awaited operations. Return a completed task to avoid CS1998.
        return Task.CompletedTask;
    }
}

/// <summary>
/// Represents a test event for EventStore operations.
/// </summary>
internal class TestEvent
{
    /// <summary>
    /// Gets or sets the entity identifier.
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the important data associated with the event.
    /// </summary>
    public string ImportantData { get; set; } = string.Empty;
}
