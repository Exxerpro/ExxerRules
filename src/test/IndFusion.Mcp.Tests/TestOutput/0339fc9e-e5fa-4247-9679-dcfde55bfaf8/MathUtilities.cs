namespace IndFusion.Mcp.Tests;

public class MathUtilities
{
    public static string FormatCurrency(decimal amount)
    {
        return $"${amount:F2}"; // This static method could be moved to a utility class
    }
    public static void LogOperation(string operation)
    {
        Console.WriteLine($"[{DateTime.Now}] {operation}");
    }
}
