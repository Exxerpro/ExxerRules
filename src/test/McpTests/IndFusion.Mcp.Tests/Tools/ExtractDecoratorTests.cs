namespace IndFusion.Mcp.Tests.Tools;

 ///<summary>
 ///Type ExtractDecoratorTests : TestBase.
 ///</summary>
public class ExtractDecoratorTests : TestBase
{
    /// <summary>
    /// ExtractDecorator AddsClass.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ExtractDecorator_AddsClass()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "Decorator.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForDecorator());

        var result = await ExtractDecoratorTool.ExtractDecorator(
            SolutionPath,
            testFile,
            "Greeter",
            "Greet",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Created decorator", result);
        var text = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("GreeterDecorator", text);
    }
}
