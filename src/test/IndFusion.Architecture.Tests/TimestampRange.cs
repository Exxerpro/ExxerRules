namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Represents a timestamp range for log analysis.
/// </summary>
public sealed class TimestampRange
{
    /// <summary>
    /// Gets or sets the start timestamp.
    /// </summary>
    public DateTimeOffset Start { get; set; }

    /// <summary>
    /// Gets or sets the end timestamp.
    /// </summary>
    public DateTimeOffset End { get; set; }

    /// <summary>
    /// Gets the duration of the timestamp range.
    /// </summary>
    public TimeSpan Duration => End - Start;
}