using IndFusion.Mcp.Web;
using IndFusion.Mcp.Web.Services;

namespace IndFusion.Mcp.Web.Tests;

/// <summary>
/// Base class for IndFusion.Mcp.Web tests providing common web testing utilities
/// </summary>
public abstract class WebTestBase : IDisposable
{
    protected WebApplicationFactory<Program> Factory { get; }
    protected HttpClient Client { get; }

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

    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
        // Override with test implementations
        services.AddScoped<IDashboardService, MockDashboardService>();
        services.AddScoped<IMetricsService, MockMetricsService>();
    }

    public virtual void Dispose()
    {
        Client?.Dispose();
        Factory?.Dispose();
        GC.SuppressFinalize(this);
    }
}
