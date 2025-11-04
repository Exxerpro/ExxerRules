using IndFusion.Mcp.Tests.Tools;

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
    private static readonly string ExampleFilePath = Path.GetFullPath(Path.Combine(TestUtilities.GetTestProjectDirectory(), "ExampleCode.cs"));
    private readonly string _originalDir = Directory.GetCurrentDirectory();

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyzeExxerFactoringOpportunitiesTests"/> class.
    /// </summary>
    public AnalyzeExxerFactoringOpportunitiesTests()
    {
        // Ensure ExampleCode.cs file exists before running tests
        TestUtilities.EnsureExampleCodeFile();
    }

    /// <summary>
    /// Disposes the test instance.
    /// </summary>
    public void Dispose()
    {
        Directory.SetCurrentDirectory(_originalDir);
    }

    /// <summary>
    /// Verifies that analyzing example code returns refactoring suggestions.
    /// </summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
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