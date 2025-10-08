namespace HubConnection.IntegrationTests.Fixtures;

using System;
using Microsoft.Extensions.Configuration;

public static class TestConfiguration
{
    private static readonly IConfigurationRoot _config = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
        .Build();

    /// <summary>
    /// Optional real hub URL for running tests against a live server.
    /// If null or unreachable, tests fall back to in-memory TestServer.
    /// </summary>
    public static string? RealHubUrl => _config["RealHub:Url"];

    /// <summary>
    /// When true, tests must use a live hub and should not fallback to in-memory.
    /// Can be set via appsettings or environment variable HUB_REQUIRE_LIVE=true.
    /// </summary>
    public static bool RequireLive =>
        bool.TryParse(Environment.GetEnvironmentVariable("HUB_REQUIRE_LIVE"), out var envFlag) && envFlag
        || bool.TryParse(_config["RealHub:RequireLive"], out var cfgFlag) && cfgFlag;

    /// <summary>
    /// Timeout (seconds) for live hub probe; defaults to 5s when not specified.
    /// </summary>
    public static int ProbeTimeoutSeconds =>
        int.TryParse(_config["RealHub:ProbeTimeoutSeconds"], out var s) ? s : 5;

    /// <summary>
    /// When true (default), tests will attempt to auto-start the Hub server out-of-process if not reachable.
    /// </summary>
    public static bool AutoStart =>
        // default true unless explicitly set to false
        !bool.TryParse(_config["RealHub:AutoStart"], out var cfg) || cfg;

    /// <summary>
    /// Optional project path to the Hub server csproj for dotnet run.
    /// Default points to the solution structure.
    /// </summary>
    public static string ProjectPath =>
        Environment.GetEnvironmentVariable("HUB_PROJECT_PATH")
        ?? _config["RealHub:ProjectPath"]
        ?? "Src/Code/Infrastructure/IndTrace.Hub/IndTrace.Hub.Server.csproj";

    /// <summary>
    /// Timeout (seconds) to wait for the Hub to become reachable after auto-start.
    /// </summary>
    public static int StartTimeoutSeconds =>
        int.TryParse(_config["RealHub:StartTimeoutSeconds"], out var st) ? st : 30;
}
