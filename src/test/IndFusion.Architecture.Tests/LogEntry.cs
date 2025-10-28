namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Represents a captured log entry.
/// </summary>
internal sealed class LogEntry
{
    public string CategoryName { get; set; } = string.Empty;
    public LogLevel LogLevel { get; set; }
    public EventId EventId { get; set; }
    public object? State { get; set; }
    public Exception? Exception { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
}