namespace HubConnection.IntegrationTests.Scenarios;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HubConnection.IntegrationTests.Fixtures;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Implementations;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

/// <summary>
/// Integration tests using real SignalR connections with TestServer.
/// Tests the full stack with actual SignalR protocol.
/// </summary>
public class RealSignalRConnectionTests : IAsyncLifetime
{
    private readonly SignalRTestFixture _fixture = new();
    private IHubConnection _connection = null!;
    private IHubConnectionFactory _factory = null!;

    public async ValueTask InitializeAsync()
    {
        await _fixture.InitializeAsync();

        // Prefer real hub if configured and reachable; fallback to in-memory TestServer
        _factory = new HybridHubConnectionFactory(_fixture.Server, _fixture.HubUrl, TestConfiguration.RealHubUrl);
        _connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task Connection_ShouldConnectToRealHub()
    {
        // Act
        await _connection.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        _connection.State.ShouldBe(HubConnectionState.Connected);
        _connection.ConnectionId.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Connection_ShouldSendAndReceiveMessages()
    {
        // Arrange
        await _connection.StartAsync(TestContext.Current.CancellationToken);
        var receivedMessages = new List<(string connectionId, string message)>();

        _connection.On<string, string>("ReceiveMessage", (connId, msg) =>
        {
            receivedMessages.Add((connId, msg));
            return Task.CompletedTask;
        });

        // Act
        await _connection.SendAsync("SendMessage", new object?[] { "Hello SignalR!" }, TestContext.Current.CancellationToken);
        await Task.Delay(300, TestContext.Current.CancellationToken); // Allow message to propagate

        // Assert
        receivedMessages.Count.ShouldBe(1);
        receivedMessages[0].connectionId.ShouldBe(_connection.ConnectionId);
        receivedMessages[0].message.ShouldBe("Hello SignalR!");
    }

    [Fact]
    public async Task Connection_ShouldInvokeMethodsWithResults()
    {
        // Arrange
        await _connection.StartAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await _connection.InvokeAsync<string>("Echo", new object?[] { "Test Message" }, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe("Echo: Test Message");
    }

    [Fact]
    public async Task Connection_ShouldHandleMultipleConnections()
    {
        // Arrange
        var connection1 = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        var connection2 = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        var connection3 = await _factory.CreateAsync(TestContext.Current.CancellationToken);

        // Act
        await connection1.StartAsync(TestContext.Current.CancellationToken);
        await connection2.StartAsync(TestContext.Current.CancellationToken);
        await connection3.StartAsync(TestContext.Current.CancellationToken);

        var connectionCount = await connection1.InvokeAsync<int>("GetConnectionCount", Array.Empty<object?>(), TestContext.Current.CancellationToken);

        // Assert
        connectionCount.ShouldBeGreaterThanOrEqualTo(3);

        // Cleanup
        await connection1.DisposeAsync();
        await connection2.DisposeAsync();
        await connection3.DisposeAsync();
    }

    [Fact]
    public async Task Connection_ShouldReceiveConnectionEvents()
    {
        // Arrange
        var connectedUsers = new List<string>();
        var disconnectedUsers = new List<string>();

        _connection.On<string>("UserConnected", userId =>
        {
            connectedUsers.Add(userId);
            return Task.CompletedTask;
        });

        _connection.On<string>("UserDisconnected", userId =>
        {
            disconnectedUsers.Add(userId);
            return Task.CompletedTask;
        });

        // Act
        await _connection.StartAsync(TestContext.Current.CancellationToken);
        var myConnectionId = _connection.ConnectionId;

        // Create and connect another client
        var otherConnection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await otherConnection.StartAsync(TestContext.Current.CancellationToken);
        var otherConnectionId = otherConnection.ConnectionId;

        await Task.Delay(100, TestContext.Current.CancellationToken); // Allow events to propagate

        // Disconnect the other client
        await otherConnection.StopAsync(TestContext.Current.CancellationToken);
        await Task.Delay(100, TestContext.Current.CancellationToken); // Allow disconnect event to propagate

        // Assert
        connectedUsers.ShouldContain(otherConnectionId!);
        disconnectedUsers.ShouldContain(otherConnectionId!);

        // Cleanup
        await otherConnection.DisposeAsync();
    }

    [Fact]
    public async Task Connection_ShouldHandleComplexDataTypes()
    {
        // Arrange
        await _connection.StartAsync(TestContext.Current.CancellationToken);
        var receivedData = new List<(string dataType, TestData data)>();

        _connection.On<string, TestData>("DataReceived", (type, data) =>
        {
            receivedData.Add((type, data));
            return Task.CompletedTask;
        });

        var testData = new TestData
        {
            Id = 123,
            Name = "Test Object",
            Values = new[] { 1.1, 2.2, 3.3 },
            Timestamp = DateTime.UtcNow
        };

        // Act
        await _connection.SendAsync("BroadcastData", new object?[] { "TestData", testData }, TestContext.Current.CancellationToken);
        await Task.Delay(300, TestContext.Current.CancellationToken); // Allow message to propagate

        // Assert
        receivedData.Count.ShouldBe(1);
        receivedData[0].dataType.ShouldBe("TestData");
        receivedData[0].data.Id.ShouldBe(123);
        receivedData[0].data.Name.ShouldBe("Test Object");
        receivedData[0].data.Values.ShouldBe(new[] { 1.1, 2.2, 3.3 });
    }

    [Fact]
    public async Task Connection_ShouldHandleReconnection()
    {
        // This test would require simulating network failures
        // For now, we test basic disconnect/reconnect

        // Arrange
        await _connection.StartAsync(TestContext.Current.CancellationToken);
        var originalConnectionId = _connection.ConnectionId;

        // Act
        await _connection.StopAsync(TestContext.Current.CancellationToken);
        _connection.State.ShouldBe(HubConnectionState.Disconnected);

        await _connection.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        _connection.State.ShouldBe(HubConnectionState.Connected);
        _connection.ConnectionId.ShouldNotBe(originalConnectionId); // New connection gets new ID
    }

    [Fact]
    public async Task Factory_ShouldCreateMultipleIndependentConnections()
    {
        // Arrange
        var connections = new List<IHubConnection>();

        // Act
        for (int i = 0; i < 5; i++)
        {
            var conn = await _factory.CreateAsync(TestContext.Current.CancellationToken);
            await conn.StartAsync(TestContext.Current.CancellationToken);
            connections.Add(conn);
        }

        // Assert
        connections.All(c => c.State == HubConnectionState.Connected).ShouldBeTrue();
        var connectionIds = connections.Select(c => c.ConnectionId).ToList();
        connectionIds.Distinct().Count().ShouldBe(5); // All unique IDs

        // Cleanup
        foreach (var conn in connections)
        {
            await conn.DisposeAsync();
        }
    }

    private class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double[] Values { get; set; } = Array.Empty<double>();
        public DateTime Timestamp { get; set; }
    }
}
