using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace IndFusion.Mcp.Tests.Integration.Infrastructure;

/// <summary>
/// Contract test base for validating service interface implementations.
/// Ensures that implementations properly fulfill their contracts.
/// </summary>
/// <typeparam name="TInterface">The service interface being tested.</typeparam>
/// <typeparam name="TImplementation">The service implementation being tested.</typeparam>
public abstract class ServiceContractTestBase<TInterface, TImplementation> : IITDDTestBase
    where TInterface : class
    where TImplementation : class, TInterface
{
    protected TInterface Service { get; private set; } = null!;

    protected ServiceContractTestBase(ITestOutputHelper testOutput) : base(testOutput)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        // Register the service implementation
        services.AddScoped<TInterface, TImplementation>();
        
        // Register any dependencies the service might need
        ConfigureServiceDependencies(services);
    }

    /// <summary>
    /// Configures dependencies for the service being tested.
    /// Override this method to register required dependencies.
    /// </summary>
    /// <param name="services">Service collection to configure.</param>
    protected abstract void ConfigureServiceDependencies(IServiceCollection services);

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Service = GetService<TInterface>();
        
        // Validate that the service is properly registered and resolved
        Service.ShouldNotBeNull($"Service of type {typeof(TInterface).Name} should be resolved");
        Service.ShouldBeOfType<TImplementation>($"Service should be of type {typeof(TImplementation).Name}");
        
        Logger.LogInformation("Service contract test initialized for {InterfaceType} -> {ImplementationType}", 
            typeof(TInterface).Name, typeof(TImplementation).Name);
    }

    /// <summary>
    /// Validates that the service implements all required interface methods.
    /// </summary>
    [Fact]
    public void Service_ShouldImplementAllInterfaceMethods()
    {
        var interfaceType = typeof(TInterface);
        var implementationType = typeof(TImplementation);
        
        var interfaceMethods = interfaceType.GetMethods()
            .Where(m => !m.IsSpecialName) // Exclude properties and events
            .ToList();
        
        foreach (var method in interfaceMethods)
        {
            var implementationMethod = implementationType.GetMethod(
                method.Name, 
                method.GetParameters().Select(p => p.ParameterType).ToArray());
            
            implementationMethod.ShouldNotBeNull(
                $"Implementation should have method {method.Name} with matching signature");
            
            Logger.LogDebug("Method validation passed for {MethodName}", method.Name);
        }
        
        Logger.LogInformation("All interface methods validated for {InterfaceType}", interfaceType.Name);
    }

    /// <summary>
    /// Validates that the service can be properly disposed.
    /// </summary>
    [Fact]
    public async Task Service_ShouldBeDisposable()
    {
        if (Service is IDisposable disposable)
        {
            // Should not throw when disposing
            Should.NotThrow(() => disposable.Dispose());
            Logger.LogDebug("Service disposal validation passed");
        }
        else if (Service is IAsyncDisposable asyncDisposable)
        {
            // Should not throw when disposing asynchronously
            await Should.NotThrowAsync(async () => await asyncDisposable.DisposeAsync());
            Logger.LogDebug("Service async disposal validation passed");
        }
        else
        {
            Logger.LogDebug("Service does not implement disposal interfaces");
        }
    }

    /// <summary>
    /// Validates that the service handles cancellation tokens properly.
    /// </summary>
    [Fact]
    public virtual async Task Service_ShouldHandleCancellationTokens()
    {
        // This is a base implementation that can be overridden by specific service tests
        // to validate cancellation token handling for async methods
        
        Logger.LogDebug("Cancellation token validation - base implementation (override for specific tests)");
        await Task.CompletedTask; // Placeholder for base implementation
    }

    /// <summary>
    /// Validates that the service handles null parameters appropriately.
    /// </summary>
    [Fact]
    public virtual async Task Service_ShouldHandleNullParameters()
    {
        // This is a base implementation that can be overridden by specific service tests
        // to validate null parameter handling
        
        Logger.LogDebug("Null parameter validation - base implementation (override for specific tests)");
        await Task.CompletedTask; // Placeholder for base implementation
    }
}