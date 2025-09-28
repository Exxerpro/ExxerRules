namespace IndFusion.Mcp.Tests;

/// <summary>
/// Type Logger
/// </summary>
public class Logger
{
    // Target location for moved instance methods
    /// <summary>
    /// Log.
    /// </summary>
    /// <param name="message"></param>
    public void Log(string message)
    {
        Console.WriteLine($"[LOG] {message}");
    }
}
