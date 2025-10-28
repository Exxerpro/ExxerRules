namespace ExxerAI.Architecture.Tests;

/// <summary>
/// In-memory log provider for capturing log entries during testing.
/// </summary>
internal sealed class InMemoryLogProvider : ILoggerProvider
{
    private volatile List<LogEntry> _logEntries = new();
    private readonly object _lock = new();

    public ILogger CreateLogger(string categoryName)
    {
        return new InMemoryLogger(categoryName, this);
    }

    public void AddLogEntry(LogEntry entry)
    {
        lock (_lock)
        {
            _logEntries.Add(entry);
        }
    }

    public IEnumerable<LogEntry> GetLogs()
    {
        lock (_lock)
        {
            return _logEntries.ToList(); // Return a copy for safety
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _logEntries.Clear();
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _logEntries.Clear();
        }
    }
}