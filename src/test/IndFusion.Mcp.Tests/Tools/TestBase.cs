namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Common test base providing solution paths and per-test temp directories.
/// </summary>
public abstract class TestBase : IDisposable
{
    protected static readonly string SolutionPath = TestUtilities.GetSolutionPath();
    protected static readonly string ExampleFilePath = Path.Combine(Path.GetDirectoryName(SolutionPath)!, "IndFusion.Mcp.Tests", "ExampleCode.cs");

    private static readonly string TestOutputRoot =
        Path.Combine(Path.GetDirectoryName(SolutionPath)!, "IndFusion.Mcp.Tests", "TestOutput");

    protected string TestOutputPath { get; }

    /// <summary>
    /// Initializes a new temp output directory for the test instance.
    /// </summary>
    protected TestBase()
    {
        Directory.CreateDirectory(TestOutputRoot);
        TestOutputPath = Path.Combine(TestOutputRoot, Guid.NewGuid().ToString());
        Directory.CreateDirectory(TestOutputPath);
    }

    /// <summary>
    /// Cleans up the temp output directory after the test.
    /// </summary>
    public void Dispose()
    {
        if (Directory.Exists(TestOutputPath))
            Directory.Delete(TestOutputPath, true);
    }
}
