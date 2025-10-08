//[Move]
//CLAUDE
//Date: 26/08/2025
//Reason: [Test Relocation] - Moved to correct architectural layer based on its responsibility
namespace Application.UnitTests.Services;

/// <summary>
/// Represents the CacheManagerTests.
/// </summary>

public class CacheManagerTests
{
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);
    /// <summary>
    /// Executes GetOrRefreshAsync_ShouldReturnCachedData_WhenCacheIsValid operation.
    /// </summary>
    /// <returns>The result of GetOrRefreshAsync_ShouldReturnCachedData_WhenCacheIsValid.</returns>

    [Fact]
    public async Task GetOrRefreshAsync_ShouldReturnCachedData_WhenCacheIsValid()
    {
        // Arrange
        var cacheManager = new CacheManager<string>(_cacheDuration);
        var refreshFunc = Substitute.For<Func<Task<string>>>();
        refreshFunc().Returns("NewData");

        // Act - First call to cache the data
        var firstCall = await cacheManager.GetOrRefreshAsync(refreshFunc, cancellationToken: TestContext.Current.CancellationToken);
        // Act - Second call should return cached data
        var secondCall = await cacheManager.GetOrRefreshAsync(refreshFunc, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        firstCall.ShouldBe("NewData");
        secondCall.ShouldBe("NewData");
        await refreshFunc.Received(1).Invoke(); // Refresh function should only be called once
    }

    /// <summary>
    /// Executes GetOrRefreshAsync_ShouldCallRefreshFunc_WhenCacheIsExpired operation.
    /// </summary>
    /// <returns>The result of GetOrRefreshAsync_ShouldCallRefreshFunc_WhenCacheIsExpired.</returns>

    [Fact]
    public async Task GetOrRefreshAsync_ShouldCallRefreshFunc_WhenCacheIsExpired()
    {
        // Arrange
        var cacheManager = new CacheManager<string>(TimeSpan.FromSeconds(1)); // Short cache duration
        var refreshFunc = Substitute.For<Func<Task<string>>>();
        refreshFunc().Returns("NewData");

        // Act - First call to cache the data
        await cacheManager.GetOrRefreshAsync(refreshFunc, cancellationToken: TestContext.Current.CancellationToken);
        // Wait for the cache to expire
        await Task.Delay(2000, TestContext.Current.CancellationToken);
        // Act - Second call should trigger a refresh
        var secondCall = await cacheManager.GetOrRefreshAsync(refreshFunc, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        secondCall.ShouldBe("NewData");
        await refreshFunc.Received(2).Invoke(); // Refresh function should be called twice
    }

    /// <summary>
    /// Executes GetOrRefreshAsync_ShouldCallRefreshFunc_WhenForceRefreshIsTrue operation.
    /// </summary>
    /// <returns>The result of GetOrRefreshAsync_ShouldCallRefreshFunc_WhenForceRefreshIsTrue.</returns>

    [Fact]
    public async Task GetOrRefreshAsync_ShouldCallRefreshFunc_WhenForceRefreshIsTrue()
    {
        // Arrange
        var cacheManager = new CacheManager<string>(_cacheDuration);
        var refreshFunc = Substitute.For<Func<Task<string>>>();
        refreshFunc().Returns("NewData");

        // Act - First call to cache the data
        await cacheManager.GetOrRefreshAsync(refreshFunc, cancellationToken: TestContext.Current.CancellationToken);
        // Act - Second call with force refresh
        var secondCall = await cacheManager.GetOrRefreshAsync(refreshFunc, forceRefresh: true, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        secondCall.ShouldBe("NewData");
        await refreshFunc.Received(2).Invoke(); // Refresh function should be called twice
    }

    /// <summary>
    /// Executes InvalidateCache_ShouldClearCachedData_AndTriggerRefresh operation.
    /// </summary>

    [Fact]
    public async Task InvalidateCache_ShouldClearCachedData_AndTriggerRefresh()
    {
        // Arrange
        var cacheManager = new CacheManager<string>(_cacheDuration);
        var refreshFunc = Substitute.For<Func<Task<string>>>();
        refreshFunc().Returns("NewData");

        // Act - Cache data
        cacheManager.InvalidateCache();
        var result = await cacheManager.GetOrRefreshAsync(refreshFunc, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe("NewData");  // Since the cache was invalidated, the refreshFunc should be called and return "NewData"
    }

    /// <summary>
    /// Executes InvalidateCache_ShouldClearCachedData_AndTriggerRefreshWithNewData operation.
    /// </summary>
    [Fact]
    public async Task InvalidateCache_ShouldClearCachedData_AndTriggerRefreshWithNewData()
    {
        // Arrange
        var cacheManager = new CacheManager<string>(_cacheDuration);
        var refreshFunc = Substitute.For<Func<Task<string>>>();

        // First call - populate the cache with "FirstData"
        refreshFunc().Returns(Task.FromResult("FirstData"));
        var firstResult = await cacheManager.GetOrRefreshAsync(refreshFunc, cancellationToken: TestContext.Current.CancellationToken);

        // Assert first call result
        firstResult.ShouldBe("FirstData");

        // Invalidate the cache
        cacheManager.InvalidateCache();

        // Change the return value of the refresh function to simulate a refresh with new data
        refreshFunc().Returns(Task.FromResult("SecondData"));
        var secondResult = await cacheManager.GetOrRefreshAsync(refreshFunc, cancellationToken: TestContext.Current.CancellationToken);

        secondResult.ShouldNotBe(firstResult);
        // Assert second call result after cache invalidation and refresh
        secondResult.ShouldBe("SecondData");
    }

    /// <summary>
    /// Executes GetOrRefreshAsync_ShouldHandleConcurrencyCorrectly operation.
    /// </summary>
    /// <returns>The result of GetOrRefreshAsync_ShouldHandleConcurrencyCorrectly.</returns>

    [Fact]
    public async Task GetOrRefreshAsync_ShouldHandleConcurrencyCorrectly()
    {
        // Arrange
        var cacheManager = new CacheManager<string>(_cacheDuration);
        var refreshFunc = Substitute.For<Func<Task<string>>>();
        refreshFunc().Returns(async call =>
        {
            await Task.Delay(100, TestContext.Current.CancellationToken); // Simulate delay in fetching data
            return "NewData";
        });

        // Act - Simulate multiple concurrent requests
        var task1 = cacheManager.GetOrRefreshAsync(refreshFunc, cancellationToken: TestContext.Current.CancellationToken);
        var task2 = cacheManager.GetOrRefreshAsync(refreshFunc, cancellationToken: TestContext.Current.CancellationToken);
        var results = await Task.WhenAll(task1, task2);

        // Assert
        results.ShouldAllBe(r => r == "NewData", "because all concurrent cache calls should return the same value");
        await refreshFunc.Received(1).Invoke(); // Refresh function should only be called once
    }
}
