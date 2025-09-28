namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Common test base providing solution paths and per-test temp directories.
/// </summary>
public abstract class TestBase : IDisposable
{
    /// <summary>
    /// Absolute path to the solution file used for locating test assets.
    /// </summary>
    protected static readonly string SolutionPath = TestUtilities.GetSolutionPath();

    /// <summary>
    /// Absolute path to the shared example C# file used in tests.
    /// </summary>
    protected static readonly string ExampleFilePath = Path.Combine(Path.GetDirectoryName(SolutionPath)!, "IndFusion.Mcp.Tests", "ExampleCode.cs");

    private static readonly string TestOutputRoot =
        Path.Combine(Path.GetDirectoryName(SolutionPath)!, "IndFusion.Mcp.Tests", "TestOutput");

    /// <summary>
    /// TestOutputPath.
    /// </summary>
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
