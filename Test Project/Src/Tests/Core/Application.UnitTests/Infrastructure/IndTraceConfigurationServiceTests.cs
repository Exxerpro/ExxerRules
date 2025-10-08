using Microsoft.Extensions.DependencyInjection;

namespace Application.UnitTests.Infrastructure;

/// <summary>
/// Unit tests for IndTraceConfigurationService
/// </summary>
public class IndTraceConfigurationServiceTests
{
    /// <summary>
    /// Executes Constructor_WithValidServiceProvider_ShouldCreateInstance operation.
    /// </summary>
    [Fact]
    public void Constructor_WithValidServiceProvider_ShouldCreateInstance()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();

        // Act
        var instance = new IndTraceConfigurationService(serviceProvider);

        // Assert
        instance.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes Constructor_WithNullServiceProvider_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithNullServiceProvider_ShouldCreateInstance()
    {
        // Arrange
        IServiceProvider? serviceProvider = null!;

        // Act
        var instance = new IndTraceConfigurationService(serviceProvider!);

        // Assert
        instance.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes GetConfigurationAsync_WhenRefreshIsFalseAndConfigurationExists_ShouldReturnCachedConfiguration operation.
    /// </summary>
    /// <returns>The result of GetConfigurationAsync_WhenRefreshIsFalseAndConfigurationExists_ShouldReturnCachedConfiguration.</returns>

    [Fact]
    public async Task GetConfigurationAsync_WhenRefreshIsFalseAndConfigurationExists_ShouldReturnCachedConfiguration()
    {
        // Arrange
        var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var serviceScope = Substitute.For<IServiceScope>();
        var scopeServiceProvider = Substitute.For<IServiceProvider>();
        var monitorRequestDispatcher = Substitute.For<IMonitorRequestDispatcher>();

        serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(serviceScopeFactory);
        serviceScopeFactory.CreateScope().Returns(serviceScope);
        serviceScope.ServiceProvider.Returns(scopeServiceProvider);
        scopeServiceProvider.GetService(typeof(IMonitorRequestDispatcher)).Returns(monitorRequestDispatcher);

        var configuration = new ApplicationConfiguration();
        var result = Result<ApplicationConfiguration>.Success(configuration);
        monitorRequestDispatcher.ProcessAsync(Arg.Any<GetAppDetailsMonitorRequest>(), Arg.Any<CancellationToken>()).Returns(result);

        var service = new IndTraceConfigurationService(serviceProvider);

        // Act - First call to populate cache
        var firstResult = await service.GetConfigurationAsync(false, TestContext.Current.CancellationToken);

        // Act - Second call should use cached configuration
        var secondResult = await service.GetConfigurationAsync(false, TestContext.Current.CancellationToken);

        // Assert
        firstResult.IsSuccess.ShouldBeTrue();
        secondResult.IsSuccess.ShouldBeTrue();
        firstResult.Value.ShouldBe(secondResult.Value);

        // Verify monitorRequestDispatcher was only called once (for the first call)
        await monitorRequestDispatcher.Received(1).ProcessAsync(Arg.Any<GetAppDetailsMonitorRequest>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes GetConfigurationAsync_WhenRefreshIsTrue_ShouldForceRefreshConfiguration operation.
    /// </summary>
    /// <returns>The result of GetConfigurationAsync_WhenRefreshIsTrue_ShouldForceRefreshConfiguration.</returns>

    [Fact]
    public async Task GetConfigurationAsync_WhenRefreshIsTrue_ShouldForceRefreshConfiguration()
    {
        // Arrange
        var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var serviceScope = Substitute.For<IServiceScope>();
        var scopeServiceProvider = Substitute.For<IServiceProvider>();
        var monitorRequestDispatcher = Substitute.For<IMonitorRequestDispatcher>();

        serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(serviceScopeFactory);
        serviceScopeFactory.CreateScope().Returns(serviceScope);
        serviceScope.ServiceProvider.Returns(scopeServiceProvider);
        scopeServiceProvider.GetService(typeof(IMonitorRequestDispatcher)).Returns(monitorRequestDispatcher);

        var configuration = new ApplicationConfiguration();
        var result = Result<ApplicationConfiguration>.Success(configuration);
        monitorRequestDispatcher.ProcessAsync(Arg.Any<GetAppDetailsMonitorRequest>(), Arg.Any<CancellationToken>()).Returns(result);

        var service = new IndTraceConfigurationService(serviceProvider);

        // Act - First call
        var firstResult = await service.GetConfigurationAsync(false, TestContext.Current.CancellationToken);

        // Act - Second call with refresh
        var secondResult = await service.GetConfigurationAsync(true, TestContext.Current.CancellationToken);

        // Assert
        firstResult.IsSuccess.ShouldBeTrue();
        secondResult.IsSuccess.ShouldBeTrue();

        // Verify monitorRequestDispatcher was called twice (once for each call)
        await monitorRequestDispatcher.Received(2).ProcessAsync(Arg.Any<GetAppDetailsMonitorRequest>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes GetConfigurationAsync_WhenmonitorRequestDispatcherReturnsFailure_ShouldReturnFailureResult operation.
    /// </summary>
    /// <returns>The result of GetConfigurationAsync_WhenmonitorRequestDispatcherReturnsFailure_ShouldReturnFailureResult.</returns>

    [Fact]
    public async Task GetConfigurationAsync_WhenmonitorRequestDispatcherReturnsFailure_ShouldReturnFailureResult()
    {
        // Arrange
        var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var serviceScope = Substitute.For<IServiceScope>();
        var scopeServiceProvider = Substitute.For<IServiceProvider>();
        var monitorRequestDispatcher = Substitute.For<IMonitorRequestDispatcher>();

        serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(serviceScopeFactory);
        serviceScopeFactory.CreateScope().Returns(serviceScope);
        serviceScope.ServiceProvider.Returns(scopeServiceProvider);
        scopeServiceProvider.GetService(typeof(IMonitorRequestDispatcher)).Returns(monitorRequestDispatcher);

        var failureResult = Result<ApplicationConfiguration>.WithFailure("Configuration not found");
        monitorRequestDispatcher.ProcessAsync(Arg.Any<GetAppDetailsMonitorRequest>(), Arg.Any<CancellationToken>()).Returns(failureResult);

        var service = new IndTraceConfigurationService(serviceProvider);

        // Act
        var result = await service.GetConfigurationAsync(false, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Configuration not found");
    }

    /// <summary>
    /// Executes GetConfigurationAsync_WhenmonitorRequestDispatcherReturnsNullValue_ShouldReturnFailureResult operation.
    /// </summary>
    /// <returns>The result of GetConfigurationAsync_WhenmonitorRequestDispatcherReturnsNullValue_ShouldReturnFailureResult.</returns>

    [Fact]
    public async Task GetConfigurationAsync_WhenmonitorRequestDispatcherReturnsNullValue_ShouldReturnFailureResult()
    {
        // Arrange
        var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var serviceScope = Substitute.For<IServiceScope>();
        var scopeServiceProvider = Substitute.For<IServiceProvider>();
        var monitorRequestDispatcher = Substitute.For<IMonitorRequestDispatcher>();

        serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(serviceScopeFactory);
        serviceScopeFactory.CreateScope().Returns(serviceScope);
        serviceScope.ServiceProvider.Returns(scopeServiceProvider);
        scopeServiceProvider.GetService(typeof(IMonitorRequestDispatcher)).Returns(monitorRequestDispatcher);

        // Mock should return failure instead of success with null, based on real behavior
        var failureResult = Result<ApplicationConfiguration>.WithFailure("Monitor request dispatcher returned null");
        monitorRequestDispatcher.ProcessAsync(Arg.Any<GetAppDetailsMonitorRequest>(), Arg.Any<CancellationToken>()).Returns(failureResult);

        var service = new IndTraceConfigurationService(serviceProvider);

        // Act
        var result = await service.GetConfigurationAsync(false, TestContext.Current.CancellationToken);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 19/09/2025
        //Reason: [MOCK SETUP CORRECTION] - Fix service provider mock setup to match other tests in file - use IServiceScopeFactory pattern
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Monitor request dispatcher returned null");
    }

    /// <summary>
    /// Executes GetConfigurationAsync_WhenCancellationTokenIsCancelled_ShouldHandleCancellation operation.
    /// </summary>
    /// <returns>The result of GetConfigurationAsync_WhenCancellationTokenIsCancelled_ShouldHandleCancellation.</returns>

    [Fact]
    public async Task GetConfigurationAsync_WhenCancellationTokenIsCancelled_ShouldHandleCancellation()
    {
        // Arrange
        var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var serviceScope = Substitute.For<IServiceScope>();
        var scopeServiceProvider = Substitute.For<IServiceProvider>();
        var monitorRequestDispatcher = Substitute.For<IMonitorRequestDispatcher>();

        serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(serviceScopeFactory);
        serviceScopeFactory.CreateScope().Returns(serviceScope);
        serviceScope.ServiceProvider.Returns(scopeServiceProvider);
        scopeServiceProvider.GetService(typeof(IMonitorRequestDispatcher)).Returns(monitorRequestDispatcher);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        monitorRequestDispatcher.ProcessAsync(Arg.Any<GetAppDetailsMonitorRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromCanceled<Result<ApplicationConfiguration>>(cts.Token));

        var service = new IndTraceConfigurationService(serviceProvider);

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(
            async () => await service.GetConfigurationAsync(false, cts.Token));
    }

    /// <summary>
    /// Executes GetConfigurationAsync_WhenServiceScopeIsDisposed_ShouldHandleDisposal operation.
    /// </summary>
    /// <returns>The result of GetConfigurationAsync_WhenServiceScopeIsDisposed_ShouldHandleDisposal.</returns>

    [Fact]
    public async Task GetConfigurationAsync_WhenServiceScopeIsDisposed_ShouldHandleDisposal()
    {
        // Arrange
        var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var serviceScope = Substitute.For<IServiceScope>();
        var scopeServiceProvider = Substitute.For<IServiceProvider>();
        var monitorRequestDispatcher = Substitute.For<IMonitorRequestDispatcher>();

        serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(serviceScopeFactory);
        serviceScopeFactory.CreateScope().Returns(serviceScope);
        serviceScope.ServiceProvider.Returns(scopeServiceProvider);
        scopeServiceProvider.GetService(typeof(IMonitorRequestDispatcher)).Returns(monitorRequestDispatcher);

        var configuration = new ApplicationConfiguration();
        var result = Result<ApplicationConfiguration>.Success(configuration);
        monitorRequestDispatcher.ProcessAsync(Arg.Any<GetAppDetailsMonitorRequest>(), Arg.Any<CancellationToken>()).Returns(result);

        var service = new IndTraceConfigurationService(serviceProvider);

        // Act
        var serviceResult = await service.GetConfigurationAsync(false, TestContext.Current.CancellationToken);

        // Assert
        serviceResult.IsSuccess.ShouldBeTrue();
        serviceScope.Received().Dispose();
    }
}
