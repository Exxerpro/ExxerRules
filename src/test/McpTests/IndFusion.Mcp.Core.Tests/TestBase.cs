namespace IndFusion.Mcp.Core.Tests;

using IndFusion.Mcp.Core.Extensions;

/// <summary>
/// Base class for IndFusion.Mcp.Core tests providing common setup and utilities
/// </summary>
public abstract class TestBase : IDisposable
{
    /// <summary>
    /// The root test <see cref="ServiceProvider"/> used to resolve services.
    /// </summary>
    protected ServiceProvider ServiceProvider { get; }

    /// <summary>
    /// The per-test <see cref="IServiceScope"/> created from the service provider.
    /// </summary>
    protected IServiceScope Scope { get; }

    /// <summary> ///  TestBase. /// </summary>
    protected TestBase()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
        Scope = ServiceProvider.CreateScope();
    }

    /// <summary> ///  ConfigureServices. /// </summary> /// <param name="services"></param>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Add configuration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(GetTestConfiguration())
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Add ExxerFactor.Mcp Core services
        services.AddExxerFactorMcpCore();
    }

    /// <summary> ///  GetTestConfiguration. /// </summary>
    protected virtual Dictionary<string, string?> GetTestConfiguration()
    {
        return new Dictionary<string, string?>
        {
            ["Logging:LogLevel:Default"] = "Debug",
            ["ExxerFactor.Mcp:TestMode"] = "true"
        };
    }

    /// <summary> /// Initializes a new instance of the class. /// </summary>
    protected T GetService<T>() where T : notnull
    {
        return Scope.ServiceProvider.GetRequiredService<T>();
    }

    /// <summary> /// Initializes a new instance of the class. /// </summary>
    protected T? GetOptionalService<T>()
    {
        return Scope.ServiceProvider.GetService<T>();
    }

    /// <summary>
    /// Gets a typed <see cref="ILogger{TCategoryName}"/> from the container.
    /// </summary>
    /// <typeparam name="T">The category type for the logger.</typeparam>
    /// <returns>The resolved <see cref="ILogger{TCategoryName}"/>.</returns>
    protected ILogger<T> GetLogger<T>()
    {
        return GetService<ILogger<T>>();
    }

    /// <summary>
    /// Dispose.
    /// </summary>
    /// <returns></returns>
    public virtual void Dispose()
    {
        Scope?.Dispose();
        ServiceProvider?.Dispose();
        GC.SuppressFinalize(this);
    }
}
