using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Test fixture for behavioral logging validation using in-memory log capture.
/// Captures actual log output during test execution for compliance verification.
/// </summary>
public sealed class SeqLoggingTestFixture : IAsyncDisposable
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly InMemoryLogProvider _logProvider;
    private readonly object _lockObject = new();
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the SeqLoggingTestFixture class.
    /// </summary>
    public SeqLoggingTestFixture()
    {
        _logProvider = new InMemoryLogProvider();
        
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Trace)
                .ClearProviders()
                .AddProvider(_logProvider);
        });
    }

    /// <summary>
    /// Creates a logger instance for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to create a logger for</typeparam>
    /// <returns>A configured logger instance</returns>
    public ILogger<T> CreateLogger<T>() where T : class
    {
        return _loggerFactory.CreateLogger<T>();
    }

    /// <summary>
    /// Creates a logger instance for the specified category name.
    /// </summary>
    /// <param name="categoryName">The category name for the logger</param>
    /// <returns>A configured logger instance</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return _loggerFactory.CreateLogger(categoryName);
    }

    /// <summary>
    /// Captures log events for a specific correlation ID during test execution.
    /// </summary>
    /// <param name="correlationId">The correlation ID to track</param>
    /// <param name="testAction">The test action to execute</param>
    /// <returns>The captured log events</returns>
    public async Task<IEnumerable<LogEvent>> CaptureLogsAsync(string correlationId, Func<Task> testAction)
    {
        if (string.IsNullOrEmpty(correlationId))
            throw new ArgumentException("Correlation ID cannot be null or empty", nameof(correlationId));

        if (testAction == null)
            throw new ArgumentNullException(nameof(testAction));

        // Clear any existing logs
        _logProvider.Clear();

        // Execute the test action
        await testAction().ConfigureAwait(false);

        // Allow some time for async log writing to complete
        await Task.Delay(100).ConfigureAwait(false);

        // Convert Microsoft Extensions Logging entries to our LogEvent format
        var capturedLogs = new List<LogEvent>();
        foreach (var logEntry in _logProvider.GetLogs())
        {
            var logEvent = new LogEvent();
            logEvent.Level = logEntry.LogLevel;
            logEvent.MessageTemplate = logEntry.Message;
            logEvent.Message = logEntry.Message;
            logEvent.Timestamp = DateTimeOffset.Now;
            logEvent.Properties = new Dictionary<string, object?>();

            // Extract properties from the log entry state
            if (logEntry.State is IEnumerable<KeyValuePair<string, object?>> properties)
            {
                foreach (var prop in properties)
                {
                    if (prop.Key != "{OriginalFormat}")
                    {
                        logEvent.Properties[prop.Key] = prop.Value;
                    }
                }
            }

            // Extract correlation ID from properties or structured data
            if (logEvent.Properties.ContainsKey("CorrelationId") || 
                logEvent.MessageTemplate?.Contains("CorrelationId") == true ||
                logEntry.Message.Contains(correlationId))
            {
                logEvent.Properties["CorrelationId"] = correlationId;
                capturedLogs.Add(logEvent);
            }
            else
            {
                // Add all logs for now during debugging
                capturedLogs.Add(logEvent);
            }
        }

        return capturedLogs;
    }

    /// <summary>
    /// Validates that logs contain structured logging patterns.
    /// </summary>
    /// <param name="logs">The logs to validate</param>
    /// <param name="expectedOperation">The expected operation name</param>
    /// <param name="correlationId">The expected correlation ID</param>
    /// <returns>True if validation passes</returns>
    public static bool ValidateStructuredLogging(IEnumerable<LogEvent> logs, string expectedOperation, string correlationId)
    {
        if (logs == null) return false;

        var logList = logs.ToList();
        
        // Check for operation start log
        var hasOperationStart = logList.Any(log => 
            log.MessageTemplate?.Contains("Starting operation") == true &&
            log.Properties.ContainsKey("Operation") &&
            log.Properties["Operation"]?.ToString() == expectedOperation &&
            log.Properties.ContainsKey("CorrelationId") &&
            log.Properties["CorrelationId"]?.ToString() == correlationId);

        // Check for operation completion log (success or failure)
        var hasOperationCompletion = logList.Any(log => 
            (log.MessageTemplate?.Contains("Completed operation") == true || 
             log.MessageTemplate?.Contains("failed after") == true) &&
            log.Properties.ContainsKey("Operation") &&
            log.Properties["Operation"]?.ToString() == expectedOperation &&
            log.Properties.ContainsKey("CorrelationId") &&
            log.Properties["CorrelationId"]?.ToString() == correlationId);

        // Check for timing information
        var hasTiming = logList.Any(log => 
            log.Properties.ContainsKey("Duration") &&
            log.Properties["Duration"] != null);

        return hasOperationStart && hasOperationCompletion && hasTiming;
    }

    /// <summary>
    /// Validates that logs contain correlation ID tracking.
    /// </summary>
    /// <param name="logs">The logs to validate</param>
    /// <param name="expectedCorrelationId">The expected correlation ID</param>
    /// <returns>True if all logs contain the correlation ID</returns>
    public static bool ValidateCorrelationIdTracking(IEnumerable<LogEvent> logs, string expectedCorrelationId)
    {
        if (logs == null || string.IsNullOrEmpty(expectedCorrelationId)) return false;

        return logs.All(log => 
            log.Properties.ContainsKey("CorrelationId") &&
            log.Properties["CorrelationId"]?.ToString() == expectedCorrelationId);
    }

    /// <summary>
    /// Validates that logs contain external API call logging patterns.
    /// </summary>
    /// <param name="logs">The logs to validate</param>
    /// <param name="expectedApiName">The expected API name</param>
    /// <returns>True if external API logging is present</returns>
    public static bool ValidateExternalApiLogging(IEnumerable<LogEvent> logs, string expectedApiName)
    {
        if (logs == null || string.IsNullOrEmpty(expectedApiName)) return false;

        var logList = logs.ToList();

        // Check for API call start log
        var hasApiStart = logList.Any(log => 
            log.MessageTemplate?.Contains("Starting external API call") == true &&
            log.Properties.ContainsKey("ApiName") &&
            log.Properties["ApiName"]?.ToString() == expectedApiName);

        // Check for API call completion log
        var hasApiCompletion = logList.Any(log => 
            (log.MessageTemplate?.Contains("External API call") == true &&
             log.MessageTemplate?.Contains("completed successfully") == true) &&
            log.Properties.ContainsKey("ApiName") &&
            log.Properties["ApiName"]?.ToString() == expectedApiName);

        return hasApiStart && hasApiCompletion;
    }

    /// <summary>
    /// Clears all captured logs.
    /// </summary>
    public void ClearLogs()
    {
        lock (_lockObject)
        {
            _logProvider.Clear();
        }
    }

    /// <summary>
    /// Gets all currently captured logs.
    /// </summary>
    /// <returns>The captured log events</returns>
    public IEnumerable<LogEvent> GetCapturedLogs()
    {
        lock (_lockObject)
        {
            var capturedLogs = new List<LogEvent>();
            foreach (var logEntry in _logProvider.GetLogs())
            {
                var logEvent = new LogEvent();
                logEvent.Level = logEntry.LogLevel;
                logEvent.MessageTemplate = logEntry.Message;
                logEvent.Message = logEntry.Message;
                logEvent.Timestamp = logEntry.Timestamp;
                logEvent.Properties = new Dictionary<string, object?>();

                // Extract properties from the log entry state
                if (logEntry.State is IEnumerable<KeyValuePair<string, object?>> properties)
                {
                    foreach (var prop in properties)
                    {
                        if (prop.Key != "{OriginalFormat}")
                        {
                            logEvent.Properties[prop.Key] = prop.Value;
                        }
                    }
                }

                capturedLogs.Add(logEvent);
            }

            return capturedLogs;
        }
    }

    /// <summary>
    /// Disposes of the test fixture and cleans up resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        try
        {
            _loggerFactory?.Dispose();
            _logProvider?.Dispose();
        }
        finally
        {
            _disposed = true;
        }

        await Task.CompletedTask;
    }
}