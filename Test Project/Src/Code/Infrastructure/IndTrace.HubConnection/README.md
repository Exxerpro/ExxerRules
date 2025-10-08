# IndTrace.HubConnection

Abstractions and thin adapters over SignalR `HubConnection` for DI, testability, and clean architecture. Aligns with ADR: IHubConnection Interface – Single Responsibility SignalR Wrapper.

## What’s Inside
- `Abstractions/IHubConnection` – minimal, cancellation-aware surface with correct SignalR lifecycle events.
- `Abstractions/IHubConnectionFactory` – builds configured, disconnected connections (no auto-start).
- `Implementations/SignalRHubConnectionAdapter` – delegates to concrete `HubConnection`.
- `Implementations/HubConnectionFactory` – creates the inner `HubConnection` from `HubMonitorOptions`.
- `Extensions/HubConnectionInterfaceExtensions` – `TryStartHubConnectionAsync`, `EnsureHubConnectionIsValid` for the interface.
- `Extensions/ServiceCollectionExtensions` – `AddHubConnectionAbstractions(IConfiguration)` for DI.

## DI Registration
```
// Program.cs
using IndTrace.HubConnection.Extensions;

builder.Services.AddHubConnectionAbstractions(builder.Configuration);
```

## Typical Usage
```
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Extensions;

public class Worker(IHubConnectionFactory factory, ILogger<Worker> logger) : BackgroundService
{
    private IHubConnection? connection;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        connection ??= await factory.CreateAsync(stoppingToken);

        // Start (with retry scheduling on error)
        await connection.TryStartHubConnectionAsync(logger, stoppingToken);

        // Subscribe
        connection.On<string, string>("BroadcastMessageToClients", (user, msg) =>
        {
            logger.LogInformation("{User}: {Message}", user, msg);
            return Task.CompletedTask;
        });

        // Send
        await connection.SendAsync("BroadcastMessageToClients", new object?[]{"Gateway","Started"}, stoppingToken);
    }
}
```

## Lifecycle Events
- `Reconnecting(Exception?)`, `Reconnected(string?)`, `Closed(Exception?)` – parity with SignalR.

## Migration Notes
- Old `HubConnection` helpers exist in `IndTrace.Dependencies/Hubs`. Porting strategy:
  - Keep old helpers temporarily; add `IHubConnection` overloads under `IndTrace.HubConnection.Extensions`.
  - Flip fields and factories at call sites (`HubConnection` → `IHubConnection`, `CreateConnectionAsync` → `CreateAsync`).
  - Import `IndTrace.HubConnection.Extensions` and use the interface helpers.

## Testing Strategy (TDD)
Start with three RED tests to pin behavior:
1) Adapter forwards `InvokeAsync<T>`/`SendAsync` with cancellation.
2) Adapter forwards lifecycle events.
3) `EnsureHubConnectionIsValid` creates when null and starts when disconnected.

See `docs/HubInventory.md` for the full inventory and migration map.
