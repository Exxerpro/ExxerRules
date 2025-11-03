using IndFusion.Mcp.Tests.Tools;
using IndFusion.Mcp.Core.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for converting mutable fields to readonly fields.
/// </summary>
public class MakeFieldReadonlyToolTests : TestBase
{
    /// <summary>
    /// Adds the readonly modifier to a field with an initializer.
    /// </summary>
    [Fact]
    public async Task MakeFieldReadonly_AddsReadonly()
    {
        const string initialCode = """
public class Sample
{
    private string name;
    public Sample() { }
}
""";

        const string expectedCode = """
public class Sample
{
    private readonly string name;
    public Sample() { }
}
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "Readonly.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await MakeFieldReadonlyTool.MakeFieldReadonly(
            SolutionPath,
            testFile,
            "name",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully made field", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var normalizedExpected = expectedCode.Replace("\r\n", "\n").Replace("\r", "\n");
        var normalizedActual = fileContent.Replace("\r\n", "\n").Replace("\r", "\n");
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    /// <summary>
    /// Adds the readonly modifier when the field has no initializer.
    /// </summary>
    [Fact]
    public async Task MakeFieldReadonly_FieldWithoutInitializer_AddsReadonly()
    {
        const string initialCode = """
public class Sample
{
    private string description;
}
""";

        const string expectedCode = """
public class Sample
{
    private readonly string description;
}
""";

        var testFile = Path.Combine(TestOutputPath, "ReadonlyNoInit.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await MakeFieldReadonlyTool.MakeFieldReadonly(
            null,
            testFile,
            "description",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully made field 'description' readonly", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var normalizedExpected = expectedCode.Replace("\r\n", "\n").Replace("\r", "\n");
        var normalizedActual = fileContent.Replace("\r\n", "\n").Replace("\r", "\n");
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    /// <summary>
    /// Throws when the field name cannot be found.
    /// </summary>
    [Fact]
    public async Task MakeFieldReadonly_InvalidIdentifier_ReturnsError()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);

        McpException ex = await Assert.ThrowsAsync<McpException>(() => MakeFieldReadonlyTool.MakeFieldReadonly(
            SolutionPath,
            ExampleFilePath,
            "nonexistent",
            cancellationToken: Xunit.TestContext.Current.CancellationToken));

        Assert.Equal("Error: Error: No field named 'nonexistent' found", ex.Message);
    }
}
