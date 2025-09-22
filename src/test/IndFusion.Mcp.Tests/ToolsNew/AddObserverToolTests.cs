using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for AddObserver tool that adds events and invocations to classes.
/// </summary>
public class AddObserverToolTests : TestBase
{
    /// <summary>
    /// Adds an event and invocation to the specified class and method.
    /// </summary>
    [Fact]
    public async Task AddObserver_AddsEventAndInvocation()
    {
        const string initialCode = """
public class Counter
{
    public void Update() { }
}
""";

        const string expectedCode = """
public class Counter
{
    public void Update() { Updated?.Invoke(); }

    public event Action Updated;
}
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "Observer.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await AddObserverTool.AddObserver(
            SolutionPath,
            testFile,
            "Counter",
            "Update",
            "Updated");

        Assert.Contains("Added observer", result);
        var fileContent = await File.ReadAllTextAsync(testFile);
        Assert.Equal(expectedCode, fileContent.Replace("\r\n", "\n"));
    }

    /// <summary>
    /// Throws when the target class name does not exist in the file.
    /// </summary>
    [Fact]
    public async Task AddObserver_InvalidClassName_ThrowsMcpException()
    {
        const string initialCode = """
public class Counter
{
    public void Update() { }
}
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "Observer.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        McpException ex = await Assert.ThrowsAsync<McpException>(() => AddObserverTool.AddObserver(
            SolutionPath,
            testFile,
            "WrongClass",
            "Update",
            "Updated"));

        Assert.Equal("Error adding observer: Error: Class 'WrongClass' not found", ex.Message);
    }

    /// <summary>
    /// Throws when the target method name does not exist.
    /// </summary>
    [Fact]
    public async Task AddObserver_InvalidMethodName_ThrowsMcpException()
    {
        const string initialCode = """
public class Counter
{
    public void Update() { }
}
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "Observer.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        McpException ex = await Assert.ThrowsAsync<McpException>(() => AddObserverTool.AddObserver(
            SolutionPath,
            testFile,
            "Counter",
            "WrongMethod",
            "Updated"));

        Assert.Equal("Error adding observer: Error: Method 'WrongMethod' not found", ex.Message);
    }
}
