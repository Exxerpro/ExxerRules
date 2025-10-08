// <copyright file="IndTraceEventsService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.UI.Services;

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Globalization;
using IndTrace.Application.Models.Helpers;

/// <summary>
/// Central event management service for the IndTrace manufacturing system.
/// Provides real-time monitoring of PLC communications, machine states, and production events.
/// Implements the Observer pattern for state change notifications and maintains thread-safe collections
/// for tracking gateway requests, responses, controller monitors, and station monitors.
/// </summary>
/// <remarks>
/// This service acts as a singleton and provides centralized state management for:
/// - PLC communication events (requests and responses)
/// - Controller and station monitoring data
/// - Production data and messages
/// - Real-time state change notifications to UI components
///
/// All collections are thread-safe using ConcurrentDictionary implementations.
/// The service automatically filters and sorts data for optimal presentation.
/// </remarks>
public class IndTraceEventsService : IIndTraceEventsService, IObservable<StateChange>
{
    // Private data for monitor
    private readonly FixedSizedStack<string> messages = new();

    /// <summary>
    /// Gets a read-only collection of system messages ordered chronologically.
    /// </summary>
    /// <value>
    /// A collection containing recent system messages with timestamps and machine identifiers.
    /// Messages are automatically managed in a fixed-size stack to prevent memory growth.
    /// </value>
    public IReadOnlyCollection<string> Messages => this.messages.ToReadOnlyCollection();

    // Define dictionaries as ConcurrentDictionary to ensure thread-safe operations
    private readonly ConcurrentDictionary<int, TaskGatewayResponse> responseEvents = new();

    private readonly ConcurrentDictionary<int, TaskGatewayRequest> requestEvents = new();

    private readonly ConcurrentDictionary<int, ControllerMonitor> controllerMonitors = new();
    private readonly ConcurrentDictionary<int, StationMonitor> stationMonitors = new();

    // Expose dictionaries as ordered read-only collections
    // 24 september 2024
    // ABR
    // TODO: Consider optimizing this if the size of these dictionaries grows significantly
    // or if performance becomes an issue due to frequent access.
    // Currently, this is fine given the small number of items (3-10), but it may need optimization
    // for thread safety and performance if the application scales.

    /// <summary>
    /// Gets a read-only dictionary of PLC gateway response events, filtered and ordered by machine ID.
    /// </summary>
    /// <value>
    /// A dictionary where keys are machine IDs and values are gateway response events containing
    /// production cycle results, validation statuses, and processing outcomes.
    /// </value>
    public IReadOnlyDictionary<int, TaskGatewayResponse> ResponseEvents =>
        new ReadOnlyDictionary<int, TaskGatewayResponse>(this.responseEvents
            .FilterByModel()
            .OrderBy(k => k.Key)
            .ToDictionary(pair => pair.Key, pair => pair.Value));

    /// <summary>
    /// Gets a read-only dictionary of PLC gateway request events, filtered and ordered by machine ID.
    /// </summary>
    /// <value>
    /// A dictionary where keys are machine IDs and values are gateway request events containing
    /// commands sent to PLCs for production operations, barcode processing, and cycle control.
    /// </value>
    public IReadOnlyDictionary<int, TaskGatewayRequest> RequestEvents =>
        new ReadOnlyDictionary<int, TaskGatewayRequest>(this.requestEvents
            .FilterByModel()
            .OrderBy(k => k.Key)
            .ToDictionary(pair => pair.Key, pair => pair.Value));

    // Filter out entries with MachineId <= 0 and sort by Label, then by Key
    // This ensures that the dictionary is always in a consistent state and ready for use.
    // The filtering and sorting are done in a thread-safe manner using LINQ.

