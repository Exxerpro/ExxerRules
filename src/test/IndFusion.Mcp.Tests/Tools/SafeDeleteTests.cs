namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Tests for SafeDeleteTests.
/// </summary>
public class SafeDeleteTests : TestBase
{
    /// <summary>
    /// SafeDeleteField UnusedField ReturnsSuccess.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SafeDeleteField_UnusedField_ReturnsSuccess()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "SafeDeleteField.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForSafeDelete());

        var result = await SafeDeleteTool.SafeDeleteField(
            SolutionPath,
            testFile,
            "deprecatedCounter");

        Assert.Contains("Successfully deleted field", result);
        var fileContent = await File.ReadAllTextAsync(testFile);
        Assert.DoesNotContain("deprecatedCounter", fileContent);
    }

    /// <summary>
    /// SafeDeleteMethod UnusedMethod ReturnsSuccess.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SafeDeleteMethod_UnusedMethod_ReturnsSuccess()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "SafeDeleteMethod.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForSafeDelete());

        var result = await SafeDeleteTool.SafeDeleteMethod(
            SolutionPath,
            testFile,
            "UnusedHelper");

        Assert.Contains("Successfully deleted method", result);
        var fileContent = await File.ReadAllTextAsync(testFile);
        Assert.DoesNotContain("UnusedHelper", fileContent);
    }

    /// <summary>
    /// SafeDeleteVariable UnusedLocal ReturnsSuccess.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SafeDeleteVariable_UnusedLocal_ReturnsSuccess()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "SafeDeleteVariable.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForSafeDelete());

        var result = await SafeDeleteTool.SafeDeleteVariable(
            SolutionPath,
            testFile,
            "85:13-85:30");

        Assert.Contains("Successfully deleted variable", result);
        var fileContent = await File.ReadAllTextAsync(testFile);
        Assert.DoesNotContain("tempValue", fileContent);
    }
}
