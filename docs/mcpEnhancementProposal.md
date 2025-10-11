# MCP Server Enhancement Proposal

## Overview
The IndFusion MCP server builder currently advertises complete tool/resource support, yet building a host through the fluent API fails at runtime and silently leaves WebSocket transport unimplemented. This proposal captures the concrete remediation steps needed to make the server production-ready.

## Findings and Recommendations

### 1. Ensure core service registration before MCP discovery
- **Location:** `src/code/IndFusion.Mcp.Server/Services/McpServerBuilder.cs:31`
- **Classes/Methods:** `McpServerBuilder.WithExxerFactoringTools`
- **Reasoning:** `ListToolsMcp` and `MetricsResourceMcp` (`src/code/IndFusion.Mcp.Server/Tools/ListToolsMcp.cs`, `src/code/IndFusion.Mcp.Server/Resources/MetricsResourceMcp.cs`) constructor-inject `IExxerFactoringService`. The current builder only calls `AddMcpServer()` before scanning the assembly, so `IExxerFactoringService` is never registered when consumers rely solely on the builder. Host construction succeeds, but the first request for either endpoint throws because the DI container cannot resolve the service.
- **Recommendation:** Invoke the existing `AddExxerFactorMcpServer()` extension before chaining MCP server discovery so all required abstractions are present.
- **Code Suggestion:**

```csharp
// before
_services
    .AddMcpServer()
    .WithToolsFromAssembly()
    .WithResourcesFromAssembly()
    .WithPromptsFromAssembly();
```

```csharp
// after
_services
    .AddExxerFactorMcpServer()
    .AddMcpServer()
    .WithToolsFromAssembly()
    .WithResourcesFromAssembly()
    .WithPromptsFromAssembly();
```

- **Behavioral Unit Test:** Add to `src/test/IndFusion.Mcp.Server.Tests/Services/McpServerBuilderTests.cs`.

```csharp
[Fact]
public async Task Build_ShouldResolve_ListToolsMcpDependencies()
{
    var host = Host.CreateDefaultBuilder()
        .CreateMcpServerBuilder()
        .WithExxerFactoringTools()
        .WithStdioTransport()
        .Build();

    using var scope = host.Services.CreateScope();
    var tool = scope.ServiceProvider.GetRequiredService<ListToolsMcp>();

    var result = await tool.ListToolsCommand();

    result.ShouldContain("extract-method");
}
```

### 2. Replace stubbed WebSocket transport with explicit behavior
- **Location:** `src/code/IndFusion.Mcp.Server/Services/McpServerBuilder.cs:55`
- **Classes/Methods:** `McpServerBuilder.WithWebSocketTransport`
- **Reasoning:** The method signature and default parameter imply full WebSocket support, yet the implementation returns immediately with a TODO comment. Consumers enabling WebSocket transport expect a running listener; instead, the host starts without one, creating a silent failure that is hard to diagnose.
- **Recommendation:** Wire the MCP HTTP transport (which provides WebSocket compatibility) by delegating to the SDK's `WithHttpTransport()` helper and hosting ASP.NET Core endpoints via `MapMcp`.
- **Code Suggestion:**

```csharp
public McpServerBuilder WithWebSocketTransport(int port = 8080)
{
    var builder = _services.AddMcpServer();
    builder.WithHttpTransport();

    _hostBuilder.ConfigureServices((_, services) => services.AddRouting());

    _hostBuilder.ConfigureWebHostDefaults(webHost =>
    {
        webHost.ConfigureKestrel(options => options.ListenAnyIP(port));
        webHost.Configure(app =>
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapMcp());
        });
    });

    return this;
}
```

- **Behavioral Unit Test:** Add to `src/test/IndFusion.Mcp.Server.Tests/Services/McpServerBuilderTests.cs`.

```csharp
[Fact]
public async Task Build_WithWebSocketTransport_ShouldResolve_ListToolsMcp()
{
    using var host = Host.CreateDefaultBuilder()
        .CreateMcpServerBuilder()
        .WithWebSocketTransport(0)
        .WithExxerFactoringTools()
        .Build();

    using var scope = host.Services.CreateScope();
    var tool = scope.ServiceProvider.GetRequiredService<ListToolsMcp>();

    var result = await tool.ListToolsCommand();

    result.ShouldContain("extract-method");
}
```

## Test Coverage Tracker
- Add the two unit tests above to `McpServerBuilderTests` to lock in DI wiring and transport expectations.
- After implementation of WebSocket transport, replace the exception test with assertions that the host registers the expected transport service (e.g., via `IHostedService` inspection or integration smoke test).

## Reference Implementation Blueprint
- **Upstream source:** `F:\Dynamic\ExxerAi\ExxerAI\code\SampleCode\mcp\ModelContextProtocol`. This folder contains the 0.3.0 MCP SDK that our projects already reference via NuGet.
- **Key APIs to consume:**
  - `Microsoft.Extensions.DependencyInjection.McpServerServiceCollectionExtensions.AddMcpServer(...)` to attach the SDK’s builder to our service collection (no custom `ServiceCollection` needed).
  - `Microsoft.Extensions.DependencyInjection.McpServerBuilderExtensions.WithToolsFromAssembly()`, `.WithResourcesFromAssembly()`, and `.WithPromptsFromAssembly()` for discovery.
  - `Microsoft.Extensions.DependencyInjection.McpServerBuilderExtensions.WithStdioServerTransport()` to wire the SDK’s `StdioServerTransport`, `SingleSessionMcpServerHostedService`, and `McpServerFactory` registrations.
  - Optionally, `Microsoft.Extensions.DependencyInjection.HttpMcpServerBuilderExtensions.WithHttpTransport()` (from `ModelContextProtocol.AspNetCore`) if/when we expose HTTP/WebSocket endpoints.
- **Implementation plan:**
  1. Update `McpServerBuilder.WithExxerFactoringTools()` to call `services.AddExxerFactorMcpServer()` followed by the SDK’s fluent builder (`AddMcpServer().WithToolsFromAssembly()...`) so we rely on existing discovery logic.
  2. Ensure `WithStdioTransport()` delegates entirely to `.AddMcpServer().WithStdioServerTransport()` rather than hand-rolled registrations.
  3. For WebSockets/HTTP, either guard with `NotSupportedException` or, when ready, call the SDK’s HTTP transport helpers and add any required options/provider glue.
  4. Keep the proposed unit tests; they verify that consuming the SDK APIs correctly populates the DI container and exposes working transports.
