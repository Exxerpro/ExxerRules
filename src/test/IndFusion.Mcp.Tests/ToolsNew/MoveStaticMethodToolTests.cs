using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for moving static methods to target types/files.
/// </summary>
public class MoveStaticMethodToolTests : TestBase
{
    /// <summary>
    /// Creates a new file for the target class and moves the static method there.
    /// </summary>
    [Fact]
    public async Task MoveStaticMethod_CreatesTargetFile()
    {
        const string initialCode = """
public class SourceClass
{
    public static int Foo() { return 1; }
}
""";

        // Expected strings are checked via specific Contains/DoesNotContain assertions below.

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "MoveStatic.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await MoveMethodTool.MoveStaticMethod(
            SolutionPath,
            testFile,
            "Foo",
            "TargetClass");

        Assert.Contains("Successfully moved static method", result);
        var sourceContent = await File.ReadAllTextAsync(testFile);
        Assert.DoesNotContain("Foo() { return 1; }", sourceContent);
        var targetFile = Path.Combine(TestOutputPath, "TargetClass.cs");
        var targetContent = await File.ReadAllTextAsync(targetFile);
        Assert.Contains("static int Foo", targetContent);
    }

    /// <summary>
    /// Preserves required usings for the moved method so the file compiles.
    /// </summary>
    [Fact]
    public async Task MoveStaticMethod_AddsUsingsAndCompiles()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "MoveStaticWithUsings.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForMoveStaticMethodWithUsings());

        var result = await MoveMethodTool.MoveStaticMethod(
            SolutionPath,
            testFile,
            "PrintList",
            "UtilClass");

        Assert.Contains("Successfully moved static method", result);
        var targetFile = Path.Combine(TestOutputPath, "UtilClass.cs");
        var fileContent = await File.ReadAllTextAsync(targetFile);
        Assert.Contains("using System", fileContent);
        Assert.Contains("using System.Collections.Generic", fileContent);

        var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
        var refs = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
            .Split(Path.PathSeparator)
            .Select(p => MetadataReference.CreateFromFile(p));
        var compilation = CSharpCompilation.Create(
            "test",
            new[] { syntaxTree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var diagnostics = compilation.GetDiagnostics();
        Assert.DoesNotContain(diagnostics, d => d.Severity == DiagnosticSeverity.Error);
    }
}
