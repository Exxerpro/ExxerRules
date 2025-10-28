namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Represents a captured log event for behavioral testing.
/// </summary>
public sealed class LogEvent
{
    /// <summary>
    /// Gets or sets the log level.
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// Gets or sets the message template.
    /// </summary>
    public string? MessageTemplate { get; set; }

    /// <summary>
    /// Gets or sets the rendered message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the log properties.
    /// </summary>
    public Dictionary<string, object?> Properties { get; set; } = new();

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the exception (if any).
    /// </summary>
    public Exception? Exception { get; set; }
}