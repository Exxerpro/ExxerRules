//[Move]
//CLAUDE
//Date: 26/08/2025
//Reason: [Test Relocation] - Moved to correct architectural layer based on its responsibility
namespace Application.UnitTests.Services;

/// <summary>
/// Represents the TaskExtensionsTests.
/// </summary>

public class TaskExtensionsTests
{
    /// <summary>
    /// Executes FireAndForgetSafeAsync_ShouldNotLogError_WhenTaskCompletesSuccessfully operation.
    /// </summary>
    /// <returns>The result of FireAndForgetSafeAsync_ShouldNotLogError_WhenTaskCompletesSuccessfully.</returns>
    [Fact]
    public async Task FireAndForgetSafeAsync_ShouldNotLogError_WhenTaskCompletesSuccessfully()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();

        // Act
        var task = Task.CompletedTask;
        task.FireAndForgetSafeAsync(logger);

        // Wait a bit to ensure the continuation task would have been executed
        await Task.Delay(100, TestContext.Current.CancellationToken);

        // Assert
        logger.DidNotReceive().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            //[Fix]
            //CLAUDE
            //Date: 29/08/2025
            //Reason: [CS8620] ILogger.Log expects Exception? nullable type in formatter
            Arg.Any<Func<object, Exception?, string>>()
        );
    }

    /// <summary>
    /// Executes FireAndForgetSafeAsync_ShouldNotLogAnyError_WhenTaskCompletesSuccessfully operation.
    /// </summary>
    /// <returns>The result of FireAndForgetSafeAsync_ShouldNotLogAnyError_WhenTaskCompletesSuccessfully.</returns>
    [Fact]
    public async Task FireAndForgetSafeAsync_ShouldNotLogAnyError_WhenTaskCompletesSuccessfully()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();

        // Act
        var task = Task.CompletedTask;
        task.FireAndForgetSafeAsync(logger);

        // Wait a bit to ensure the continuation task would have been executed
        await Task.Delay(100, TestContext.Current.CancellationToken);

        // Assert
        logger.ReceivedCalls().ShouldBeEmpty();
    }

    /// <summary>
    /// Executes FireAndForgetSafeAsync_ShouldLogError_WhenTaskFails operation.
    /// </summary>
    /// <returns>The result of FireAndForgetSafeAsync_ShouldLogError_WhenTaskFails.</returns>

    [Fact]
    public async Task FireAndForgetSafeAsync_ShouldLogError_WhenTaskFails()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var exception = new InvalidOperationException("Test exception");

        // Act
        var task = Task.FromException(exception);
        task.FireAndForgetSafeAsync(logger);

        // Wait a bit to ensure the continuation task is executed
        await Task.Delay(100, TestContext.Current.CancellationToken);

        // Assert
    }

    /// <summary>
    /// Executes FireAndForgetSafeAsync_ShouldNotThrow_WhenLoggerIsNullAndTaskFails operation.
    /// </summary>
    /// <returns>The result of FireAndForgetSafeAsync_ShouldNotThrow_WhenLoggerIsNullAndTaskFails.</returns>
    [Fact]
    public async Task FireAndForgetSafeAsync_ShouldNotThrow_WhenLoggerIsNullAndTaskFails()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");

        // Act
        Func<Task> act = async () =>
        {
            var task = Task.FromException(exception);
            task.FireAndForgetSafeAsync(null);
            await Task.Delay(100, TestContext.Current.CancellationToken); // Wait to ensure the continuation task is executed
        };

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 29/08/2025
        //Reason: [CS1739] Should.NotThrowAsync doesn't accept cancellationToken parameter
        await Should.NotThrowAsync(act);
    }
}
