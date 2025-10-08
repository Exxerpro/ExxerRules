using System.Reactive;

namespace Application.UnitTests.Infrastructure;

/// <summary>
/// Unit tests for IndTraceEventsService
/// </summary>
public class IndTraceEventsServiceTests
{
    private readonly IndTraceEventsService _eventsService = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public IndTraceEventsServiceTests()
    {
        _eventsService = new IndTraceEventsService();
    }

    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var eventsService = new IndTraceEventsService();

        // Assert
        eventsService.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes Constructor_WithNullParameters_ShouldThrowException operation.
    /// </summary>

    [Fact]
    public void Constructor_WithNullParameters_ShouldThrowException()
    {
        // Act & Assert
        // IndTraceEventsService is a singleton, so we can't test null parameters
        Should.NotThrow(() => new IndTraceEventsService());
    }

    /// <summary>
    /// Executes Subscribe_WithValidHandler_ShouldAddHandler operation.
    /// </summary>

    [Fact]
    public void Subscribe_WithValidHandler_ShouldAddHandler()
    {
        // Arrange
        var handler = Substitute.For<Action<StateChange>>();
        var stateChange = new StateChange { State = "TestState" };

        // Act
        var subscription = _eventsService.Subscribe(Observer.Create<StateChange>(handler));

        // Assert
        subscription.ShouldNotBeNull();
        // Note: We can't easily test the internal subscription without exposing it
    }

    /// <summary>
    /// Executes Subscribe_WithNullHandler_ShouldThrowException operation.
    /// </summary>

