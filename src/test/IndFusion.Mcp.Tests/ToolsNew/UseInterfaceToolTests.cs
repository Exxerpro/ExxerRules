using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for replacing concrete parameter types with interface types.
/// </summary>
public class UseInterfaceToolTests : TestBase
{
    /// <summary>
    /// Changes a method parameter from a class to an interface type.
    /// </summary>
    [Fact]
    public async Task UseInterface_ChangesParameterType()
    {
        const string initialCode = """
public class Service
{
    public void Process(Logger logger) { }
}

public class Logger { }
public interface ILogger { }
""";

        const string expectedCode = """
public class Service
{
    public void Process(ILogger logger) { }
}

public class Logger { }
public interface ILogger { }
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "UseInterface.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await UseInterfaceTool.UseInterface(
            SolutionPath,
            testFile,
            "Process",
            "logger",
            "ILogger");

        Assert.Contains("Successfully changed parameter", result);
        var fileContent = await File.ReadAllTextAsync(testFile);
        Assert.Equal(expectedCode, fileContent.Replace("\r\n", "\n"));
    }
}
