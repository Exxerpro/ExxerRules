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
    private readonly string _originalDir = Directory.GetCurrentDirectory();

    /// <summary>
    /// Gets the path to the ExampleCode.cs file, ensuring it exists first.
    /// </summary>
    private static string ExampleFilePath
    {
        get
        {
            // Ensure the file exists first
            TestUtilities.EnsureExampleCodeFile();
            
            // Get the path from TestUtilities
            var testProjectDir = TestUtilities.GetTestProjectDirectory();
            var examplePath = Path.Combine(testProjectDir, "ExampleCode.cs");
            
            // If file doesn't exist at the expected path, try alternative locations
            if (!File.Exists(examplePath))
            {
                // Try the actual test project directory
                var assemblyLocation = typeof(AnalyzeExxerFactoringOpportunitiesTests).Assembly.Location;
                var assemblyDir = Path.GetDirectoryName(assemblyLocation);
                if (!string.IsNullOrEmpty(assemblyDir))
                {
                    var alternativePath = Path.Combine(assemblyDir, "ExampleCode.cs");
                    if (File.Exists(alternativePath))
                    {
                        return alternativePath;
                    }
                }
                
                // Try relative to the test project directory
                var currentDir = Directory.GetCurrentDirectory();
                var relativePath = Path.Combine(currentDir, "ExampleCode.cs");
                if (File.Exists(relativePath))
                {
                    return relativePath;
                }
            }
            
            return Path.GetFullPath(examplePath);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyzeExxerFactoringOpportunitiesTests"/> class.
    /// </summary>
    public AnalyzeExxerFactoringOpportunitiesTests()
    {
        // Ensure ExampleCode.cs file exists before running tests
        TestUtilities.EnsureExampleCodeFile();
        
        // Verify the file exists at the expected path
        if (!File.Exists(ExampleFilePath))
        {
            throw new FileNotFoundException($"ExampleCode.cs not found at expected path: {ExampleFilePath}");
        }
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