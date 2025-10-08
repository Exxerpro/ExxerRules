namespace HubConnection.IntegrationTests.Fixtures;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Implementations;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;

/// <summary>
/// Hybrid factory: prefers a real hub URL if reachable; otherwise falls back to TestServer in-memory hub.
/// </summary>
public class HybridHubConnectionFactory : IHubConnectionFactory
{
    private readonly TestServer _testServer;
    private readonly string _inMemoryHubUrl;
    private readonly string? _realHubUrl;
    private readonly ILogger _logger;

    public HybridHubConnectionFactory(TestServer testServer, string inMemoryHubUrl, string? realHubUrl)
    {
        _testServer = testServer ?? throw new ArgumentNullException(nameof(testServer));
        _inMemoryHubUrl = inMemoryHubUrl ?? throw new ArgumentNullException(nameof(inMemoryHubUrl));
        _realHubUrl = string.IsNullOrWhiteSpace(realHubUrl) ? null : realHubUrl;
        _logger = XUnitLogger.CreateLogger<HybridHubConnectionFactory>();
    }

    public async Task<IHubConnection> CreateAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating hub connection. Real hub URL: {RealHubUrl}, In-memory URL: {InMemoryUrl}", _realHubUrl ?? "not configured", _inMemoryHubUrl);

        var useReal = await IsRealHubReachableAsync(_realHubUrl, cancellationToken).ConfigureAwait(false);

        if (!useReal && TestConfiguration.AutoStart)
        {
            // Attempt to auto-start the real hub out-of-process
            _logger.LogInformation("Attempting to auto-start real hub...");
            var projectPath = TestConfiguration.ProjectPath;
            var startOk = await LiveHubProcessManager.EnsureStartedAsync(_realHubUrl, projectPath, TestConfiguration.StartTimeoutSeconds, cancellationToken).ConfigureAwait(false);
            if (startOk)
            {
                useReal = await IsRealHubReachableAsync(_realHubUrl, cancellationToken).ConfigureAwait(false);
            }
        }

        HubConnection connection;
        if (useReal)
        {
            _logger.LogInformation("Using REAL hub server at: {HubUrl}", _realHubUrl);
            connection = new HubConnectionBuilder()
                .WithUrl(_realHubUrl!)
                .WithAutomaticReconnect()
                .Build();
        }
        else
        {
            if (TestConfiguration.RequireLive)
            {
                throw new InvalidOperationException($"HUB_REQUIRE_LIVE is enabled or RealHub:RequireLive=true, but live hub not reachable at '{_realHubUrl}'.");
            }
            _logger.LogInformation("Using IN-MEMORY TestServer hub at: {HubUrl}", _inMemoryHubUrl);
            connection = new HubConnectionBuilder()
                .WithUrl(_inMemoryHubUrl, options =>
                {
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                    options.HttpMessageHandlerFactory = _ => _testServer.CreateHandler();
                })
                .WithAutomaticReconnect()
                .Build();
        }

        return new SignalRHubConnectionAdapter(connection);
    }

    private async Task<bool> IsRealHubReachableAsync(string? hubUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(hubUrl))
        {
            _logger.LogDebug("No real hub URL configured");
            return false;
        }

        try
        {
            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var seconds = TestConfiguration.ProbeTimeoutSeconds;
            using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(seconds) };

            // Probe SignalR negotiate endpoint to validate availability
            var negotiate = hubUrl.TrimEnd('/') + "/negotiate?negotiateVersion=1";
            _logger.LogDebug("Checking real hub availability at: {NegotiateUrl}", negotiate);

            using var resp = await client.GetAsync(negotiate, cancellationToken).ConfigureAwait(false);
            var isReachable = resp.IsSuccessStatusCode;

            _logger.LogInformation("Real hub server reachability check: {IsReachable} (Status: {StatusCode})", isReachable, resp.StatusCode);
            return isReachable;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to reach real hub server at {HubUrl}", hubUrl);
            return false;
        }
    }
}
