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

    public string LogFilePath => _logFilePath;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        var scope = _xunitLogger.BeginScope(state);
        WriteToFile($"SCOPE: {state}");
        return scope;
    }

    public bool IsEnabled(LogLevel logLevel) => _xunitLogger.IsEnabled(logLevel);

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

    public void Dispose()
    {
        _xunitLogger.LogInformation("=== Test Logging Completed ===");
        _fileWriter?.Dispose();
    }
}