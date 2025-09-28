namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Tests for adding observer events and invocations to classes.
/// </summary>
public class AddObserverTests : TestBase
{
    /// <summary>
    /// Adds an event and invocation to the specified class and method.
    /// </summary>
    [Fact]
    public async Task AddObserver_AddsEvent()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "Observer.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForObserver());

        var result = await AddObserverTool.AddObserver(
            SolutionPath,
            testFile,
            "Counter",
            "Update",
            "Updated",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Added observer", result);
        var text = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("event", text);
        Assert.Contains("Updated?.Invoke", text);
    }

    /// <summary>
    /// Throws when the specified class cannot be found.
    /// </summary>
    [Fact]
    public async Task AddObserver_InvalidClassName_ThrowsMcpException()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "Observer.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForObserver());

        McpException ex = await Assert.ThrowsAsync<McpException>(() => AddObserverTool.AddObserver(
            SolutionPath,
            testFile,
            "WrongClass",
            "Update",
            "Updated",
            cancellationToken: Xunit.TestContext.Current.CancellationToken));

        Assert.Equal("Error adding observer: Error: Class 'WrongClass' not found", ex.Message);
    }

    /// <summary>
    /// Throws when the specified method cannot be found.
    /// </summary>
    [Fact]
    public async Task AddObserver_InvalidMethodName_ThrowsMcpException()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "Observer.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForObserver());

        McpException ex = await Assert.ThrowsAsync<McpException>(() => AddObserverTool.AddObserver(
            SolutionPath,
            testFile,
            "Counter",
            "WrongMethod",
            "Updated",
            cancellationToken: Xunit.TestContext.Current.CancellationToken));

        Assert.Equal("Error adding observer: Error: Method 'WrongMethod' not found", ex.Message);
    }
}
