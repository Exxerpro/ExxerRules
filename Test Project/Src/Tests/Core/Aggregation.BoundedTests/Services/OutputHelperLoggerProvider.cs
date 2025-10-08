namespace IndTrace.Aggregation.BoundedTests.Services;
/// <summary>
/// Represents the OutputHelperLoggerProvider.
/// </summary>

public class OutputHelperLoggerProvider(ITestOutputHelper outputHelper) : ILoggerProvider
{
    /// <summary>
    /// Executes CreateLogger operation.
    /// </summary>
    /// <param name="categoryName">The categoryName.</param>
    /// <returns>The result of CreateLogger.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new OutputHelperLogger(outputHelper, categoryName);
    }
    /// <summary>
    /// Executes Dispose operation.
    /// </summary>

    public void Dispose()
    {
    }

    private class OutputHelperLogger(ITestOutputHelper outputHelper, string categoryName) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }
        /// <summary>
        /// Executes IsEnabled operation.
        /// </summary>
        /// <param name="logLevel">The logLevel.</param>
        /// <returns>The result of IsEnabled.</returns>

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (formatter is not null && exception is not null)
            {
                outputHelper.WriteLine($"{logLevel.ToString()} [{categoryName}] {formatter(state, exception)}");
            }


        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Func<TState, string> formatter)
        {


            if (formatter is not null)
            {
                outputHelper.WriteLine($"{logLevel.ToString()} [{categoryName}] {formatter(state)}");
            }
        }
    }
}
