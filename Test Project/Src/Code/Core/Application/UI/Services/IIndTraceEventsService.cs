// <copyright file="IIndTraceEventsService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.UI.Services;

// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Review interface segregation: ensure IIndTraceEventsService does not force implementers to depend on methods they do not use (ISP - SOLID). Consider splitting if needed.
// TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated method signatures or patterns that could be abstracted. Refactor for maintainability if necessary.
public interface IIndTraceEventsService : IObservable<StateChange>
{
    // Property to expose messages in a read-only fashion
    IReadOnlyCollection<string> Messages { get; }

    // Read-only properties to access event data
    IReadOnlyDictionary<int, TaskGatewayResponse> ResponseEvents { get; }

    IReadOnlyDictionary<int, TaskGatewayRequest> RequestEvents { get; }

    IReadOnlyDictionary<int, ControllerMonitor> ControllerMonitors { get; }

    IReadOnlyDictionary<int, StationMonitor> StationMonitors { get; }

    // Expose ProductionData in a read-only way
    ProductionData ProductionData { get; }

    // Methods to handle configuration and updates
    void ApplyConfiguration(ApplicationConfiguration configuration);

    void UpdateStationFromGatewayRequest(TaskGatewayRequest request);

    void UpdateStationFromGatewayResponse(TaskGatewayResponse response);

    void AddOrUpdateControllerFromGateway(int id, ControllerMonitor heartBeatControllerMonitor);

    // Expose methods for managing state changes
    void PushMessage(string user, string message);

    void AddOrUpdateTaskGatewayRequest(int id, TaskGatewayRequest requestEvent);

    void AddOrUpdateTaskGatewayResponse(int id, TaskGatewayResponse responseEvent);

    // Method to notify state change
}
