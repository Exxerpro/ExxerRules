namespace IndFusion.Mcp.Tests.Tools;

///<summary>
///Type IntroduceFieldTests : TestBase.
///</summary>
public class IntroduceFieldTests : TestBase
{
    /// <summary>
    /// IntroduceField ValidExpression ReturnsSuccess.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task IntroduceField_ValidExpression_ReturnsSuccess()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "IntroduceFieldTest.cs");
        var sample = TestUtilities.GetSampleCodeForIntroduceField();
        await TestUtilities.CreateTestFile(testFile, sample);

        var selection = TestUtilities.GetSelectionRange(sample, "numbers.Sum() / (double)numbers.Count");

        var result = await IntroduceFieldTool.IntroduceField(
            SolutionPath,
            testFile,
            selection,
            "_averageValue",
            "private",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully introduced", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("_averageValue", fileContent);
    }

    /// <summary>
    /// IntroduceField WithPublicModifier ReturnsSuccess.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task IntroduceField_WithPublicModifier_ReturnsSuccess()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "IntroduceFieldPublicTest.cs");
        var sample = TestUtilities.GetSampleCodeForIntroduceField();
        await TestUtilities.CreateTestFile(testFile, sample);

        var selection = TestUtilities.GetSelectionRange(sample, "numbers.Sum() / (double)numbers.Count");

        var result = await IntroduceFieldTool.IntroduceField(
            SolutionPath,
            testFile,
            selection,
            "_publicField",
            "public",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully introduced public field", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("_publicField", fileContent);
    }

    /// <summary>
    /// IntroduceField DifferentAccessModifiers ReturnsCorrectModifier.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task IntroduceField_DifferentAccessModifiers_ReturnsCorrectModifier()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "AccessModifierTest.cs");

        var accessModifiers = new[] { "private", "public", "protected", "internal" };
        var sample = TestUtilities.GetSampleCodeForIntroduceField();
        var selection = TestUtilities.GetSelectionRange(sample, "numbers.Sum() / (double)numbers.Count");

        foreach (var modifier in accessModifiers)
        {
            var modifierTestFile = testFile.Replace(".cs", $"_{modifier}.cs");
            await TestUtilities.CreateTestFile(modifierTestFile, sample);

            var result = await IntroduceFieldTool.IntroduceField(
                SolutionPath,
                modifierTestFile,
                selection,
                $"_{modifier}Field",
                modifier,
                cancellationToken: Xunit.TestContext.Current.CancellationToken);

            Assert.Contains($"Successfully introduced {modifier} field", result);
            var fileContent = await File.ReadAllTextAsync(modifierTestFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
            Assert.Contains($"_{modifier}Field", fileContent);
        }
    }

    /// <summary>
    /// IntroduceField FieldNameAlreadyExists ReturnsError.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task IntroduceField_FieldNameAlreadyExists_ReturnsError()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "IntroduceFieldDuplicate.cs");
        var sample = TestUtilities.GetSampleCodeForIntroduceField();
        await TestUtilities.CreateTestFile(testFile, sample);

        var selection = TestUtilities.GetSelectionRange(sample, "numbers.Sum() / (double)numbers.Count");

        var result = await IntroduceFieldTool.IntroduceField(
            SolutionPath,
            testFile,
            selection,
            "numbers",
            "private",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Equal("Error: Field 'numbers' already exists", result);
    }
}
