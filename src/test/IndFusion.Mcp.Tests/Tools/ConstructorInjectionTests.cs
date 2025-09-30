namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Tests for converting parameter usage into constructor-injected members.
/// </summary>
public class ConstructorInjectionTests : TestBase
{
    /// <summary>
    /// Converts a method parameter to a constructor-injected field.
    /// </summary>
    [Fact]
    public async Task ConstructorInjection_Valid_ReturnsSuccess()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "ConstructorInjection.cs");
        await TestUtilities.CreateTestFile(testFile, "class C{ int M(int x){ return x+1; } void Call(){ M(1); } }");

        var result = await ConstructorInjectionTool.ConvertToConstructorInjection(
            SolutionPath,
            testFile,
            new[] { new ConstructorInjectionTool.MethodParameterPair("M", "x") },
            false,
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully injected", result);
        var content = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("_x", content);
    }
}
