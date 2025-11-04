using System.Diagnostics;

namespace IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers;

/// <summary>
/// System Under Test (SUT) tracer for method entry/exit logging with correlation IDs.
/// Helps debug hanging tests by tracing method calls in the SUT.
/// </summary>
public class SUTTracer
{
	private readonly ITestOutputHelper? _output;
	private readonly string _correlationId;

	/// <summary>
	/// Initializes a new instance of the <see cref="SUTTracer"/> class.
	/// </summary>
	/// <param name="output">Optional test output helper for logging.</param>
	public SUTTracer(ITestOutputHelper? output = null)
	{
		_output = output;
		_correlationId = Guid.NewGuid().ToString("N")[..8]; // Short correlation ID
	}

	/// <summary>
	/// Traces an async operation with method entry/exit logging and timing.
	/// </summary>
	/// <typeparam name="T">The return type of the operation.</typeparam>
	/// <param name="methodName">The name of the method being traced.</param>
	/// <param name="operation">The async operation to trace.</param>
	/// <param name="parameters">Optional dictionary of parameters for logging.</param>
	/// <returns>The result of the operation.</returns>
	public async Task<T> TraceAsync<T>(
		string methodName,
		Func<Task<T>> operation,
		Dictionary<string, object>? parameters = null)
	{
		var startTime = DateTime.UtcNow;
		var stopwatch = Stopwatch.StartNew();
		LogEntry(methodName, parameters);

		try
		{
			var result = await operation();
			stopwatch.Stop();
			LogExit(methodName, result, stopwatch.Elapsed);
			return result;
		}
		catch (Exception ex)
		{
			stopwatch.Stop();
			LogException(methodName, ex, stopwatch.Elapsed);
			throw;
		}
	}

	/// <summary>
	/// Traces an async operation that returns void with method entry/exit logging and timing.
	/// </summary>
	/// <param name="methodName">The name of the method being traced.</param>
	/// <param name="operation">The async operation to trace.</param>
	/// <param name="parameters">Optional dictionary of parameters for logging.</param>
	/// <returns>A task representing the async operation.</returns>
	public async Task TraceAsync(
		string methodName,
		Func<Task> operation,
		Dictionary<string, object>? parameters = null)
	{
		var startTime = DateTime.UtcNow;
		var stopwatch = Stopwatch.StartNew();
		LogEntry(methodName, parameters);

		try
		{
			await operation();
			stopwatch.Stop();
			LogExit(methodName, stopwatch.Elapsed);
		}
		catch (Exception ex)
		{
			stopwatch.Stop();
			LogException(methodName, ex, stopwatch.Elapsed);
			throw;
		}
	}

	/// <summary>
	/// Traces a synchronous operation with method entry/exit logging and timing.
	/// </summary>
	/// <typeparam name="T">The return type of the operation.</typeparam>
	/// <param name="methodName">The name of the method being traced.</param>
	/// <param name="operation">The operation to trace.</param>
	/// <param name="parameters">Optional dictionary of parameters for logging.</param>
	/// <returns>The result of the operation.</returns>
	public T Trace<T>(
		string methodName,
		Func<T> operation,
		Dictionary<string, object>? parameters = null)
	{
		var startTime = DateTime.UtcNow;
		var stopwatch = Stopwatch.StartNew();
		LogEntry(methodName, parameters);

		try
		{
			var result = operation();
			stopwatch.Stop();
			LogExit(methodName, result, stopwatch.Elapsed);
			return result;
		}
		catch (Exception ex)
		{
			stopwatch.Stop();
			LogException(methodName, ex, stopwatch.Elapsed);
			throw;
		}
	}

	private void LogEntry(string methodName, Dictionary<string, object>? parameters)
	{
		var timestamp = DateTime.UtcNow.ToString("HH:mm:ss.fff");
		var message = $"[{timestamp}] [{_correlationId}] ENTRY: {methodName}";
		
		if (parameters != null && parameters.Count > 0)
		{
			var paramString = string.Join(", ", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
			message += $" ({paramString})";
		}

		_output?.WriteLine(message);
	}

	private void LogExit<T>(string methodName, T? result, TimeSpan elapsed)
	{
		var timestamp = DateTime.UtcNow.ToString("HH:mm:ss.fff");
		var resultString = result is null ? "null" : result.ToString();
		var message = $"[{timestamp}] [{_correlationId}] EXIT: {methodName} -> {resultString} ({elapsed.TotalMilliseconds:F2}ms)";
		_output?.WriteLine(message);
	}

	private void LogExit(string methodName, TimeSpan elapsed)
	{
		var timestamp = DateTime.UtcNow.ToString("HH:mm:ss.fff");
		var message = $"[{timestamp}] [{_correlationId}] EXIT: {methodName} ({elapsed.TotalMilliseconds:F2}ms)";
		_output?.WriteLine(message);
	}

	private void LogException(string methodName, Exception ex, TimeSpan elapsed)
	{
		var timestamp = DateTime.UtcNow.ToString("HH:mm:ss.fff");
		var message = $"[{timestamp}] [{_correlationId}] EXCEPTION: {methodName} -> {ex.GetType().Name}: {ex.Message} ({elapsed.TotalMilliseconds:F2}ms)";
		_output?.WriteLine(message);
		_output?.WriteLine($"Stack Trace: {ex.StackTrace}");
	}
}
