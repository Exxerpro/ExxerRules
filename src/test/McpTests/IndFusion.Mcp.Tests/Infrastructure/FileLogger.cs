using Microsoft.Extensions.Logging;
using Meziantou.Extensions.Logging.Xunit.v3;
using System.Text;

namespace IndFusion.Mcp.Tests.Infrastructure;

/// <summary>
/// Logger that writes to both xUnit output (via Meziantou) and a file for debugging.
/// </summary>
public sealed class FileLogger<T> : ILogger<T>, IDisposable
{
    private readonly ILogger<T> _xunitLogger;
    private readonly StreamWriter _fileWriter;
    private readonly string _logFilePath;
    private readonly object _lockObject = new();

    /// <summary>
    /// Initializes a logger that mirrors messages to xUnit output and a timestamped log file.
    /// </summary>
    /// <param name="output">The xUnit output helper for the current test.</param>
    /// <param name="testName">The name used to derive the log file.</param>
    public FileLogger(Xunit.ITestOutputHelper output, string testName)
    {
        // Create xUnit logger via Meziantou
        _xunitLogger = Meziantou.Extensions.Logging.Xunit.v3.XUnitLogger.CreateLogger<T>(output);

        // Create file logger in test output directory
        var testOutputDir = Path.Combine(Path.GetTempPath(), "IndFusion.Tests", "Logs");
        Directory.CreateDirectory(testOutputDir);
        _logFilePath = Path.Combine(testOutputDir, $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.log");
        _fileWriter = new StreamWriter(_logFilePath, append: true, Encoding.UTF8)
        {
            AutoFlush = true
        };

        _xunitLogger.LogInformation("=== Test Logging Started ===");
        _xunitLogger.LogInformation("Test: {TestName}", testName);
        _xunitLogger.LogInformation("Log File: {LogFilePath}", _logFilePath);
    }

    /// <summary>
    /// Gets the full path to the log file generated for the test.
    /// </summary>
    public string LogFilePath => _logFilePath;

    /// <summary>
    /// Begins a logging scope recorded in both xUnit output and the file log.
    /// </summary>
    /// <typeparam name="TState">The type representing the scope state.</typeparam>
    /// <param name="state">The scope state.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the scope when disposed.</returns>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        var scope = _xunitLogger.BeginScope(state);
        WriteToFile($"SCOPE: {state}");
        return scope;
    }

    /// <summary>
    /// Determines whether the specified log level is enabled.
    /// </summary>
    /// <param name="logLevel">The log level to evaluate.</param>
    /// <returns><c>true</c> when logging at the level is enabled; otherwise, <c>false</c>.</returns>
    public bool IsEnabled(LogLevel logLevel) => _xunitLogger.IsEnabled(logLevel);

    /// <summary>
    /// Writes a log entry to xUnit output and the backing log file.
    /// </summary>
    /// <typeparam name="TState">The type of the log state.</typeparam>
    /// <param name="logLevel">The severity of the entry.</param>
    /// <param name="eventId">The event identifier associated with the entry.</param>
    /// <param name="state">The log state to record.</param>
    /// <param name="exception">An optional exception related to the entry.</param>
    /// <param name="formatter">The formatter used to render the message.</param>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        // Forward to xUnit logger
        _xunitLogger.Log(logLevel, eventId, state, exception, formatter);

        // Also write to file with timestamp
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var message = formatter(state, exception);
        var logEntry = $"[{timestamp}] [{logLevel}] {message}";

        if (exception != null)
        {
            logEntry += $"{Environment.NewLine}Exception: {exception}{Environment.NewLine}{exception.StackTrace}";
        }

        WriteToFile(logEntry);
    }

    private void WriteToFile(string message)
    {
        lock (_lockObject)
        {
            try
            {
                _fileWriter.WriteLine(message);
                _fileWriter.Flush();
            }
            catch
            {
                // Ignore file write errors to prevent test failures
            }
        }
    }

    /// <summary>
    /// Completes logging for the test and releases file resources.
    /// </summary>
    public void Dispose()
    {
        _xunitLogger.LogInformation("=== Test Logging Completed ===");
        _fileWriter?.Dispose();
    }
}
