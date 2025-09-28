using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for moving multiple methods into a separate file/type.
/// </summary>
public class MoveMethodsFileToolTests : TestBase
{
    /// <summary>
    /// MoveStaticMethodInFile CreatesNewFileAndStub.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task MoveStaticMethodInFile_CreatesNewFileAndStub()
    {
        var testFile = Path.Combine(TestOutputPath, "StaticFile.cs");
        await TestUtilities.CreateTestFile(testFile, "public class A { public static int Foo(){ return 1; } } public class B { }");

        var result = await MoveMethodFileService.MoveStaticMethodInFile(
            testFile,
            "Foo",
            "B",
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Contains("Successfully moved static method", result);
        var targetFile = Path.Combine(Path.GetDirectoryName(testFile)!, "B.cs");
        Assert.True(File.Exists(targetFile));

        var sourceContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("return B.Foo()", sourceContent);
        var targetContent = await File.ReadAllTextAsync(targetFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("class B", targetContent);
        Assert.Contains("static int Foo", targetContent);
    }

    /// <summary>
    /// MoveStaticMethodInFile SameFileAddsMethodAndStub.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task MoveStaticMethodInFile_SameFileAddsMethodAndStub()
    {
        var testFile = Path.Combine(TestOutputPath, "StaticSameFile.cs");
        await TestUtilities.CreateTestFile(testFile, "public class A { public static int Foo(){ return 1; } } public class B { }");

        var result = await MoveMethodFileService.MoveStaticMethodInFile(
            testFile,
            "Foo",
            "B",
            testFile,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Contains("Successfully moved static method", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("class B", fileContent);
        Assert.Contains("static int Foo", fileContent);
        Assert.Contains("return B.Foo()", fileContent);
    }

    /// <summary>
    /// MoveInstanceMethodInFile CreatesNewFileAndStub.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task MoveInstanceMethodInFile_CreatesNewFileAndStub()
    {
        var testFile = Path.Combine(TestOutputPath, "InstanceFile.cs");
        await TestUtilities.CreateTestFile(testFile, "public class A { public int Bar(){ return 1; } } public class B { }");

        var targetFile = Path.Combine(Path.GetDirectoryName(testFile)!, "B.cs");
        var result = await MoveMethodFileService.MoveInstanceMethodInFile(
            testFile,
            "A",
            "Bar",
            Array.Empty<string>(),
            Array.Empty<string>(),
            "B",
            "",
            "",
            targetFile,
            progress: null,
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully moved instance method", result);
        Assert.Contains("made static", result);
        Assert.True(File.Exists(targetFile));

        var sourceContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("return B.Bar()", sourceContent);
        var targetContent = await File.ReadAllTextAsync(targetFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("class B", targetContent);
        Assert.Contains("static int Bar", targetContent);
    }

    /// <summary>
    /// MoveInstanceMethodInFile SameFileAddsMethodAndStub.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task MoveInstanceMethodInFile_SameFileAddsMethodAndStub()
    {
        var testFile = Path.Combine(TestOutputPath, "InstanceSameFile.cs");
        await TestUtilities.CreateTestFile(testFile, "public class A { public int Bar(){ return 1; } } public class B { }");

        var result = await MoveMethodFileService.MoveInstanceMethodInFile(
            testFile,
            "A",
            "Bar",
            Array.Empty<string>(),
            Array.Empty<string>(),
            "B",
            "",
            "",
            testFile,
            progress: null,
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully moved instance method", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("class B", fileContent);
        Assert.Contains("static int Bar", fileContent);
        Assert.Contains("return B.Bar()", fileContent);
    }
}
