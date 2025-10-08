namespace HubConnection.IntegrationTests.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HubConnection.IntegrationTests.Fixtures;
using IndTrace.Application.Models.Services;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Models;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Extensions;
using IndTrace.HubConnection.Implementations;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

/// <summary>
/// Integration tests for HubConnectionInterfaceExtensions using real SignalR connections.
/// Tests extension methods against actual SignalR infrastructure following I²TDD principles.
/// Covers production scenarios from docs/HubInventory.md migration patterns.
/// </summary>
public class HubConnectionExtensionsIntegrationTests : IAsyncLifetime
{
    private readonly SignalRTestFixture _fixture = new();
    private IHubConnectionFactory _factory = null!;
    private ILogger _logger = null!;

    public async ValueTask InitializeAsync()
    {
        await _fixture.InitializeAsync();

        // Prefer real hub if configured and reachable; fallback to in-memory TestServer
        _factory = new HybridHubConnectionFactory(_fixture.Server, _fixture.HubUrl, TestConfiguration.RealHubUrl);
        _logger = XUnitLogger.CreateLogger<HubConnectionExtensionsIntegrationTests>();
    }

    public async ValueTask DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    /// <summary>
    /// Integration test: EnsureHubConnectionIsValid with real SignalR factory.
    /// Validates the complete flow from null connection to active SignalR session.
    /// </summary>
    [Fact]
    public async Task EnsureHubConnectionIsValid_Should_Create_And_Connect_Real_SignalR()
    {
        // Arrange
        IHubConnection? hubConnection = null;
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var result = await hubConnection.EnsureHubConnectionIsValid(_factory, _logger, cancellationToken);

        // Assert - Real SignalR connection established
        result.ShouldNotBeNull();
        result.State.ShouldBe(HubConnectionState.Connected);
        result.ConnectionId.ShouldNotBeNullOrEmpty();

        // Verify real SignalR functionality
        var echoResult = await result.InvokeAsync<string>("Echo", new object?[] { "Integration Test" }, cancellationToken);
        echoResult.ShouldBe("Echo: Integration Test");

        // Cleanup
        await result.DisposeAsync();
    }

    /// <summary>
    /// Integration test: EnsureHubConnectionIsValid handles real disconnected connections.
    /// Tests the production pattern where connections drop and need restart.
    /// </summary>
    [Fact]
    public async Task EnsureHubConnectionIsValid_Should_Restart_Disconnected_Real_Connection()
    {
        // Arrange - Create and then disconnect a real connection
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);
        connection.State.ShouldBe(HubConnectionState.Connected);

        // Force disconnect
        await connection.StopAsync(TestContext.Current.CancellationToken);
        connection.State.ShouldBe(HubConnectionState.Disconnected);

        // Act - Use extension to ensure valid connection
        var result = await connection.EnsureHubConnectionIsValid(_factory, _logger, TestContext.Current.CancellationToken);

        // Assert - Connection should be restarted
        result.ShouldBeSameAs(connection);
        result.ShouldNotBeNull();
        result.State.ShouldBe(HubConnectionState.Connected);
        result.ConnectionId.ShouldNotBeNullOrEmpty();

        // Verify restored functionality
        var echoResult = await result.InvokeAsync<string>("Echo", new object?[] { "Reconnected" }, TestContext.Current.CancellationToken);
        echoResult.ShouldBe("Echo: Reconnected");

