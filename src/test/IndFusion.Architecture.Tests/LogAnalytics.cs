namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Analytics information about a log dataset.
/// </summary>
public sealed class LogAnalytics
{
    /// <summary>
    /// Gets or sets the total number of log events.
    /// </summary>
    public int TotalLogCount { get; set; }

    /// <summary>
    /// Gets or sets the distribution of log levels.
    /// </summary>
    public Dictionary<LogLevel, int> LogLevelDistribution { get; set; } = new();

    /// <summary>
    /// Gets or sets the number of unique correlation IDs.
    /// </summary>
    public int UniqueCorrelationIds { get; set; }

    /// <summary>
    /// Gets or sets the list of operation types found in the logs.
    /// </summary>
    public List<string> OperationTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets the average length of log messages.
    /// </summary>
    public double AverageMessageLength { get; set; }

    /// <summary>
    /// Gets or sets the timestamp range of the logs.
    /// </summary>
    public TimestampRange? TimestampRange { get; set; }
}