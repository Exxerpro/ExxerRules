namespace HubConnection.IntegrationTests.Resilience;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HubConnection.IntegrationTests.Fixtures;
using IndTrace.Application.Models.Services;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Implementations;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

/// <summary>
/// Resilience tests for SignalR connections.
/// Tests connection recovery, error handling, and edge cases.
/// </summary>
public class ConnectionResilienceTests : IAsyncLifetime
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
    public async Task Should_Handle_Connection_Interruption_Gracefully()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);
        connection.State.ShouldBe(HubConnectionState.Connected);

        // Act - Force disconnect
        await connection.StopAsync(TestContext.Current.CancellationToken);

        // Assert
        connection.State.ShouldBe(HubConnectionState.Disconnected);

        // Should be able to reconnect
        await connection.StartAsync(TestContext.Current.CancellationToken);
        connection.State.ShouldBe(HubConnectionState.Connected);

        // Cleanup
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task Should_Fire_Connection_Events()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        var closedFired = false;
        Exception? closedException = null;

        connection.Closed += ex =>
        {
            closedFired = true;
            closedException = ex;
            return Task.CompletedTask;
        };

        // Act
        await connection.StartAsync(TestContext.Current.CancellationToken);
        await connection.StopAsync(TestContext.Current.CancellationToken);

        // Small delay to allow event to fire
        await Task.Delay(50, TestContext.Current.CancellationToken);

        // Assert
        closedFired.ShouldBeTrue();
        closedException.ShouldBeNull(); // Clean shutdown, no exception

        // Cleanup
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task Should_Handle_Invalid_Method_Calls_Gracefully()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        // Act & Assert - Call non-existent method
        await Should.ThrowAsync<Exception>(
            async () => await connection.InvokeAsync<string>("NonExistentMethod", Array.Empty<object?>(), TestContext.Current.CancellationToken));

        // Connection should still be functional
        connection.State.ShouldBe(HubConnectionState.Connected);

        // Should still be able to call valid methods
        var result = await connection.InvokeAsync<string>("Echo", new object?[] { "test" }, TestContext.Current.CancellationToken);
        result.ShouldBe("Echo: test");

        // Cleanup
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task Should_Handle_Operations_On_Disconnected_Connection()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        // Don't start the connection

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            async () => await connection.SendAsync("Echo", new object?[] { "test" }, TestContext.Current.CancellationToken));

        await Should.ThrowAsync<InvalidOperationException>(
            async () => await connection.InvokeAsync<string>("Echo", new object?[] { "test" }, TestContext.Current.CancellationToken));

        // Cleanup
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task Should_Handle_Handler_Exceptions_Gracefully()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        var exceptionHandlerCalled = false;
        var normalHandlerCalled = false;

        // Register handler that throws
        connection.On<string, string>("ReceiveMessage", (_, __) =>
        {
            exceptionHandlerCalled = true;
            throw new InvalidOperationException("Handler error");
        });

        // Register another handler that should still work
        connection.On<string, string>("ReceiveMessage", (_, __) =>
        {
            normalHandlerCalled = true;
            return Task.CompletedTask;
        });

        // Act - Send message that will trigger handlers
        await connection.SendAsync("SendMessage", new object?[] { "test" }, TestContext.Current.CancellationToken);
        await Task.Delay(100, TestContext.Current.CancellationToken); // Allow handlers to execute

        // Assert
        exceptionHandlerCalled.ShouldBeTrue();
        normalHandlerCalled.ShouldBeTrue();

        // Connection should still be working despite handler exception
        connection.State.ShouldBe(HubConnectionState.Connected);

        // Cleanup
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task Should_Handle_Concurrent_Operations_Safely()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        var exceptions = new List<Exception>();
        var results = new List<string>();

        // Act - Perform many concurrent operations
        var tasks = new List<Task>();
        for (int i = 0; i < 100; i++)
        {
            int index = i; // Capture for closure
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var result = await connection.InvokeAsync<string>("Echo", new object?[] { $"Message {index}" }, TestContext.Current.CancellationToken);
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
            }, TestContext.Current.CancellationToken));
        }

        await Task.WhenAll(tasks);

        // Assert
        exceptions.ShouldBeEmpty();
        results.Count.ShouldBe(100);

        // All results should be echoed correctly
        for (int i = 0; i < 100; i++)
        {
            results.ShouldContain($"Echo: Message {i}");
        }

        // Cleanup
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task Should_Handle_Large_Messages()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        // Create a large message (1MB string)
        var largeMessage = new string('A', 1024 * 1024);

        // Act
        var result = await connection.InvokeAsync<string>("Echo", new object?[] { largeMessage }, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe($"Echo: {largeMessage}");
        connection.State.ShouldBe(HubConnectionState.Connected);

        // Cleanup
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task Should_Handle_Rapid_Connect_Disconnect_Cycles()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);

        // Act & Assert - Rapid connect/disconnect cycles
        for (int i = 0; i < 10; i++)
        {
            await connection.StartAsync(TestContext.Current.CancellationToken);
            connection.State.ShouldBe(HubConnectionState.Connected);

            await connection.StopAsync(TestContext.Current.CancellationToken);
            connection.State.ShouldBe(HubConnectionState.Disconnected);
        }

        // Final verification
        await connection.StartAsync(TestContext.Current.CancellationToken);
        var result = await connection.InvokeAsync<string>("Echo", new object?[] { "Final test" }, TestContext.Current.CancellationToken);
        result.ShouldBe("Echo: Final test");

        // Cleanup
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task Should_Handle_Multiple_Dispose_Calls()
    {
        // Arrange
        var connection = await _factory.CreateAsync(TestContext.Current.CancellationToken);
        await connection.StartAsync(TestContext.Current.CancellationToken);

        // Act & Assert - Multiple dispose calls should not throw
        await Should.NotThrowAsync(async () =>
        {
            await connection.DisposeAsync();
            await connection.DisposeAsync();
            await connection.DisposeAsync();
        });
    }
}
