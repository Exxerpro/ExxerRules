namespace HubConnection.IntegrationTests.Fixtures;

using System;
using System.Threading;
using System.Threading.Tasks;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Implementations;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;

/// <summary>
/// Test-specific hub connection factory that uses TestServer's HttpClient
/// for in-memory SignalR connections during integration testing.
/// </summary>
public class TestServerHubConnectionFactory : IHubConnectionFactory
{
    private readonly TestServer _testServer;
    private readonly string _hubUrl;

    public TestServerHubConnectionFactory(TestServer testServer, string hubUrl)
    {
        _testServer = testServer ?? throw new ArgumentNullException(nameof(testServer));
        _hubUrl = hubUrl ?? throw new ArgumentNullException(nameof(hubUrl));
    }

    public Task<IHubConnection> CreateAsync(CancellationToken cancellationToken = default)
    {
        // Create the hub connection using TestServer's HttpClient for in-memory transport
        var connection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                // Force LongPolling so TestServer's HttpMessageHandler is used (no WebSockets)
                options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                options.HttpMessageHandlerFactory = _ => _testServer.CreateHandler();
            })
            .Build();

        IHubConnection adapted = new SignalRHubConnectionAdapter(connection);
        return Task.FromResult(adapted);
    }
}
