namespace IndTrace.Dependencies.Interceptors;

public static class LogTimerExtensions
{
    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate log timer extension logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    //TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated extension or logging logic. Refactor for maintainability if necessary.
    //TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency log timer operations, consider optimizing data processing and memory usage.

    private static readonly ConcurrentDictionary<string, DateTime> lastLogTimes
         = new ConcurrentDictionary<string, DateTime>();

    // For thread safety, we'll keep a lock object (if needed for finer-grained control).
    private static readonly object syncLock = new object();

    /// <summary>
    /// Logs a warning message only if the specified time interval has elapsed
    /// since the last time the exact same message was logged.
    /// </summary>
    /// <param name="logger">The ILogger instance.</param>
    /// <param name="message">The warning message to log.</param>
    /// <param name="minimumInterval">The minimum interval that must elapse before logging again.</param>
    public static void LogWarningRateLimited<T>(this ILogger<T> logger, string message, TimeSpan minimumInterval)
    {
        // Optional: In some scenarios, you might use the message as a key.
        // If you have multiple unique messages, each can be tracked separately.
        var key = message;

        lock (syncLock)
        {
            // Get the current time
            var now = DateTime.UtcNow;

            // Check when we last logged this particular message
            if (lastLogTimes.TryGetValue(key, out DateTime lastTime))
            {
                // If not enough time has passed, we skip logging
                if (now - lastTime < minimumInterval)
                {
                    return;
                }
            }

            // Update the dictionary with the new log time
            lastLogTimes[key] = now;

            // Log the warning
            logger.LogWarning(message);
        }
    }

    public static void LogWarningRateLimited(this ILogger logger, string message, TimeSpan minimumInterval)
    {
        // Optional: In some scenarios, you might use the message as a key.
        // If you have multiple unique messages, each can be tracked separately.
        var key = message;

        lock (syncLock)
        {
            // Get the current time
            var now = DateTime.UtcNow;

            // Check when we last logged this particular message
            if (lastLogTimes.TryGetValue(key, out DateTime lastTime))
            {
                // If not enough time has passed, we skip logging
                if (now - lastTime < minimumInterval)
                {
                    return;
                }
            }

            // Update the dictionary with the new log time
            lastLogTimes[key] = now;

            // Log the warning
            logger.LogWarning(message);
        }
    }
}
