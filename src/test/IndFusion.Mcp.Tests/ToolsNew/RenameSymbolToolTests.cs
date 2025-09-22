using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for renaming symbols and updating references across code.
/// </summary>
public class RenameSymbolToolTests : TestBase
{
    /// <summary>
    /// Renames a field and updates all references in the file.
    /// </summary>
    [Fact]
    public async Task RenameSymbol_Field_RenamesReferences()
    {
        const string initialCode = """
using System.Collections.Generic;
using System.Linq;

public class Sample
{
    private List<int> numbers = new();
    public int Sum() => numbers.Sum();
}
""";

        const string expectedCode = """
using System.Collections.Generic;
using System.Linq;

public class Sample
{
    private List<int> values = new();
    public int Sum() => values.Sum();
}
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "Rename.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(SolutionPath);
        var project = solution.Projects.First();
        ExxerFactoringHelpers.AddDocumentToProject(project, testFile);

        var result = await RenameSymbolTool.RenameSymbol(
            SolutionPath,
            testFile,
            "numbers",
            "values");

        Assert.Contains("Successfully renamed", result);
        var fileContent = await File.ReadAllTextAsync(testFile);
        Assert.Equal(expectedCode, fileContent.Replace("\r\n", "\n"));
    }

    /// <summary>
    /// Throws when the symbol to rename cannot be found.
    /// </summary>
    [Fact]
    public async Task RenameSymbol_InvalidName_ThrowsMcpException()
    {
        const string initialCode = """
using System.Collections.Generic;
using System.Linq;

public class Sample
{
    private List<int> numbers = new();
    public int Sum() => numbers.Sum();
}
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "RenameInvalid.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(SolutionPath);
        var project = solution.Projects.First();
        ExxerFactoringHelpers.AddDocumentToProject(project, testFile);

        await Assert.ThrowsAsync<McpException>(() =>
            RenameSymbolTool.RenameSymbol(
                SolutionPath,
                testFile,
                "missing",
                "newName"));
    }
}
