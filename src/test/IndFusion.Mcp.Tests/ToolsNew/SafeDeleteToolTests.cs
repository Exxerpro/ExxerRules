using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for safely deleting unused fields, methods, and types.
/// </summary>
    public class SafeDeleteToolTests : TestBase
    {
        /// <summary>
        /// Removes an unused field from a class.
        /// </summary>
        [Fact]
        public async Task SafeDeleteField_RemovesUnusedField()
    {
        const string initialCode = """
public class Sample
{
    private int unused;
}
""";

        const string expectedCode = """
public class Sample
{
}
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "SafeDelete.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await SafeDeleteTool.SafeDeleteField(
            SolutionPath,
            testFile,
            "unused");

        Assert.Contains("Successfully deleted field", result);
        var fileContent = await File.ReadAllTextAsync(testFile);
        Assert.Equal(expectedCode, fileContent.Replace("\r\n", "\n"));
    }

        /// <summary>
        /// Removes an unused private method from a class.
        /// </summary>
        [Fact]
        public async Task SafeDeleteMethod_RemovesUnusedMethod()
    {
        const string initialCode = """
public class Sample
{
    private void UnusedHelper()
    {
        int tempValue = 0;
    }
}
""";

        const string expectedCode = """
public class Sample
{
}
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "SafeDeleteMethod.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await SafeDeleteTool.SafeDeleteMethod(
            SolutionPath,
            testFile,
            "UnusedHelper");

        Assert.Contains("Successfully deleted method", result);
        var fileContent = await File.ReadAllTextAsync(testFile);
        Assert.Equal(expectedCode, fileContent.Replace("\r\n", "\n"));
    }

        /// <summary>
        /// Removes an unused local variable within a method.
        /// </summary>
        [Fact]
        public async Task SafeDeleteVariable_RemovesUnusedLocal()
    {
        const string initialCode = """
public class Sample
{
    public void DoWork()
    {
        int tempValue = 0;
    }
}
""";

        const string expectedCode = """
public class Sample
{
    public void DoWork()
    {
    }
}
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "SafeDeleteVariable.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await SafeDeleteTool.SafeDeleteVariable(
            SolutionPath,
            testFile,
            "5:9-5:26");

        Assert.Contains("Successfully deleted variable", result);
        var fileContent = await File.ReadAllTextAsync(testFile);
        Assert.Equal(expectedCode, fileContent.Replace("\r\n", "\n"));
    }
}
