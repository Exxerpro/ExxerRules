using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace IndFusion.Mcp.Tests.Integration.Infrastructure;

/// <summary>
/// Base class for Integration Interface Test-Driven Development (IITDD) tests.
/// Provides common infrastructure for testing service contracts with real implementations.
/// </summary>
public abstract class IITDDTestBase : IAsyncLifetime
{
    protected readonly ITestOutputHelper TestOutput;
    protected readonly IHost Host;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ILogger Logger;

    protected IITDDTestBase(ITestOutputHelper testOutput)
    {
        TestOutput = testOutput;
        Host = CreateHost();
        ServiceProvider = Host.Services;
        Logger = ServiceProvider.GetRequiredService<ILogger<object>>();
    }

    /// <summary>
    /// Creates the test host with service registrations for IITDD testing.
    /// Override this method to customize service registrations for specific test scenarios.
    /// </summary>
    /// <returns>Configured host for testing.</returns>
    protected virtual IHost CreateHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureServices)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddXunit(TestOutput);
                logging.SetMinimumLevel(LogLevel.Debug);
            })
            .Build();
    }

    /// <summary>
    /// Configures services for IITDD testing.
    /// Override this method to register specific services and their implementations.
    /// </summary>
    /// <param name="services">Service collection to configure.</param>
    protected abstract void ConfigureServices(IServiceCollection services);

    /// <summary>
    /// Initializes the test environment.
    /// </summary>
    public virtual async Task InitializeAsync()
    {
        await Host.StartAsync();
        Logger.LogInformation("IITDD test initialized for {TestType}", GetType().Name);
    }

    /// <summary>
    /// Cleans up the test environment.
    /// </summary>
    public virtual async Task DisposeAsync()
    {
        Logger.LogInformation("IITDD test cleanup for {TestType}", GetType().Name);
        await Host.StopAsync();
        Host.Dispose();
    }

    /// <summary>
    /// Gets a service from the service provider with proper error handling.
    /// </summary>
    /// <typeparam name="T">Type of service to retrieve.</typeparam>
    /// <returns>Service instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when service is not registered.</exception>
    protected T GetService<T>() where T : notnull
    {
        var service = ServiceProvider.GetService<T>();
        if (service == null)
        {
            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");
        }
        return service;
    }

    /// <summary>
    /// Gets a service from the service provider with proper error handling.
    /// </summary>
    /// <param name="serviceType">Type of service to retrieve.</param>
    /// <returns>Service instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when service is not registered.</exception>
    protected object GetService(Type serviceType)
    {
        var service = ServiceProvider.GetService(serviceType);
        if (service == null)
        {
            throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered.");
        }
        return service;
    }

    /// <summary>
    /// Validates that all required services are registered and can be resolved.
    /// </summary>
    /// <param name="serviceTypes">Types of services to validate.</param>
    protected void ValidateServiceRegistrations(params Type[] serviceTypes)
    {
        foreach (var serviceType in serviceTypes)
        {
            var service = ServiceProvider.GetService(serviceType);
            service.ShouldNotBeNull($"Service of type {serviceType.Name} should be registered");
            Logger.LogDebug("Service validation passed for {ServiceType}", serviceType.Name);
        }
    }

    /// <summary>
    /// Creates a test cancellation token with timeout.
    /// </summary>
    /// <param name="timeoutMs">Timeout in milliseconds.</param>
    /// <returns>Cancellation token with timeout.</returns>
    protected CancellationToken CreateTestCancellationToken(int timeoutMs = 30000)
    {
        var cts = new CancellationTokenSource(timeoutMs);
        return cts.Token;
    }
}