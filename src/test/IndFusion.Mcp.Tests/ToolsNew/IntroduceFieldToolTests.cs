using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary> /// Represents the  IntroduceFieldToolTests  class. /// </summary>
public class IntroduceFieldToolTests : TestBase
{
    /// <summary> ///  IntroduceField CreatesField. /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task IntroduceField_CreatesField()
    {
        const string initialCode = """
using System.Linq;

public class Sample
{
    public double GetAverage(int[] values)
    {
        return values.Sum() / (double)values.Length;
    }
}
""";
        const string expectedCode = """
using System.Linq;

public class Sample
{
    private int _avg = values.Sum();

    public double GetAverage(int[] values)
    {
        return _avg / (double)values.Length;
    }
}
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "IntroduceField.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await IntroduceFieldTool.IntroduceField(
            SolutionPath,
            testFile,
            "6:16-6:57",
            "_avg");

        Assert.Contains("Successfully introduced", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(expectedCode, fileContent.Replace("\r\n", "\n"));
    }

    /// <summary> ///  IntroduceField SupportsAccessModifiers. /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task IntroduceField_SupportsAccessModifiers()
    {
        const string code = """
using System.Linq;

public class Sample
{
    public double GetAverage(int[] values)
    {
        return values.Sum() / (double)values.Length;
    }
}
""";
        var modifiers = new[] { "public", "protected", "internal" };
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        foreach (var modifier in modifiers)
        {
            var file = Path.Combine(TestOutputPath, $"Access_{modifier}.cs");
            await TestUtilities.CreateTestFile(file, code);

            var result = await IntroduceFieldTool.IntroduceField(
                SolutionPath,
                file,
                "6:16-6:57",
                $"_{modifier}Field",
                modifier);

            Assert.Contains($"Successfully introduced {modifier} field", result);
            var content = await File.ReadAllTextAsync(file, cancellationToken: Xunit.TestContext.Current.CancellationToken);
            Assert.Contains($"_{modifier}Field", content);
        }
    }

    /// <summary> ///  IntroduceField FieldNameAlreadyExists ReturnsError. /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task IntroduceField_FieldNameAlreadyExists_ReturnsError()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "DuplicateField.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForIntroduceField());

        var result = await IntroduceFieldTool.IntroduceField(
            SolutionPath,
            testFile,
            "36:20-36:56",
            "numbers",
            "private");

        Assert.Equal("Error: Field 'numbers' already exists", result);
    }
}
