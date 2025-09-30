using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests;

/// <summary>
/// Helper to debug and fix test selection ranges
/// </summary>
public static class TestRangeHelper
{
    /// <summary>
    /// Prints example code with line numbers for debugging selection ranges
    /// </summary>
    public static void PrintExampleCodeWithLineNumbers()
    {
        var code = TestUtilities.GetSampleCodeForIntroduceField();
        var lines = code.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            Console.WriteLine($"{i + 1:D2}: {lines[i]}");
        }
    }

    /// <summary>
    /// Finds the selection range for a given expression in source code
    /// </summary>
    /// <param name="sourceCode">The source code to search in</param>
    /// <param name="searchExpression">The expression to find</param>
    /// <returns>The selection range string</returns>
    public static string FindExpressionRange(string sourceCode, string searchExpression)
    {
        return TestUtilities.GetSelectionRange(sourceCode, searchExpression);
    }
}
