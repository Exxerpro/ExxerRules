using IndFusion.Mcp.Core.Tools;

class Program
{
    static async Task Main()
    {
        var result = await SummaryResources.GetSummary(@"F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\src\test\IndFusion.Mcp.Tests\ExampleCode.cs", CancellationToken.None);
        Console.WriteLine("=== Current Output ===");
        Console.WriteLine(result);
        Console.WriteLine("=== End Output ===");
        
        // Print the specific string we're looking for
        var expected = "public int Calculate(int a, int b)\n        {}";
        Console.WriteLine($"Looking for: {expected}");
        Console.WriteLine($"Contains expected: {result.Contains(expected)}");
        
        // Find the Calculate method to see current format
        var lines = result.Split('\n');
        foreach (var line in lines)
        {
            if (line.Contains("Calculate"))
            {
                Console.WriteLine($"Found Calculate line: '{line}'");
            }
        }
    }
}