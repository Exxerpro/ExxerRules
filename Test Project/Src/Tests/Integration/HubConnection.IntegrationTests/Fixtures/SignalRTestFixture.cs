namespace HubConnection.IntegrationTests.Fixtures;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Models;

/// <summary>
/// Test fixture for SignalR integration tests.
/// Creates an in-memory test server with SignalR hub.
/// </summary>
public class SignalRTestFixture : IAsyncDisposable
{
    private IHost? _host;

    public TestServer Server { get; private set; } = null!;
    public string HubUrl { get; private set; } = "http://localhost:5000/testhub";

    public async Task InitializeAsync()
    {
        _host = new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddSignalR().AddHubOptions<TestHub>(o =>
                        {
                            o.EnableDetailedErrors = true;
                            o.MaximumReceiveMessageSize = 1024 * 1024; // allow large payload tests
                        });
                        services.AddSingleton<TestHub>();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHub<TestHub>("/testhub");
                        });
                    });
            })
            .Build();

        await _host.StartAsync();
        Server = _host.GetTestServer();

        // Set the correct URL after server is initialized
        var baseUri = Server.BaseAddress ?? new Uri("http://localhost:5000");
        HubUrl = new Uri(baseUri, "testhub").ToString();
    }

    public async ValueTask DisposeAsync()
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }
}

/// <summary>
/// Test SignalR hub for integration testing.
/// </summary>
public class TestHub : Hub
{
    private static int _connectionCount;

    public override async Task OnConnectedAsync()
    {
        _connectionCount++;
        await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connectionCount--;
        await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId, message);
    }

    public Task<string> Echo(string message)
    {
        return Task.FromResult($"Echo: {message}");
    }

    public Task<int> GetConnectionCount(object? _ = null)
    {
        return Task.FromResult(_connectionCount);
    }

    public async Task BroadcastData(string dataType, object data)
    {
        await Clients.All.SendAsync("DataReceived", dataType, data);
    }

    // Methods expected by HubConnectionInterfaceExtensions (InvokeAsync with two args)
    public async Task BroadcastMessageToClients(string source, string message)
    {
        // Mirror to the same event name so tests can optionally listen to it
        await Clients.All.SendAsync("BroadcastMessageToClients", source, message);
    }

    public Task BroadcastTaskGatewayRequest(int machineId, TaskGatewayRequest request)
    {
        // For tests we don't need to push to clients; just accept the call successfully
        return Task.CompletedTask;
    }

    public Task BroadcastTaskGatewayResponse(int machineId, TaskGatewayResponse response)
    {
        // Accept and no-op
        return Task.CompletedTask;
    }

    public async Task SendToConnection(string connectionId, string message)
    {
        await Clients.Client(connectionId).SendAsync("DirectMessage", message);
    }
}
