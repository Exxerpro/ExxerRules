using System.Reflection;
using System.Text.Json;
using ModelContextProtocol.Server;
using IndFusion.Mcp.Core.Tools;

namespace IndFusion.Mcp.Core.Services;

/// <summary>
/// Utility for logging and replaying MCP server tool invocations to a JSON lines file for diagnostics.
/// </summary>
public static class ToolCallLogger
{
    private const string LogFileEnvVar = "ExxerFactor_Mcp_LOG_FILE";
    private static string _logFile = "tool-call-log.jsonl";

    /// <summary>
    /// Gets the default log file path used for tool call logging.
    /// </summary>
    public static string DefaultLogFile => _logFile;

    /// <summary>
    /// Sets the directory used for tool call logs and updates the environment variable to point to the new file.
    /// </summary>
    /// <param name="directory">The directory where the log file should be created.</param>
    public static void SetLogDirectory(string directory)
    {
        Directory.CreateDirectory(directory);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"); // Include milliseconds
        var processId = Environment.ProcessId;
        var threadId = Environment.CurrentManagedThreadId;
        var guid = Guid.NewGuid().ToString("N")[..8]; // Use first 8 chars of GUID for uniqueness
        _logFile = Path.Combine(directory, $"tool-call-log-{timestamp}-{processId}-{threadId}-{guid}.jsonl");
        Environment.SetEnvironmentVariable(LogFileEnvVar, _logFile);
    }

    /// <summary>
    /// Restores the log file path from the environment variable when present.
    /// </summary>
    public static void RestoreFromEnvironment()
    {
        var file = Environment.GetEnvironmentVariable(LogFileEnvVar);
        if (!string.IsNullOrEmpty(file))
            _logFile = file;
    }

    /// <summary>
    /// Appends a tool invocation record to the log file.
    /// </summary>
    /// <param name="toolName">The name of the tool being invoked.</param>
    /// <param name="parameters">Parameters passed to the tool.</param>
    /// <param name="logFile">Optional explicit log file path to use.</param>
    public static void Log(string toolName, Dictionary<string, string?> parameters, string? logFile = null)
    {
        var file = logFile ?? DefaultLogFile;
        var dir = Path.GetDirectoryName(file);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        var record = new ToolCallRecord
        {
            Tool = toolName,
            Parameters = parameters,
            Timestamp = DateTime.UtcNow
        };
        var json = JsonSerializer.Serialize(record);

        // Use retry logic with proper file sharing to handle concurrent access
        var maxRetries = 5;
        var delay = TimeSpan.FromMilliseconds(25);

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                // Allow concurrent readers and writers to minimize contention across tests
                using var stream = new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
                using var writer = new StreamWriter(stream);
                writer.WriteLine(json);
                return;
            }
            catch (IOException) when (attempt < maxRetries - 1)
            {
                Thread.Sleep(delay);
                delay = TimeSpan.FromMilliseconds(Math.Min(200, delay.TotalMilliseconds * 2)); // Exponential backoff with cap
            }
        }
    }

	/// <summary>
	/// Replays the tool invocations from a JSON lines log file, writing tool outputs to the console.
	/// </summary>
	/// <param name="logFilePath">Path to the JSONL log file.</param>
	public static async Task PlaybackAsync(string logFilePath)
    {
        if (!File.Exists(logFilePath))
        {
            Console.WriteLine($"Log file '{logFilePath}' not found");
            return;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        foreach (var line in await File.ReadAllLinesAsync(logFilePath))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            ToolCallRecord? record = null;
            try
            {
                record = JsonSerializer.Deserialize<ToolCallRecord>(line, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid log entry: {ex.Message}");
            }
            if (record != null)
                await InvokeTool(record.Tool, record.Parameters);
        }
    }

    /// <summary>
    /// Finds the tool implementation and invokes it with the provided parameters.
    /// </summary>
    private static async Task InvokeTool(string toolName, Dictionary<string, string?> parameters)
    {
        var method = GetToolMethod(toolName);
        if (method == null)
        {
            Console.WriteLine($"Unknown tool in log: {toolName}");
            return;
        }

        var paramInfos = method.GetParameters();
        var invokeArgs = new object?[paramInfos.Length];
        for (int i = 0; i < paramInfos.Length; i++)
        {
            var p = paramInfos[i];
            parameters.TryGetValue(p.Name!, out var raw);
            if (string.IsNullOrEmpty(raw))
            {
                if (p.HasDefaultValue)
                    invokeArgs[i] = p.DefaultValue;
                else
                {
                    Console.WriteLine($"Missing parameter {p.Name} for {toolName}");
                    return;
                }
            }
            else
            {
                invokeArgs[i] = ConvertInput(raw!, p.ParameterType);
            }
        }

        var result = method.Invoke(null, invokeArgs);
        if (result is Task<string> taskStr)
            Console.WriteLine(await taskStr);
        else if (result is Task task)
        {
            await task;
            Console.WriteLine("Done");
        }
        else if (result != null)
        {
            Console.WriteLine(result.ToString());
        }
    }

    /// <summary>
    /// Locates a tool method by name among all types marked with the server tool attribute.
    /// </summary>
    private static MethodInfo? GetToolMethod(string toolName)
    {
        return typeof(LoadSolutionTool).Assembly
            .GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(McpServerToolTypeAttribute), false).Length > 0)
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .FirstOrDefault(m => m.GetCustomAttributes(typeof(McpServerToolAttribute), false).Length > 0 &&
                                 m.Name.Equals(toolName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Converts a string input to the requested target type for reflective invocation.
    /// </summary>
    private static object? ConvertInput(string value, Type targetType)
    {
        if (targetType == typeof(string))
            return value;
        if (targetType == typeof(string[]))
            return value.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (targetType == typeof(int))
            return int.Parse(value);
        if (targetType == typeof(bool))
            return bool.Parse(value);
        return Convert.ChangeType(value, targetType);
    }

    /// <summary>
    /// Serializable record representing a single tool call entry.
    /// </summary>
    private class ToolCallRecord
    {
        public string Tool { get; set; } = string.Empty;
        public Dictionary<string, string?> Parameters { get; set; } = [];
        public DateTime Timestamp { get; set; }
    }
}
