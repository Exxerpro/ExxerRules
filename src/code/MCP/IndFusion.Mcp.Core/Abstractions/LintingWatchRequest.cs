namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for starting a linting watcher.
/// </summary>
/// <param name="SolutionPath">Absolute path to the target solution file (.sln).</param>
/// <param name="WatchPatterns">File patterns to watch for changes.</param>
/// <param name="DebounceMs">Debounce delay in milliseconds before triggering linting.</param>
/// <param name="AutoFix">Whether to automatically apply fixes when possible.</param>
/// <param name="NotificationEndpoint">Optional endpoint to receive violation notifications.</param>
public record LintingWatchRequest(
    string SolutionPath,
    IEnumerable<string> WatchPatterns,
    int DebounceMs = 1000,
    bool AutoFix = false,
    string? NotificationEndpoint = null
);