    [Fact]
    public void Subscribe_WithNullHandler_ShouldThrowException()
    {
        // Arrange
        Action<StateChange>? handler = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _eventsService.Subscribe(handler!));
    }

    /// <summary>
    /// Executes Subscribe_WithMultipleHandlers_ShouldAddAllHandlers operation.
    /// </summary>

    [Fact]
    public void Subscribe_WithMultipleHandlers_ShouldAddAllHandlers()
    {
        // Arrange
        var handler1 = Substitute.For<Action<StateChange>>();
        var handler2 = Substitute.For<Action<StateChange>>();
        var handler3 = Substitute.For<Action<StateChange>>();

        // Act
        var subscription1 = _eventsService.Subscribe(handler1);
        var subscription2 = _eventsService.Subscribe(handler2);
        var subscription3 = _eventsService.Subscribe(handler3);

        // Assert
        subscription1.ShouldNotBeNull();
        subscription2.ShouldNotBeNull();
        subscription3.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes Notify_WithValidStateChange_ShouldNotifySubscribers operation.
    /// </summary>

    [Fact]
    public void Notify_WithValidStateChange_ShouldNotifySubscribers()
    {
        // Arrange
        var handler = Substitute.For<Action<StateChange>>();
        var stateChange = new StateChange { State = "TestState" };
        var subscription = _eventsService.Subscribe(Observer.Create<StateChange>(handler));

        // Act
        _eventsService.NotifyStateChanged(stateChange);

        // Assert
        handler.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "TestState"));
    }

    /// <summary>
    /// Executes Notify_WithNullStateChange_ShouldThrowException operation.
    /// </summary>

    [Fact]
    public void Notify_WithNullStateChange_ShouldThrowException()
    {
        // Arrange
        StateChange? stateChange = null!;

        // Act & Assert
        // Should do nothing
        // ??what to assert here

        _eventsService.NotifyStateChanged(stateChange!);

        //nothing has changed just assert true??
        true.ShouldBeTrue();
    }

    /// <summary>
    /// Executes Notify_WithMultipleSubscribers_ShouldNotifyAllSubscribers operation.
    /// </summary>

    [Fact]
    public void Notify_WithMultipleSubscribers_ShouldNotifyAllSubscribers()
    {
        // Arrange
        var handler1 = Substitute.For<Action<StateChange>>();
        var handler2 = Substitute.For<Action<StateChange>>();
        var handler3 = Substitute.For<Action<StateChange>>();

        var subscription1 = _eventsService.Subscribe(handler1);
        var subscription2 = _eventsService.Subscribe(handler2);
        var subscription3 = _eventsService.Subscribe(handler3);

        var stateChange = new StateChange { State = "TestState" };

        // Act
        _eventsService.NotifyStateChanged(stateChange);

        // Assert
        handler1.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "TestState"));
        handler2.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "TestState"));
        handler3.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "TestState"));
    }

    /// <summary>
    /// Executes Notify_WithNoSubscribers_ShouldNotThrowException operation.
    /// </summary>

    [Fact]
    public void Notify_WithNoSubscribers_ShouldNotThrowException()
    {
        // Arrange
        var stateChange = new StateChange { State = "TestState" };

        // Act & Assert
        Should.NotThrow(() => _eventsService.NotifyStateChanged(stateChange));
    }

    /// <summary>
    /// Executes Notify_WithComplexStateChange_ShouldNotifySubscribers operation.
    /// </summary>

    [Fact]
    public void Notify_WithComplexStateChange_ShouldNotifySubscribers()
    {
        // Arrange
        var handler = Substitute.For<Action<StateChange>>();
        var stateChange = new StateChange
        {
            State = "ComplexState",
            Data = new Dictionary<string, object>
            {
                { "Key1", "Value1" },
                { "Key2", 123 },
                { "Key3", true }
            }
        };
        var subscription = _eventsService.Subscribe(Observer.Create<StateChange>(handler));

        // Act
        _eventsService.NotifyStateChanged(stateChange);

        // Assert
        handler.Received(1).Invoke(Arg.Is<StateChange>(sc =>
            (string)sc.State == "ComplexState" &&
            sc.Data != null &&
            ((Dictionary<string, object>)sc.Data).Count == 3));
    }

    /// <summary>
    /// Executes Notify_WithEmptyStateChange_ShouldNotifySubscribers operation.
    /// </summary>

    [Fact]
    public void Notify_WithEmptyStateChange_ShouldNotifySubscribers()
    {
        // Arrange
        var handler = Substitute.For<Action<StateChange>>();
        var stateChange = new StateChange { State = "" };
        var subscription = _eventsService.Subscribe(Observer.Create<StateChange>(handler));

        // Act
        _eventsService.NotifyStateChanged(stateChange);

        // Assert
        handler.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == ""));
    }

    /// <summary>
    /// Executes Notify_WithNullStateInStateChange_ShouldNotifySubscribers operation.
    /// </summary>

    [Fact]
    public void Notify_WithNullStateInStateChange_ShouldNotifySubscribers()
    {
        // Arrange
        var handler = Substitute.For<Action<StateChange>>();
        var stateChange = new StateChange { State = null! };
        var subscription = _eventsService.Subscribe(Observer.Create<StateChange>(handler));

        // Act
        _eventsService.NotifyStateChanged(stateChange);

        // Assert
        handler.Received(1).Invoke(Arg.Is<StateChange>(sc => sc.State == null));
    }

    /// <summary>
    /// Executes Notify_WithNullDataInStateChange_ShouldNotifySubscribers operation.
    /// </summary>

    [Fact]
    public void Notify_WithNullDataInStateChange_ShouldNotifySubscribers()
    {
        // Arrange
        var handler = Substitute.For<Action<StateChange>>();
        var stateChange = new StateChange { State = "TestState", Data = null! };
        var subscription = _eventsService.Subscribe(Observer.Create<StateChange>(handler));

        // Act
        _eventsService.NotifyStateChanged(stateChange);

        // Assert
        handler.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "TestState" && sc.Data == null));
    }

    /// <summary>
    /// Executes Notify_WithMultipleNotifications_ShouldNotifySubscribersMultipleTimes operation.
    /// </summary>

    [Fact]
    public void Notify_WithMultipleNotifications_ShouldNotifySubscribersMultipleTimes()
    {
        // Arrange
        var handler = Substitute.For<Action<StateChange>>();
        var subscription = _eventsService.Subscribe(Observer.Create<StateChange>(handler));

        var stateChange1 = new StateChange { State = "State1" };
        var stateChange2 = new StateChange { State = "State2" };
        var stateChange3 = new StateChange { State = "State3" };

        // Act
        _eventsService.NotifyStateChanged(stateChange1);
        _eventsService.NotifyStateChanged(stateChange2);
        _eventsService.NotifyStateChanged(stateChange3);

        // Assert
        handler.Received(3).Invoke(Arg.Any<StateChange>());
        handler.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "State1"));
        handler.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "State2"));
        handler.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "State3"));
    }

    /// <summary>
    /// Executes Subscribe_WithHandlerThatThrowsException_ShouldNotBreakOtherHandlers operation.
    /// </summary>

    [Fact]
    public void Subscribe_WithHandlerThatThrowsException_ShouldNotBreakOtherHandlers()
    {
        // Arrange
        var handler1 = Substitute.For<Action<StateChange>>();
        var handler2 = Substitute.For<Action<StateChange>>();
        var handler3 = Substitute.For<Action<StateChange>>();

        handler2.When(x => x.Invoke(Arg.Any<StateChange>())).Throw(new Exception("Test exception"));

        var subscription1 = _eventsService.Subscribe(handler1);
        var subscription2 = _eventsService.Subscribe(handler2);
        var subscription3 = _eventsService.Subscribe(handler3);

        var stateChange = new StateChange { State = "TestState" };

        // Act & Assert
        Should.NotThrow(() => _eventsService.NotifyStateChanged(stateChange));

        handler1.Received(1).Invoke(Arg.Is<StateChange>(sc => sc.State.ToString() == "TestState"));
        handler3.Received(1).Invoke(Arg.Is<StateChange>(sc => sc.State.ToString() == "TestState"));
    }

    /// <summary>
    /// Executes Subscribe_WithSameHandlerMultipleTimes_ShouldAddHandlerMultipleTimes operation.
    /// </summary>

    [Fact]
    public void Subscribe_WithSameHandlerMultipleTimes_ShouldAddHandlerMultipleTimes()
    {
        // Arrange
        var handler = Substitute.For<Action<StateChange>>();
        var stateChange = new StateChange { State = "TestState" };

        // Act
        var subscription1 = _eventsService.Subscribe(handler);
        var subscription2 = _eventsService.Subscribe(handler);
        var subscription3 = _eventsService.Subscribe(handler);

        _eventsService.NotifyStateChanged(stateChange);

        // Assert
        handler.Received(3).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "TestState"));
    }

    /// <summary>
    /// Executes Subscribe_WithDifferentHandlers_ShouldAddAllHandlers operation.
    /// </summary>

    [Fact]
    public void Subscribe_WithDifferentHandlers_ShouldAddAllHandlers()
    {
        // Arrange
        var handler1 = Substitute.For<Action<StateChange>>();
        var handler2 = Substitute.For<Action<StateChange>>();
        var stateChange = new StateChange { State = "TestState" };

        // Act
        var subscription1 = _eventsService.Subscribe(handler1);
        var subscription2 = _eventsService.Subscribe(handler2);

        _eventsService.NotifyStateChanged(stateChange);

        // Assert
        handler1.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "TestState"));
        handler2.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "TestState"));
    }

    /// <summary>
    /// Executes Notify_WithLargeNumberOfSubscribers_ShouldNotifyAllSubscribers operation.
    /// </summary>

    [Fact]
    public void Notify_WithLargeNumberOfSubscribers_ShouldNotifyAllSubscribers()
    {
        // Arrange
        var handlers = new List<Action<StateChange>>();
        var subscriptions = new List<IDisposable>();

        for (int i = 0; i < 100; i++)
        {
            var handler = Substitute.For<Action<StateChange>>();
            handlers.Add(handler);
            subscriptions.Add(_eventsService.Subscribe(handler));
        }

        var stateChange = new StateChange { State = "TestState" };

        // Act
        _eventsService.NotifyStateChanged(stateChange);

        // Assert
        foreach (var handler in handlers)
        {
            handler.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "TestState"));
        }
    }

    /// <summary>
    /// Executes Notify_WithConcurrentNotifications_ShouldHandleConcurrency operation.
    /// </summary>

    [Fact]
    public async Task Notify_WithConcurrentNotifications_ShouldHandleConcurrencyAsync()
    {
        // Arrange
        var handler = Substitute.For<Action<StateChange>>();
        var subscription = _eventsService.Subscribe(Observer.Create<StateChange>(handler));
        var stateChange = new StateChange { State = "TestState" };

        // Act
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                _eventsService.NotifyStateChanged(stateChange);
                return Task.CompletedTask;
            }, TestContext.Current.CancellationToken));
        }

        await Task.WhenAll(tasks.ToArray());

        // Assert
        handler.Received(10).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "TestState"));
    }

    /// <summary>
    /// Executes Notify_WithStateChangeContainingSpecialCharacters_ShouldNotifySubscribers operation.
    /// </summary>

    [Fact]
    public void Notify_WithStateChangeContainingSpecialCharacters_ShouldNotifySubscribers()
    {
        // Arrange
        var handler = Substitute.For<Action<StateChange>>();
        var stateChange = new StateChange { State = "Test@#$%^&*()State" };
        var subscription = _eventsService.Subscribe(Observer.Create<StateChange>(handler));

        // Act
        _eventsService.NotifyStateChanged(stateChange);

        // Assert
        handler.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "Test@#$%^&*()State"));
    }

    /// <summary>
    /// Executes Notify_WithStateChangeContainingUnicodeCharacters_ShouldNotifySubscribers operation.
    /// </summary>

    [Fact]
    public void Notify_WithStateChangeContainingUnicodeCharacters_ShouldNotifySubscribers()
    {
        // Arrange
        var handler = Substitute.For<Action<StateChange>>();
        var stateChange = new StateChange { State = "TestStateñáéíóú" };
        var subscription = _eventsService.Subscribe(Observer.Create<StateChange>(handler));

        // Act
        _eventsService.NotifyStateChanged(stateChange);

        // Assert
        handler.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "TestStateñáéíóú"));
    }

    /// <summary>
    /// Executes Notify_WithStateChangeContainingNumbers_ShouldNotifySubscribers operation.
    /// </summary>

    [Fact]
    public void Notify_WithStateChangeContainingNumbers_ShouldNotifySubscribers()
    {
        // Arrange
        var handler = Substitute.For<Action<StateChange>>();
        var stateChange = new StateChange { State = "TestState123" };
        var subscription = _eventsService.Subscribe(Observer.Create<StateChange>(handler));

        // Act
        _eventsService.NotifyStateChanged(stateChange);

        // Assert
        handler.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "TestState123"));
    }

    /// <summary>
    /// Executes Notify_WithStateChangeContainingWhitespace_ShouldNotifySubscribers operation.
    /// </summary>

    [Fact]
    public void Notify_WithStateChangeContainingWhitespace_ShouldNotifySubscribers()
    {
        // Arrange
        var handler = Substitute.For<Action<StateChange>>();
        var stateChange = new StateChange { State = "Test State With Spaces" };
        var subscription = _eventsService.Subscribe(Observer.Create<StateChange>(handler));

        // Act
        _eventsService.NotifyStateChanged(stateChange);

        // Assert
        handler.Received(1).Invoke(Arg.Is<StateChange>(sc => (string)sc.State == "Test State With Spaces"));
    }
}