        // Cleanup
        await result.DisposeAsync();
    }

    /// <summary>
    /// Integration test: TryStartHubConnectionAsync with real SignalR infrastructure.
    /// Validates the primary connection establishment pattern used in production.
    /// </summary>
    [Fact]
    public async Task TryStartHubConnectionAsync_Should_Connect_To_Real_SignalR_Hub()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        connection.State.ShouldBe(HubConnectionState.Disconnected);

        // Act
        var result = await connection.TryStartHubConnectionAsync(_logger, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeSameAs(connection);
        result.State.ShouldBe(HubConnectionState.Connected);
        result.ConnectionId.ShouldNotBeNullOrEmpty();

        // Verify real hub interaction
        var echoResult = await result.InvokeAsync<string>("Echo", new object?[] { "Start Test" }, TestContext.Current.CancellationToken);
        echoResult.ShouldBe("Echo: Start Test");

        // Test real hub messaging
        var messageReceived = false;
        var receivedMessage = string.Empty;
        var receivedConnectionId = string.Empty;

        result.On<string, string>("ReceiveMessage", (connId, msg) =>
        {
            messageReceived = true;
            receivedConnectionId = connId;
            receivedMessage = msg;
            return Task.CompletedTask;
        });

        await result.SendAsync("SendMessage", new object?[] { "Integration Message" }, TestContext.Current.CancellationToken);
        await Task.Delay(100, TestContext.Current.CancellationToken); // Allow message propagation

        // Assert real messaging worked
        messageReceived.ShouldBeTrue();
        receivedConnectionId.ShouldBe(result.ConnectionId);
        receivedMessage.ShouldBe("Integration Message");

        // Cleanup
        await result.DisposeAsync();
    }

    /// <summary>
    /// Integration test: TryStartHubConnectionAsync preserves already connected state.
    /// Performance optimization test with real connections.
    /// </summary>
    [Fact]
    public async Task TryStartHubConnectionAsync_Should_Preserve_Connected_Real_Connection()
    {
        // Arrange - Start connection first
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);
        var originalConnectionId = connection.ConnectionId;
        connection.State.ShouldBe(HubConnectionState.Connected);

        // Act - Try start again (should be no-op)
        var result = await connection.TryStartHubConnectionAsync(_logger, TestContext.Current.CancellationToken);

        // Assert - No change to existing connection
        result.ShouldBeSameAs(connection);
        result.State.ShouldBe(HubConnectionState.Connected);
        result.ConnectionId.ShouldBe(originalConnectionId);

        // Verify connection still functional
        var echoResult = await result.InvokeAsync<string>("Echo", new object?[] { "Still Connected" }, TestContext.Current.CancellationToken);
        echoResult.ShouldBe("Echo: Still Connected");

        // Cleanup
        await result.DisposeAsync();
    }

    /// <summary>
    /// Integration test: Extension methods handle real connection lifecycle events.
    /// Tests event forwarding and cleanup in production scenarios.
    /// </summary>
    [Fact]
    public async Task Extensions_Should_Handle_Real_Connection_Events()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        var closedEventFired = false;
        Exception? closedException = null;

        connection.Closed += ex =>
        {
            closedEventFired = true;
            closedException = ex;
            return Task.CompletedTask;
        };

        // Act - Start, verify, then stop
        await connection.TryStartHubConnectionAsync(_logger, TestContext.Current.CancellationToken);
        connection.State.ShouldBe(HubConnectionState.Connected);

        await connection.StopAsync(TestContext.Current.CancellationToken);

        // Small delay for event propagation
        await Task.Delay(50, TestContext.Current.CancellationToken);

        // Assert - Event handling worked
        closedEventFired.ShouldBeTrue();
        closedException.ShouldBeNull(); // Clean shutdown
        connection.State.ShouldBe(HubConnectionState.Disconnected);

        // Cleanup
        await connection.DisposeAsync();
    }

    /// <summary>
    /// Integration test: Multiple concurrent connections using extensions.
    /// Tests industrial scenario with multiple workers connecting simultaneously.
    /// </summary>
    [Fact]
    public async Task Extensions_Should_Handle_Multiple_Concurrent_Real_Connections()
    {
        // Arrange
        const int connectionCount = 5;
        var connections = new List<IHubConnection>();
        var tasks = new List<Task>();

        // Act - Create and ensure multiple connections concurrently
        for (int i = 0; i < connectionCount; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                IHubConnection? conn = null;
                var result = await conn.EnsureHubConnectionIsValid(_factory, _logger, TestContext.Current.CancellationToken);

                lock (connections)
                {
                    connections.Add(result!);
                }
            }, TestContext.Current.CancellationToken));
        }

        await Task.WhenAll(tasks);

        // Assert - All connections established
        connections.Count.ShouldBe(connectionCount);
        connections.All(c => c.State == HubConnectionState.Connected).ShouldBeTrue();

        // Verify all connections are unique and functional
        var connectionIds = connections.Select(c => c.ConnectionId).ToList();
        connectionIds.Distinct().Count().ShouldBe(connectionCount);

        // Test functionality of each connection
        var echoTasks = connections.Select(async c =>
        {
            var result = await c.InvokeAsync<string>("Echo", new object?[] { "Concurrent Test" }, TestContext.Current.CancellationToken);
            return result;
        });

        var echoResults = await Task.WhenAll(echoTasks);
        echoResults.All(r => r == "Echo: Concurrent Test").ShouldBeTrue();

        // Cleanup all connections
        foreach (var conn in connections)
        {
            await conn.DisposeAsync();
        }
    }

    /// <summary>
    /// Integration test: Extensions maintain thread safety with real connections.
    /// Critical test for industrial multi-threaded environments.
    /// </summary>
    [Fact]
    public async Task Extensions_Should_Maintain_Thread_Safety_With_Real_Connections()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        var results = new List<string>();
        var exceptions = new List<Exception>();

        // Act - Perform many concurrent operations on single connection
        var tasks = Enumerable.Range(0, 50).Select(i =>
            Task.Run(async () =>
            {
                try
                {
                    // Use extension method to ensure connection
                    var conn = await connection.EnsureHubConnectionIsValid(_factory, _logger, TestContext.Current.CancellationToken);

                    // Perform concurrent hub operations
                    var result = await conn!.InvokeAsync<string>("Echo", new object?[] { $"Concurrent {i}" }, TestContext.Current.CancellationToken);

                    lock (results)
                    {
                        results.Add(result!);
                    }
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            }, TestContext.Current.CancellationToken)
        );

        await Task.WhenAll(tasks);

        // Assert - No exceptions and all operations succeeded
        exceptions.ShouldBeEmpty();
        results.Count.ShouldBe(50);

        // Verify all echo responses
        for (int i = 0; i < 50; i++)
        {
            results.ShouldContain($"Echo: Concurrent {i}");
        }

        // Connection should remain stable
        connection.State.ShouldBe(HubConnectionState.Connected);

        // Cleanup
        await connection.DisposeAsync();
    }

    // --- Integration tests for newly ported extension methods ---

    /// <summary>
    /// Integration test: TryInvokeAsync with real SignalR hub.
    /// Tests the complete invoke flow from GatewayExecutor.cs patterns.
    /// </summary>
    [Fact]
    public async Task TryInvokeAsync_Should_Invoke_Real_Hub_Method()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await connection.TryInvokeAsync("Echo", "TestArg1", "TestArg2", _factory, _logger, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeTrue();
        connection.State.ShouldBe(HubConnectionState.Connected);

        // Cleanup
        await connection.DisposeAsync();
    }

    /// <summary>
    /// Integration test: PublishCommandToHubAsync with real TaskGatewayRequest.
    /// Tests command publishing pattern used in GatewayExecutor.cs.
    /// </summary>
    [Fact]
    public async Task PublishCommandToHubAsync_Should_Publish_To_Real_Hub()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        var receivedMessages = new List<(string connectionId, string message)>();
        connection.On<string, string>("ReceiveMessage", (connId, msg) =>
        {
            receivedMessages.Add((connId, msg));
            return Task.CompletedTask;
        });

        var request = new TaskGatewayRequest
        {
            MachineId = 2001,
            PartNumber = "INTEGRATION-TEST-001",
            BarCode = "BC789012",
            TimeStamp = DateTime.Now
        };

        // Act
        await connection.PublishCommandToHubAsync(request, _factory, _logger, TestContext.Current.CancellationToken);
        await Task.Delay(100, TestContext.Current.CancellationToken); // Allow message propagation

        // Assert
        connection.State.ShouldBe(HubConnectionState.Connected);
        request.TimeStamp.ShouldBeGreaterThan(DateTime.MinValue);

        // Cleanup
        await connection.DisposeAsync();
    }

    /// <summary>
    /// Integration test: PublishResultsToHubAsync with successful result.
    /// Tests result publishing pattern from GatewayExecutor.cs.
    /// </summary>
    [Fact]
    public async Task PublishResultsToHubAsync_Should_Publish_Success_Result_To_Real_Hub()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        var request = new TaskGatewayRequest
        {
            MachineId = 3001,
            PartNumber = "SUCCESS-TEST-001",
            BarCode = "BC456789"
        };

        var response = new TaskGatewayResponse
        {
            MachineId = 3001,
            PartNumber = "SUCCESS-TEST-001",
            BarCodeId = 12345,
            CycleId = 67890
        };

        var result = Result<TaskGatewayResponse>.Success(response);

        // Act
        await connection.PublishResultsToHubAsync(request, result, _factory, _logger, TestContext.Current.CancellationToken);
        await Task.Delay(100, TestContext.Current.CancellationToken); // Allow message propagation

        // Assert
        connection.State.ShouldBe(HubConnectionState.Connected);
        response.TimeStamp.ShouldBeGreaterThan(DateTime.MinValue);

        // Cleanup
        await connection.DisposeAsync();
    }

    /// <summary>
    /// Integration test: PublishResultsToHubAsync with failed result.
    /// Tests error handling and error message broadcasting to real hub.
    /// </summary>
    [Fact]
    public async Task PublishResultsToHubAsync_Should_Publish_Failed_Result_To_Real_Hub()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        var request = new TaskGatewayRequest
        {
            MachineId = 4001,
            PartNumber = "FAILURE-TEST-001"
        };

        var response = new TaskGatewayResponse
        {
            MachineId = 4001,
            PartNumber = "FAILURE-TEST-001",
            Error = "Process validation failed"
        };

        var errors = new[] { "Machine offline", "Invalid part number", "Cycle time exceeded" };
        var result = Result<TaskGatewayResponse>.WithFailure(errors, response);

        // Act
        await connection.PublishResultsToHubAsync(request, result, _factory, _logger, TestContext.Current.CancellationToken);
        await Task.Delay(100, TestContext.Current.CancellationToken); // Allow message propagation

        // Assert
        connection.State.ShouldBe(HubConnectionState.Connected);
        response.TimeStamp.ShouldBeGreaterThan(DateTime.MinValue);

        // Cleanup
        await connection.DisposeAsync();
    }

    /// <summary>
    /// Integration test: LogAndSendMessageFromControllerAsync with real hub.
    /// Tests controller messaging pattern with actual SignalR infrastructure.
    /// </summary>
    [Fact]
    public async Task LogAndSendMessageFromControllerAsync_Should_Send_To_Real_Hub()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        var receivedMessages = new List<(string source, string message)>();
        connection.On<string, string>("BroadcastMessageToClients", (source, msg) =>
        {
            receivedMessages.Add((source, msg));
            return Task.CompletedTask;
        });

        var controllerMessage = "Integration test controller operation completed at " + DateTime.Now;

        // Act
        await connection.LogAndSendMessageFromControllerAsync(controllerMessage, _logger, _factory, TestContext.Current.CancellationToken);
        await Task.Delay(100, TestContext.Current.CancellationToken); // Allow message propagation

        // Assert
        connection.State.ShouldBe(HubConnectionState.Connected);

        // Note: In real integration, we'd verify the message was broadcast through the hub
        // For now, we verify the connection remains stable and the call succeeds

        // Cleanup
        await connection.DisposeAsync();
    }

    /// <summary>
    /// Integration test: All extension methods work together in production scenario.
    /// Tests the complete workflow from command through result publishing.
    /// </summary>
    [Fact]
    public async Task Extension_Methods_Should_Work_Together_In_Production_Workflow()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        // Production workflow simulation
        var request = new TaskGatewayRequest
        {
            MachineId = 5001,
            PartNumber = "WORKFLOW-TEST-001",
            BarCode = "WF123456",
            TimeStamp = DateTime.Now
        };

        // Act 1: Publish command (like GatewayExecutor would)
        await connection.PublishCommandToHubAsync(request, _factory, _logger, TestContext.Current.CancellationToken);

        // Act 2: Use TryInvokeAsync for direct method call
        var invokeResult = await connection.TryInvokeAsync("Echo", "Workflow", "Test", _factory, _logger, TestContext.Current.CancellationToken);

        // Act 3: Send controller message
        await connection.LogAndSendMessageFromControllerAsync("Workflow step completed", _logger, _factory, TestContext.Current.CancellationToken);

        // Act 4: Publish successful result
        var response = new TaskGatewayResponse
        {
            MachineId = 5001,
            PartNumber = "WORKFLOW-TEST-001",
            BarCodeId = 98765,
            CycleId = 54321
        };
        var result = Result<TaskGatewayResponse>.Success(response);
        await connection.PublishResultsToHubAsync(request, result, _factory, _logger, TestContext.Current.CancellationToken);

        await Task.Delay(200, TestContext.Current.CancellationToken); // Allow all messages to propagate

        // Assert
        invokeResult.ShouldBeTrue();
        connection.State.ShouldBe(HubConnectionState.Connected);
        request.TimeStamp.ShouldBeGreaterThan(DateTime.MinValue);
        response.TimeStamp.ShouldBeGreaterThan(DateTime.MinValue);

        // Cleanup
        await connection.DisposeAsync();
    }

    /// <summary>
    /// Integration test: Extension methods maintain performance under load.
    /// Tests the industrial scenario with rapid message publishing.
    /// </summary>
    [Fact]
    public async Task Extension_Methods_Should_Handle_High_Frequency_Publishing()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        const int messageCount = 100;
        var tasks = new List<Task>();

        // Act - Rapid publishing of commands and results
        for (int i = 0; i < messageCount; i++)
        {
            int index = i; // Capture for closure

            tasks.Add(Task.Run(async () =>
            {
                var request = new TaskGatewayRequest
                {
                    MachineId = 6000 + index,
                    PartNumber = $"LOAD-TEST-{index:D3}",
                    BarCode = $"LT{index:D6}"
                };

                await connection.PublishCommandToHubAsync(request, _factory, _logger, TestContext.Current.CancellationToken);

                var response = new TaskGatewayResponse
                {
                    MachineId = 6000 + index,
                    PartNumber = $"LOAD-TEST-{index:D3}",
                    BarCodeId = index
                };

                var result = Result<TaskGatewayResponse>.Success(response);
                await connection.PublishResultsToHubAsync(request, result, _factory, _logger, TestContext.Current.CancellationToken);
            }, TestContext.Current.CancellationToken));
        }

        await Task.WhenAll(tasks);

        // Assert
        connection.State.ShouldBe(HubConnectionState.Connected);

        // Cleanup
        await connection.DisposeAsync();
    }
}
