namespace ExxerAI.Architecture.Tests;

/// <summary>
/// In-memory logger implementation for capturing log entries.
/// </summary>
internal sealed class InMemoryLogger : ILogger
{
    private readonly string _categoryName;
    private readonly InMemoryLogProvider _provider;

    public InMemoryLogger(string categoryName, InMemoryLogProvider provider)
    {
        _categoryName = categoryName;
        _provider = provider;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NoOpScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);

        var entry = new LogEntry();
        entry.CategoryName = _categoryName;
        entry.LogLevel = logLevel;
        entry.EventId = eventId;
        entry.State = state;
        entry.Exception = exception;
        entry.Message = message;
        entry.Timestamp = DateTimeOffset.Now;
        _provider.AddLogEntry(entry);
    }
}