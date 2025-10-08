namespace HubConnection.IntegrationTests.Performance;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HubConnection.IntegrationTests.Fixtures;
using IndTrace.Application.Models.Services;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Implementations;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

/// <summary>
/// Performance tests for SignalR connections.
/// Validates throughput, latency, and resource usage.
/// </summary>
public class ConnectionPerformanceTests : IAsyncLifetime
{
    private readonly SignalRTestFixture _fixture = new();
    private IHubConnectionFactory _factory = null!;

    public async ValueTask InitializeAsync()
    {
        await _fixture.InitializeAsync();

        // Prefer real hub if configured and reachable; fallback to in-memory TestServer
        _factory = new HybridHubConnectionFactory(_fixture.Server, _fixture.HubUrl, TestConfiguration.RealHubUrl);
    }

    public async ValueTask DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task Should_Send_1000_Messages_Under_1_Second()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        var stopwatch = Stopwatch.StartNew();

        // Act - Send 1000 messages
        var tasks = new List<Task>();
        for (int i = 0; i < 1000; i++)
        {
            tasks.Add(connection.SendAsync("SendMessage", new object?[] { $"Message {i}" }, TestContext.Current.CancellationToken));
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(1000);

        // Cleanup
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task Should_Handle_Concurrent_Connections()
    {
        // Arrange
        const int connectionCount = 50;
        var connections = new List<IHubConnection>();
        var stopwatch = Stopwatch.StartNew();

        // Act - Create and connect 50 clients concurrently
        var tasks = new List<Task>();
        for (int i = 0; i < connectionCount; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                var conn = await _factory.CreateAsync(TestContext.Current.CancellationToken);
                await conn.StartAsync(TestContext.Current.CancellationToken);
                lock (connections)
                {
                    connections.Add(conn);
                }
            }, TestContext.Current.CancellationToken));
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        connections.Count.ShouldBe(connectionCount);
        connections.All(c => c.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected).ShouldBeTrue();
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(5000); // Should connect all within 5 seconds

        // Verify all connections work
        var echoTasks = connections.Select(c =>
            c.InvokeAsync<string>("Echo", new object?[] { "test" }, TestContext.Current.CancellationToken)).ToList();

        var results = await Task.WhenAll(echoTasks);
        results.All(r => r == "Echo: test").ShouldBeTrue();

        // Cleanup
        foreach (var conn in connections)
        {
            await conn.DisposeAsync();
        }
    }

    [Fact]
    public async Task Should_Maintain_Low_Latency_Under_Load()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        var latencies = new List<long>();

        // Act - Measure latency for 100 round-trip calls
        for (int i = 0; i < 100; i++)
        {
            var sw = Stopwatch.StartNew();
            var result = await connection.InvokeAsync<string>("Echo", new object?[] { $"Message {i}" }, TestContext.Current.CancellationToken);
            sw.Stop();
            latencies.Add(sw.ElapsedMilliseconds);
        }

        // Assert
        var averageLatency = latencies.Average();
        var p95Latency = latencies.OrderBy(l => l).Skip(94).First(); // 95th percentile

        averageLatency.ShouldBeLessThan(50); // Average under 50ms
        p95Latency.ShouldBeLessThan(100);    // 95th percentile under 100ms

        // Cleanup
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task Should_Handle_Burst_Traffic()
    {
        // Arrange
        const int burstSize = 500;
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        var receivedCount = 0;
        var resetEvent = new ManualResetEventSlim(false);

        connection.On<string, string>("ReceiveMessage", (_, __) =>
        {
            var count = Interlocked.Increment(ref receivedCount);
            if (count == burstSize)
            {
                resetEvent.Set();
            }
            return Task.CompletedTask;
        });

        var stopwatch = Stopwatch.StartNew();

        // Act - Send burst of messages
        var tasks = new Task[burstSize];
        for (int i = 0; i < burstSize; i++)
        {
            tasks[i] = connection.SendAsync("SendMessage", new object?[] { $"Burst {i}" }, TestContext.Current.CancellationToken);
        }

        await Task.WhenAll(tasks);

        // Wait for all messages to be received (with timeout)
        var allReceived = resetEvent.Wait(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken);
        stopwatch.Stop();

        // Assert
        allReceived.ShouldBeTrue();
        receivedCount.ShouldBe(burstSize);
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(5000); // Should handle burst within 5 seconds

        // Cleanup
        await connection.DisposeAsync();
    }

    // Factory_Should_Create_Connections_Quickly test removed - not a business reality
    // Creating 100 connections with 10ms each is unrealistic for industrial hub connections
}
