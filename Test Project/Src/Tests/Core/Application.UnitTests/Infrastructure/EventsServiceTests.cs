namespace Application.UnitTests.Infrastructure;

/// <summary>
/// Unit tests for EventsService
/// </summary>
public class EventsServiceTests
{
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var guiCommandDispatcher = Substitute.For<IMonitorRequestDispatcher>();
        var cache = Substitute.For<ICacheService>();

        // Act
        var instance = new EventsService(guiCommandDispatcher, cache);

        // Assert
        instance.ShouldNotBeNull();
        instance.ActualPage.ShouldBe(1);
        instance.PageSize.ShouldBe(100);
    }

    /// <summary>
    /// Executes Constructor_WithNullParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithNullParameters_ShouldCreateInstance()
    {
        // Arrange
        IMonitorRequestDispatcher? guiCommandDispatcher = null!;
        ICacheService? cache = null!;

        // Act
        var instance = new EventsService(guiCommandDispatcher!, cache!);

        // Assert
        instance.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes Properties_WhenSet_ShouldReturnCorrectValues operation.
    /// </summary>

    [Fact]
    public void Properties_WhenSet_ShouldReturnCorrectValues()
    {
        // Arrange
        var guiCommandDispatcher = Substitute.For<IMonitorRequestDispatcher>();
        var cache = Substitute.For<ICacheService>();
        var instance = new EventsService(guiCommandDispatcher, cache);

        // Act
        instance.ActualPage = 5;
        instance.PageSize = 50;

        // Assert
        instance.ActualPage.ShouldBe(5);
        instance.PageSize.ShouldBe(50);
    }

    /// <summary>
    /// Executes GetNextEventsAsync_WhenCalled_ShouldIncrementPageAndReturnEvents operation.
    /// </summary>
    /// <returns>The result of GetNextEventsAsync_WhenCalled_ShouldIncrementPageAndReturnEvents.</returns>

    [Fact]
    public async Task GetNextEventsAsync_WhenCalled_ShouldIncrementPageAndReturnEvents()
    {
        // Arrange
        var guiCommandDispatcher = Substitute.For<IMonitorRequestDispatcher>();
        var cache = Substitute.For<ICacheService>();
        var requests = new List<TaskGatewayRequest>();
        var responses = new List<TaskGatewayResponse>();
        var eventsListVm = new EventsListVm(requests, responses);
        var queryResult = Result<EventsListVm>.Success(eventsListVm);

        guiCommandDispatcher.QueryAsync(Arg.Any<GetEventsListQuery>(), Arg.Any<CancellationToken>())
            .Returns(queryResult);

        cache.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<CancellationToken, Task<EventsListVm>>>(),
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>())
            .Returns(eventsListVm);

        var service = new EventsService(guiCommandDispatcher, cache);

        // Act
        var result = await service.GetNextEventsAsync(TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(eventsListVm);
    }

    /// <summary>
    /// Executes GetNextEventsAsync_WhenCalledMultipleTimes_ShouldIncrementPageCorrectly operation.
    /// </summary>
    /// <returns>The result of GetNextEventsAsync_WhenCalledMultipleTimes_ShouldIncrementPageCorrectly.</returns>

    [Fact]
    public async Task GetNextEventsAsync_WhenCalledMultipleTimes_ShouldIncrementPageCorrectly()
    {
        // Arrange
        var guiCommandDispatcher = Substitute.For<IMonitorRequestDispatcher>();
        var cache = Substitute.For<ICacheService>();
        var requests = new List<TaskGatewayRequest>();
        var responses = new List<TaskGatewayResponse>();
        var eventsListVm = new EventsListVm(requests, responses);
        var queryResult = Result<EventsListVm>.Success(eventsListVm);

        guiCommandDispatcher.QueryAsync(Arg.Any<GetEventsListQuery>(), Arg.Any<CancellationToken>())
            .Returns(queryResult);

        cache.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<CancellationToken, Task<EventsListVm>>>(),
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>())
            .Returns(eventsListVm);

        var service = new EventsService(guiCommandDispatcher, cache);

        // Act
        await service.GetNextEventsAsync(TestContext.Current.CancellationToken); // Page 1 -> 2
        await service.GetNextEventsAsync(TestContext.Current.CancellationToken); // Page 2 -> 3
        await service.GetNextEventsAsync(TestContext.Current.CancellationToken); // Page 3 -> 4

        // Assert
        service.ActualPage.ShouldBe(4);
    }

    /// <summary>
    /// Executes GetPreviousEventsAsync_WhenOnFirstPage_ShouldReturnFailure operation.
    /// </summary>
    /// <returns>The result of GetPreviousEventsAsync_WhenOnFirstPage_ShouldReturnFailure.</returns>

    [Fact]
    public async Task GetPreviousEventsAsync_WhenOnFirstPage_ShouldReturnFailure()
    {
        // Arrange
        var guiCommandDispatcher = Substitute.For<IMonitorRequestDispatcher>();
        var cache = Substitute.For<ICacheService>();
        var service = new EventsService(guiCommandDispatcher, cache);

        // Act
        var result = await service.GetPreviousEventsAsync(TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Value.ShouldBeNull();
        service.ActualPage.ShouldBe(1); // Should remain unchanged
    }

    /// <summary>
    /// Executes GetPreviousEventsAsync_WhenNotOnFirstPage_ShouldDecrementPageAndReturnEvents operation.
    /// </summary>
    /// <returns>The result of GetPreviousEventsAsync_WhenNotOnFirstPage_ShouldDecrementPageAndReturnEvents.</returns>

    [Fact]
    public async Task GetPreviousEventsAsync_WhenNotOnFirstPage_ShouldDecrementPageAndReturnEvents()
    {
        // Arrange
        var guiCommandDispatcher = Substitute.For<IMonitorRequestDispatcher>();
        var cache = Substitute.For<ICacheService>();
        var requests = new List<TaskGatewayRequest>();
        var responses = new List<TaskGatewayResponse>();
        var eventsListVm = new EventsListVm(requests, responses);
        var queryResult = Result<EventsListVm>.Success(eventsListVm);

        guiCommandDispatcher.QueryAsync(Arg.Any<GetEventsListQuery>(), Arg.Any<CancellationToken>())
            .Returns(queryResult);

        cache.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<CancellationToken, Task<EventsListVm>>>(),
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>())
            .Returns(eventsListVm);

        var service = new EventsService(guiCommandDispatcher, cache);
        service.ActualPage = 3; // Set to page 3

        // Act
        var result = await service.GetPreviousEventsAsync(TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(eventsListVm);
        service.ActualPage.ShouldBe(2); // Should be decremented from 3 to 2
    }

    /// <summary>
    /// Executes GetPreviousEventsAsync_WhenCalledMultipleTimes_ShouldDecrementPageCorrectly operation.
    /// </summary>
    /// <returns>The result of GetPreviousEventsAsync_WhenCalledMultipleTimes_ShouldDecrementPageCorrectly.</returns>

    [Fact]
    public async Task GetPreviousEventsAsync_WhenCalledMultipleTimes_ShouldDecrementPageCorrectly()
    {
        // Arrange
        var guiCommandDispatcher = Substitute.For<IMonitorRequestDispatcher>();
        var cache = Substitute.For<ICacheService>();
        var requests = new List<TaskGatewayRequest>();
        var responses = new List<TaskGatewayResponse>();
        var eventsListVm = new EventsListVm(requests, responses);
        var queryResult = Result<EventsListVm>.Success(eventsListVm);

        guiCommandDispatcher.QueryAsync(Arg.Any<GetEventsListQuery>(), Arg.Any<CancellationToken>())
            .Returns(queryResult);

        cache.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<CancellationToken, Task<EventsListVm>>>(),
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>())
            .Returns(eventsListVm);

        var service = new EventsService(guiCommandDispatcher, cache);
        service.ActualPage = 5; // Set to page 5

        // Act
        await service.GetPreviousEventsAsync(TestContext.Current.CancellationToken); // Page 5 -> 4
        await service.GetPreviousEventsAsync(TestContext.Current.CancellationToken); // Page 4 -> 3
        await service.GetPreviousEventsAsync(TestContext.Current.CancellationToken); // Page 3 -> 2

        // Assert
        service.ActualPage.ShouldBe(2);
    }

    /// <summary>
    /// Executes GetPreviousEventsAsync_WhenOnPageTwo_ShouldAllowGoingToPageOne operation.
    /// </summary>
    /// <returns>The result of GetPreviousEventsAsync_WhenOnPageTwo_ShouldAllowGoingToPageOne.</returns>

    [Fact]
    public async Task GetPreviousEventsAsync_WhenOnPageTwo_ShouldAllowGoingToPageOne()
    {
        // Arrange
        var guiCommandDispatcher = Substitute.For<IMonitorRequestDispatcher>();
        var cache = Substitute.For<ICacheService>();
        var requests = new List<TaskGatewayRequest>();
        var responses = new List<TaskGatewayResponse>();
        var eventsListVm = new EventsListVm(requests, responses);
        var queryResult = Result<EventsListVm>.Success(eventsListVm);

        guiCommandDispatcher.QueryAsync(Arg.Any<GetEventsListQuery>(), Arg.Any<CancellationToken>())
            .Returns(queryResult);

        cache.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<CancellationToken, Task<EventsListVm>>>(),
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>())
            .Returns(eventsListVm);

        var service = new EventsService(guiCommandDispatcher, cache);
        service.ActualPage = 2; // Set to page 2

        // Act
        var result = await service.GetPreviousEventsAsync(TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(eventsListVm);
        service.ActualPage.ShouldBe(1); // Should be decremented from 2 to 1
    }

    /// <summary>
    /// Executes GetNextEventsAsync_WhenCancellationTokenIsCancelled_ShouldHandleCancellation operation.
    /// </summary>
    /// <returns>The result of GetNextEventsAsync_WhenCancellationTokenIsCancelled_ShouldHandleCancellation.</returns>

    [Fact]
    public async Task GetNextEventsAsync_WhenCancellationTokenIsCancelled_ShouldHandleCancellation()
    {
        // Arrange
        var guiCommandDispatcher = Substitute.For<IMonitorRequestDispatcher>();
        var cache = Substitute.For<ICacheService>();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Setup cache to throw OperationCanceledException when called with the cancelled token
        cache.GetOrSetAsync<EventsListVm>(
            Arg.Any<string>(),
            Arg.Any<Func<CancellationToken, Task<EventsListVm>>>(),
            Arg.Any<TimeSpan?>(),
            Arg.Is<CancellationToken>(t => t.IsCancellationRequested))
            .Returns((Task<EventsListVm?>)Task.FromException<EventsListVm?>(new OperationCanceledException()));

        var service = new EventsService(guiCommandDispatcher, cache);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await service.GetNextEventsAsync(cts.Token));

        // Verify cache was called with the cancelled token
        await cache.Received(1).GetOrSetAsync<EventsListVm>(
            Arg.Any<string>(),
            Arg.Any<Func<CancellationToken, Task<EventsListVm>>>(),
            Arg.Any<TimeSpan?>(),
            Arg.Is<CancellationToken>(t => t.IsCancellationRequested));

        // Correct NSubstitute usage for generic method
        await guiCommandDispatcher.DidNotReceiveWithAnyArgs()
            .QueryAsync<EventsListVm>(Arg.Any<IMonitorRequest<EventsListVm>>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes GetPreviousEventsAsync_WhenCancellationTokenIsCancelled_ShouldHandleCancellation operation.
    /// </summary>
    /// <returns>The result of GetPreviousEventsAsync_WhenCancellationTokenIsCancelled_ShouldHandleCancellation.</returns>

    [Fact]
    public async Task GetPreviousEventsAsync_WhenCancellationTokenIsCancelled_ShouldHandleCancellation()
    {
        // Arrange
        var guiCommandDispatcher = Substitute.For<IMonitorRequestDispatcher>();
        var cache = Substitute.For<ICacheService>();
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var service = new EventsService(guiCommandDispatcher, cache);
        service.ActualPage = 2; // Set to page 2 to allow previous

        // Act & Assert
        // Should not throw because it returns early with failure message
        var result = await service.GetPreviousEventsAsync(cts.Token);
        result.IsSuccess.ShouldBeFalse();
    }
}
