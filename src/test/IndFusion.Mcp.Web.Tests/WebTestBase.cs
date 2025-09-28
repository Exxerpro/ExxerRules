using IndFusion.Mcp.Web;
using IndFusion.Mcp.Web.Services;

namespace IndFusion.Mcp.Web.Tests;

/// <summary>
/// Base class for IndFusion.Mcp.Web tests providing a configured WebApplicationFactory
/// and HttpClient plus a hook to customize test services per test suite.
/// </summary>
public abstract class WebTestBase : IDisposable
{
    /// <summary>ASP.NET Core test host factory configured for the web app.</summary>
    protected WebApplicationFactory<Program> Factory { get; }
    /// <summary>HTTP client bound to the in-memory test server.</summary>
    protected HttpClient Client { get; }

    /// <summary>
    /// Initializes the in-memory test server and client.
    /// </summary>
    protected WebTestBase()
    {
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    ConfigureTestServices(services);
                });
            });

        Client = Factory.CreateClient();
    }

    /// <summary>
    /// Allows derived tests to override or add test-specific services.
    /// </summary>
    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
        // Override with test implementations
        services.AddScoped<IDashboardService, MockDashboardService>();
        services.AddScoped<IMetricsService, MockMetricsService>();
    }

    /// <summary>
    /// Disposes the test server and client resources.
    /// </summary>
    public virtual void Dispose()
    {
        Client?.Dispose();
        Factory?.Dispose();
        GC.SuppressFinalize(this);
    }
}
