using System.Text;
using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Web;
using IndFusion.Mcp.Web.Controllers;
using IndFusion.Mcp.Web.Services;

namespace IndFusion.Mcp.Web.Tests.Integration;

/// <summary>
/// Integration tests covering key HTTP endpoints, content types, and service registration.
/// </summary>
public class WebApplicationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    /// <summary>
    /// Initializes a test server with test service overrides for dashboard and metrics.
    /// </summary>
    public WebApplicationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Override services for testing
                services.AddScoped<IDashboardService, TestDashboardService>();
                services.AddScoped<IMetricsService, TestMetricsService>();
            });

            builder.UseEnvironment("Testing");
        });

        _client = _factory.CreateClient();
    }

    /// <summary>
    /// GET / should return 200 with text/html content type.
    /// </summary>
    [Fact]
    public async Task Get_HomePage_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/", Xunit.TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString().ShouldContain("text/html");
    }

    /// <summary>
    /// GET /health should return 200 and body 'Healthy'.
    /// </summary>
    [Fact]
    public async Task Get_HealthCheck_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health", Xunit.TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(Xunit.TestContext.Current.CancellationToken);
        content.ShouldBe("Healthy");
    }

    /// <summary>
    /// GET /metrics should return 200 with non-empty payload (Prometheus format).
    /// </summary>
    [Fact]
    public async Task Get_PrometheusMetrics_ReturnsMetricsFormat()
    {
        // Act
        var response = await _client.GetAsync("/metrics", Xunit.TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(Xunit.TestContext.Current.CancellationToken);

        // Prometheus metrics should contain specific format
        // This is a basic check - in a real scenario, you'd validate specific metrics
        content.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    /// GET tools API should return JSON with tools collection.
    /// </summary>
    [Fact]
    public async Task Get_McpTools_ReturnsToolsList()
    {
        // Act
        var response = await _client.GetAsync("/api/Mcp/tools", Xunit.TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var content = await response.Content.ReadAsStringAsync(Xunit.TestContext.Current.CancellationToken);
        var toolsResponse = JsonSerializer.Deserialize<McpListToolsResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        toolsResponse.ShouldNotBeNull();
        toolsResponse!.Tools.ShouldNotBeNull();
    }

    /// <summary>
    /// GET server-info should return expected server values.
    /// </summary>
    [Fact]
    public async Task Get_McpServerInfo_ReturnsCorrectInfo()
    {
        // Act
        var response = await _client.GetAsync("/api/Mcp/server-info", Xunit.TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(Xunit.TestContext.Current.CancellationToken);
        var serverInfo = JsonSerializer.Deserialize<McpServerInfo>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        serverInfo.ShouldNotBeNull();
        serverInfo!.Name.ShouldBe("ExxerFactor.Mcp");
        serverInfo.Version.ShouldBe("1.0.0");
    }

    /// <summary>
    /// POST tools with invalid tool should return BadRequest and error payload.
    /// </summary>
    [Fact]
    public async Task Post_McpTools_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new McpToolCallRequest
        {
            ToolName = "nonexistent-tool", // This should fail gracefully
            Parameters = new Dictionary<string, JsonElement>()
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/Mcp/tools", content, Xunit.TestContext.Current.CancellationToken);

        // Assert
        // Should return BadRequest for nonexistent tool, but not crash
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

        var responseContent = await response.Content.ReadAsStringAsync(Xunit.TestContext.Current.CancellationToken);
        var errorResponse = JsonSerializer.Deserialize<McpErrorResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        errorResponse.ShouldNotBeNull();
        errorResponse!.Error.ShouldContain("Tool not found");
    }

    /// <summary>
    /// GET /tools should return 200 with text/html content type.
    /// </summary>
    [Fact]
    public async Task Get_ToolsPage_ReturnsSuccessAndCorrectContent()
    {
        // Act
        var response = await _client.GetAsync("/tools", Xunit.TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString().ShouldContain("text/html");
    }

    /// <summary>
    /// GET /metrics page endpoint should return 200 (distinct from Prometheus).
    /// </summary>
    [Fact]
    public async Task Get_MetricsPage_ReturnsSuccessAndCorrectContent()
    {
        // Act
        var response = await _client.GetAsync("/metrics", Xunit.TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        // Note: This might conflict with Prometheus /metrics endpoint
        // In a real app, you'd want different paths
    }

    /// <summary>
    /// Public pages should return 200 for various routes.
    /// </summary>
    [Theory]
    [InlineData("/")]
    [InlineData("/tools")]
    [InlineData("/monitoring")]
    [InlineData("/logs")]
    public async Task Get_PublicPages_ReturnsSuccess(string url)
    {
        // Act
        var response = await _client.GetAsync(url, Xunit.TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Verifies required services are registered in the DI container.
    /// </summary>
    [Fact]
    public void Application_ShouldRegisterRequiredServices()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Assert - Verify key services are registered
        var dashboardService = serviceProvider.GetService<IDashboardService>();
        dashboardService.ShouldNotBeNull();

        var ExxerFactoringService = serviceProvider.GetService<IExxerFactoringService>();
        ExxerFactoringService.ShouldNotBeNull();

        var metricsService = serviceProvider.GetService<IMetricsService>();
        metricsService.ShouldNotBeNull();
    }
}

// Test service implementations

