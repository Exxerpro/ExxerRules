namespace IndFusion.Mcp.Tests;

/// <summary>
/// Type AnalyzeExxerFactoringOpportunitiesTests : IDisposable
/// </summary>
///<summary>
///Type AnalyzeExxerFactoringOpportunitiesTests : IDisposable.
///</summary>
///<summary>
///Type AnalyzeExxerFactoringOpportunitiesTests : IDisposable.
///</summary>
///<summary>
///Type AnalyzeExxerFactoringOpportunitiesTests : IDisposable.
///</summary>
///<summary>
///Type AnalyzeExxerFactoringOpportunitiesTests : IDisposable.
///</summary>
public class AnalyzeExxerFactoringOpportunitiesTests : IDisposable
{
    private static readonly string SolutionPath = TestHelpers.GetSolutionPath();
    private static readonly string ExampleFilePath = Path.Combine(Path.GetDirectoryName(SolutionPath)!, "test", "IndFusion.Mcp.Tests", "ExampleCode.cs");
    private readonly string _originalDir = Directory.GetCurrentDirectory();

    /// <summary>
    /// Constructor AnalyzeExxerFactoringOpportunitiesTests
    /// </summary>
    public void Dispose()
    {
        Directory.SetCurrentDirectory(_originalDir);
    }

    /// <summary>
    /// AnalyzeExampleCode_ReturnsSuggestions
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AnalyzeExampleCode_ReturnsSuggestions()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, cancellationToken: TestContext.Current.CancellationToken);
        var result = await AnalyzeExxerFactoringOpportunitiesTool.AnalyzeExxerFactoringOpportunities(SolutionPath, ExampleFilePath, TestContext.Current.CancellationToken);
        Assert.Contains("safe-delete-field", result, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("safe-delete-method", result, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("make-static", result, StringComparison.OrdinalIgnoreCase);
    }
}