    /// <summary>
    /// Gets a read-only dictionary of PLC controller monitors, filtered to exclude invalid entries and sorted by label and key.
    /// </summary>
    /// <value>
    /// A dictionary where keys are PLC IDs and values are controller monitor objects containing
    /// connection status, heartbeat information, IP addresses, and communication health data.
    /// Only includes controllers with valid IDs (> 0) and sorts alphabetically by label.
    /// </value>
    public IReadOnlyDictionary<int, ControllerMonitor> ControllerMonitors =>
        new ReadOnlyDictionary<int, ControllerMonitor>(this.controllerMonitors
                .Where(kvp => kvp.Key > 0)
                .OrderBy(kvp => string.IsNullOrWhiteSpace(kvp.Value.Label)) // Empty/null labels last
                .ThenBy(kvp => kvp.Value.Label) // Alphabetical within label
                .ThenBy(kvp => kvp.Key) // MachineId/Key sort
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

    /// <summary>
    /// Gets a read-only dictionary of production station monitors, filtered and ordered by machine ID.
    /// </summary>
    /// <value>
    /// A dictionary where keys are machine IDs and values are station monitor objects containing
    /// production status, cycle information, part processing data, and operational state.
    /// </value>
    public IReadOnlyDictionary<int, StationMonitor> StationMonitors =>
        new ReadOnlyDictionary<int, StationMonitor>(this.stationMonitors
            .FilterByModel()
            .OrderBy(k => k.Key)
            .ToDictionary(pair => pair.Key, pair => pair.Value));

    /// <summary>
    /// Gets the current production data containing aggregated manufacturing metrics and status information.
    /// </summary>
    /// <value>
    /// The production data object containing overall production statistics, performance indicators,
    /// and operational metrics across all monitored machines and stations.
    /// </value>
    public ProductionData ProductionData { get; private set; } = new();

    private readonly List<IObserver<StateChange>> observers = [];
    private Lock observersLock = new Lock();
    private static IndTraceEventsService instance = null!;

    /// <summary>
    /// Gets the singleton instance of the IndTrace events service.
    /// </summary>
    /// <value>
    /// The single instance of the service, created lazily on first access.
    /// This ensures global access to the event management system throughout the application.
    /// </value>
    public static IndTraceEventsService Instance => instance ??= new IndTraceEventsService();

    /// <summary>
    /// Gets or sets the current application configuration used for initializing monitoring systems.
    /// </summary>
    private ApplicationConfiguration? ApplicationConfiguration { get; set; }

    /// <summary>
    /// Applies the specified application configuration to initialize and update all monitoring systems.
    /// </summary>
    /// <param name="configuration">The application configuration containing machine, PLC, and system settings.</param>
    /// <remarks>
    /// This method updates all internal collections and monitoring systems based on the provided configuration:
    /// - Initializes request and response event tracking for enabled machines
    /// - Sets up station monitoring for production equipment
    /// - Configures controller monitoring for PLCs
    /// - Initializes production data structures
    ///
    /// If a null configuration is provided, the method returns without making changes.
    /// </remarks>
    public void ApplyConfiguration(ApplicationConfiguration configuration)
    {
        if (configuration is null)
        {
            return; // Only apply configuration if it is not null
        }

        this.ApplicationConfiguration = configuration;

        this.UpdateRequestEventsFromConfiguration(configuration);

        this.UpdateResponseEventsFromConfiguration(configuration);

        this.UpdateStationsFromConfiguration(configuration);

        this.UpdateControllersFromConfiguration(configuration);

        this.InitializeProductInfoFromConfiguration(configuration);
    }

    /// <summary>
    /// Adds a timestamped message to the system message stack and notifies observers of the state change.
    /// </summary>
    /// <param name="user">The identifier of the user or machine that generated the message.</param>
    /// <param name="message">The message content to be logged.</param>
    /// <remarks>
    /// Messages are automatically formatted with timestamp and machine identifier.
    /// The message stack has a fixed size to prevent memory growth.
    /// State change notifications are sent to all registered observers.
    /// </remarks>
    public void PushMessage(string user, string message)
    {
        var encodedMsg =
            $"Message received at {DateTime.Now.ToLocalTime().ToString(CultureInfo.InvariantCulture)} fromMachine: {user}: {message}";
        this.messages.Push(encodedMsg);

        // Notify state change
        this.UpdateState(new StateChange
        {
            PropertyName = nameof(this.Messages),
            State = encodedMsg,
        });
    }

    /// <summary>
    /// Adds or updates a PLC gateway request event and notifies observers of the state change.
    /// </summary>
    /// <param name="id">The unique identifier (typically machine ID) for the request event.</param>
    /// <param name="requestEvent">The gateway request event containing PLC command details.</param>
    /// <remarks>
    /// This method maintains a thread-safe collection of gateway requests for monitoring PLC communications.
    /// State change notifications are automatically sent to registered observers when the collection is updated.
    /// </remarks>
    public void AddOrUpdateTaskGatewayRequest(int id, TaskGatewayRequest requestEvent)
    {
        this.requestEvents[id] = requestEvent;

        // Notify state change
        this.UpdateState(new StateChange
        {
            PropertyName = nameof(this.RequestEvents),
            State = requestEvent,
        });
    }

    /// <summary>
    /// Adds or updates a PLC gateway response event, updates associated station monitors, and notifies observers.
    /// </summary>
    /// <param name="id">The unique identifier (typically machine ID) for the response event.</param>
    /// <param name="responseEvent">The gateway response event containing PLC processing results.</param>
    /// <remarks>
    /// This method not only updates the response collection but also automatically updates the corresponding
    /// station monitor based on the response data. State change notifications are sent to all registered observers.
    /// </remarks>
    public void AddOrUpdateTaskGatewayResponse(int id, TaskGatewayResponse responseEvent)
    {
        this.responseEvents[id] = responseEvent;

        // ProcessAsync the response event, such as updating station monitors
        this.UpdateStationFromGatewayResponse(responseEvent);

        // Notify state change
        this.UpdateState(new StateChange
        {
            PropertyName = nameof(this.ResponseEvents),
            State = responseEvent,
        });
    }

    private void UpdateRequestEventsFromConfiguration(ApplicationConfiguration configuration)
    {
        if (configuration == null)
        {
            return;
        }

        var currentTime = DateTime.UtcNow.ToLocalTime();

        foreach (var machine in configuration.Machines.Where(m => m.IsEnabled && m.MachineId > 0))
        {
            if (this.RequestEvents.TryGetValue(machine.MachineId, out var request))
            {
                // Update existing entry (you can add specific logic here if needed)
                request.TimeStamp = currentTime;
                request.EnsureIsValidToRenderAndPersist();
                request.Description = machine.Name;
            }
            else
            {
                // Add new entry with default values
                var newRequest = new TaskGatewayRequest
                {
                    MachineId = machine.MachineId,
                    TimeStamp = currentTime,
                    Name = machine.Name,
                    CycleStatus = CycleStatus.None,
                    PartStatus = PartStatus.None,
                    PartNumber = string.Empty,
                    GatewayTask = GatewayTask.None,
                    BarCode = string.Empty,
                };

                newRequest.EnsureIsValidToRenderAndPersist();
                this.requestEvents.TryAdd(newRequest.MachineId, newRequest);
            }
        }
    }

    private void UpdateResponseEventsFromConfiguration(ApplicationConfiguration configuration)
    {
        if (configuration == null)
        {
            return;
        }

        var currentTime = DateTime.UtcNow.ToLocalTime();

        foreach (var machine in configuration.Machines.Where(m => m.IsEnabled && m.MachineId > 0))
        {
            if (this.ResponseEvents.TryGetValue(machine.MachineId, out var response))
            {
                // Update existing entry (you can add specific logic here if needed)
                response.TimeStamp = currentTime;
                response.Name = machine.Name;
                response.EnsureIsValidToRenderAndPersist();
            }
            else
            {
                // Add new entry with default values
                var newResponse = new TaskGatewayResponse
                {
                    MachineId = machine.MachineId,
                    TimeStamp = currentTime,
                    Name = machine.Name,
                    CycleStatus = CycleStatus.None,
                    ResultValidation = ResultValidation.None,
                    Label = string.Empty,
                    PartNumber = string.Empty,
                    CyclesOk = 0,
                    NextMachineId = 0,
                    LastMachineId = 0,
                };
                newResponse.EnsureIsValidToRenderAndPersist();
                this.responseEvents.TryAdd(newResponse.MachineId, newResponse);
            }
        }
    }

    private void UpdateStationsFromConfiguration(ApplicationConfiguration configuration)
    {
        if (configuration == null)
        {
            return;
        }

        var currentTime = DateTime.UtcNow.ToLocalTime();

        foreach (var machine in configuration.Machines.Where(m => m.IsEnabled && m.MachineId > 0))
        {
            if (this.StationMonitors.TryGetValue(machine.MachineId, out var station))
            {
                station.IsEnabled = true;
                station.TimeStamp = currentTime;
                station.Description = machine.Name;
            }
            else
            {
                var stationMonitor = new StationMonitor
                {
                    IsEnabled = true,
                    MachineId = machine.MachineId,
                    TimeStamp = currentTime,
                    Name = machine.Name,
                };
                this.stationMonitors.TryAdd(stationMonitor.MachineId, stationMonitor);
            }
        }
    }

    private void UpdateControllersFromConfiguration(ApplicationConfiguration configuration)
    {
        if (configuration is null)
        {
            return;
        }

        var currentTime = DateTime.UtcNow.ToLocalTime();

        foreach (var plc in configuration.Plcs.Where(p => p.Enabled && p.PlcId > 0))
        {
            if (this.ControllerMonitors.TryGetValue(plc.PlcId, out var controller))
            {
                this.UpdateControllerFromConfiguration(controller, plc, currentTime);
            }
            else
            {
                this.AddControllerFromConfiguration(plc, currentTime);
            }
        }
    }

    private void InitializeProductInfoFromConfiguration(ApplicationConfiguration applicationConfiguration)
    {
        this.ProductionData = new ProductionData();
    }

    /// <summary>
    /// Executes UpdateStationFromGatewayRequest operation.
    /// </summary>
    /// <param name="request">The request.</param>
    public void UpdateStationFromGatewayRequest(TaskGatewayRequest? request)
    {
        if (request is null)
        {
            return;
        }

        var currentTime = DateTime.UtcNow.ToLocalTime();

        if (this.StationMonitors.TryGetValue(request.MachineId, out var station))
        {
            station.UpdateStationFromGatewayRequest(request, currentTime);
        }
        else
        {
            this.AddStationFromGatewayRequest(request, currentTime);
        }
    }

    private void AddStationFromGatewayRequest(TaskGatewayRequest request, DateTime currentTime)
    {
        var newStation = StationMonitor.CreateStationFromGatewayRequest(request, currentTime);
        this.stationMonitors.TryAdd(newStation.MachineId, newStation);
    }

    /// <summary>
    /// Executes UpdateStationFromGatewayResponse operation.
    /// </summary>
    /// <param name="response">The response.</param>
    public void UpdateStationFromGatewayResponse(TaskGatewayResponse? response)
    {
        if (response is null)
        {
            return;
        }

        var currentTime = DateTime.UtcNow.ToLocalTime();

        if (this.StationMonitors.TryGetValue(response.MachineId, out var station))
        {
            station.UpdateStationFromGatewayResponse(response, currentTime);
        }
        else
        {
            this.AddStationFromGatewayResponse(response, currentTime);
        }
    }

    private void AddStationFromGatewayResponse(TaskGatewayResponse response, DateTime currentTime)
    {
        var newStation = StationMonitor.CreateStationFromGatewayResponse(response, currentTime);
        this.stationMonitors.TryAdd(newStation.MachineId, newStation);
    }

    private void UpdateControllerFromConfiguration(ControllerMonitor controller, PlcDto plc, DateTime currentTime)
    {
        controller.MachineId = plc.MachineId;
        controller.IpAddress = plc.IpAddress;
        controller.Name = plc.Name;
        controller.TimeStamp = currentTime.AddSeconds(-60);
        controller.HeartBeat = -1;
    }

    private void AddControllerFromConfiguration(PlcDto plc, DateTime currentTime)
    {
        var controllerMonitor = new ControllerMonitor
        {
            PlcId = plc.PlcId,
            MachineId = plc.MachineId,
            IpAddress = plc.IpAddress,
            Name = plc.Name,
            TimeStamp = currentTime.AddSeconds(-60),
            HeartBeat = -1,
        };

        this.AddOrUpdateControllerMonitor(controllerMonitor.PlcId, controllerMonitor);
    }

    // Method to add or update a ControllerMonitor instance

    /// <summary>
    /// Executes AddOrUpdateControllerFromGateway operation.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="heartBeatControllerMonitor">The heartBeatControllerMonitor.</param>
    public void AddOrUpdateControllerFromGateway(int id, ControllerMonitor heartBeatControllerMonitor)
    {
        this.AddOrUpdateControllerMonitor(id, heartBeatControllerMonitor);

        this.UpdateState(new StateChange
        {
            PropertyName = nameof(this.ControllerMonitors),
            State = heartBeatControllerMonitor,
        });
    }

    private void AddOrUpdateControllerMonitor(int id, ControllerMonitor heartBeatControllerMonitor)
    {
        if (heartBeatControllerMonitor is null)
        {
            return;
        }

        // Check if the controller monitor already exists in the dictionary
        if (this.controllerMonitors.TryGetValue(id, out var existingMonitor))
        {
            // If it exists, unsubscribe the old handler to prevent memory leaks or duplicate subscriptions
            existingMonitor.ConnectionLost -= OnConnectionLostHandler;

            // Update the existing entry with the new monitor
            this.controllerMonitors[id] = heartBeatControllerMonitor;
        }
        else
        {
            // If it does not exist, add it to the dictionary
            this.controllerMonitors.TryAdd(id, heartBeatControllerMonitor);
        }

        // Ensure that the new instance is subscribed to the ConnectionLost event
        heartBeatControllerMonitor.ConnectionLost += OnConnectionLostHandler;

        // Anonymous handler to update the state when ConnectionLost event occurs
        void OnConnectionLostHandler(object? sender, EventArgs e)
        {
            this.UpdateState(new StateChange
            {
                PropertyName = nameof(this.ControllerMonitors),
                State = heartBeatControllerMonitor,
            });
        }
    }

    /// <summary>
    /// Executes Subscribe operation.
    /// </summary>
    /// <param name="observer">The observer.</param>
    /// <returns>The result of Subscribe.</returns>
    public IDisposable Subscribe(IObserver<StateChange> observer)
    {
        lock (this.observersLock)
        {
            if (!this.observers.Contains(observer))
            {
                this.observers.Add(observer);
            }
        }

        return new Unsubscriber<StateChange>(this.observers, observer, this.observersLock);
    }

    /// <summary>
    /// Executes NotifyStateChanged operation.
    /// </summary>
    /// <param name="change">The change.</param>
    public void NotifyStateChanged(StateChange change)
    {
        if (change is null)
        {
            return;
        }

        try
        {
            List<IObserver<StateChange>> snapshot;
            lock (this.observersLock)
            {
                snapshot = this.observers.ToList(); // snapshot inside lock
            }

            foreach (var observer in snapshot)
            {
                try
                {
                    observer.OnNext(change);
                }
                catch (Exception e)
                {
                    // Do we have a logger
                    // THis is a event, we must propgate or no the exception
                    // [TODO] 19 August 2025
                    Console.WriteLine(e);
                }
            }
        }
        catch (Exception e)
        {
            // Do we have a logger
            // THis is a event, we must propgate or no the exception
            // [TODO] 19 August 2025
            Console.WriteLine(e);
        }
    }

    /// <summary>
    /// Executes UpdateState operation.
    /// </summary>
    /// <param name="change">The change.</param>
    public void UpdateState(StateChange change)
    {
        this.NotifyStateChanged(change);
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate event service input and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    // TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated event handling or logging logic. Refactor for maintainability if necessary.
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency event operations, consider optimizing event dispatching and subscription logic.
}
