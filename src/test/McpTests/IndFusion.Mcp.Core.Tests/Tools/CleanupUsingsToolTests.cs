using IndFusion.Mcp.Core.Exceptions;
using McpProtocolException = ModelContextProtocol.McpException;

namespace IndFusion.Mcp.Core.Tests.Tools;

/// <summary>
/// Tests for <see cref="IndFusion.Mcp.Core.Tools.CleanupUsingsTool"/> ensuring unused using directives are removed.
/// </summary>
public class CleanupUsingsToolTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly string _testSolutionPath;
    private readonly string _testFilePath;

    /// <summary> /// Initializes a new instance of the class. /// </summary>
    public CleanupUsingsToolTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"CleanupUsingsToolTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
        _testSolutionPath = Path.Combine(_testDirectory, "TestSolution.sln");
        _testFilePath = Path.Combine(_testDirectory, "TestClass.cs");

        CreateTestSolution();
    }

    /// <summary> /// CleanupUsings WithUnusedUsings ShouldRemoveThem. /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task CleanupUsings_WithUnusedUsings_ShouldRemoveThem()
    {
        // Arrange
        var sourceCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestClass
{
    public void TestMethod()
    {
        Console.WriteLine(""Hello"");
    }
}";
        await File.WriteAllTextAsync(_testFilePath, sourceCode, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Act
        var result = await CleanupUsingsTool.CleanupUsings(null, _testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.ShouldContain("Removed unused usings");
        var cleanedCode = await File.ReadAllTextAsync(_testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        cleanedCode.ShouldNotContain("using System.Collections.Generic;");
        cleanedCode.ShouldNotContain("using System.Linq;");
        cleanedCode.ShouldContain("using System;");
    }

    /// <summary> /// CleanupUsings WithAllUsedUsings ShouldKeepThem(). /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task CleanupUsings_WithAllUsedUsings_ShouldKeepThem()
    {
        // Arrange
        var sourceCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestClass
{
    public void TestMethod()
    {
        var list = new List<string>();
        var result = list.Where(x => x.Length > 0).ToList();
        Console.WriteLine(result.Count.ToString());
    }
}";
        await File.WriteAllTextAsync(_testFilePath, sourceCode, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Act
        var result = await CleanupUsingsTool.CleanupUsings(null, _testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.ShouldContain("Removed unused usings");
        var cleanedCode = await File.ReadAllTextAsync(_testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        cleanedCode.ShouldContain("using System;");
        cleanedCode.ShouldContain("using System.Collections.Generic;");
        cleanedCode.ShouldContain("using System.Linq;");
    }

    /// <summary> /// CleanupUsings WithNoUsings ShouldReturnSuccess. /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task CleanupUsings_WithNoUsings_ShouldReturnSuccess()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod()
    {
        var x = 1;
    }
}";
        await File.WriteAllTextAsync(_testFilePath, sourceCode, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Act
        var result = await CleanupUsingsTool.CleanupUsings(null, _testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.ShouldContain("Removed unused usings");
        var cleanedCode = await File.ReadAllTextAsync(_testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        cleanedCode.ShouldBe(sourceCode); // No changes expected
    }

    /// <summary> /// CleanupUsings WithGlobalUsings ShouldHandleThemCorrectly. /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task CleanupUsings_WithGlobalUsings_ShouldHandleThemCorrectly()
    {
        // Arrange
        var sourceCode = @"
global using System;
global using System.Collections.Generic;

using System.Linq;

public class TestClass
{
    public void TestMethod()
    {
        var list = new List<string>();
        var result = list.Where(x => x.Length > 0).ToList();
    }
}";
        await File.WriteAllTextAsync(_testFilePath, sourceCode, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Act
        var result = await CleanupUsingsTool.CleanupUsings(null, _testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.ShouldContain("Removed unused usings");
        var cleanedCode = await File.ReadAllTextAsync(_testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        cleanedCode.ShouldNotContain("global using System;"); // Should be removed since not used
        cleanedCode.ShouldContain("global using System.Collections.Generic;");
        cleanedCode.ShouldContain("using System.Linq;");
    }

    /// <summary> /// CleanupUsings WithAliasUsings ShouldHandleThemCorrectly. /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task CleanupUsings_WithAliasUsings_ShouldHandleThemCorrectly()
    {
        // Arrange
        var sourceCode = @"
using System;
using System.Collections.Generic;
using System.Linq;
using MyAlias = System.Collections.Generic.List<string>;

public class TestClass
{
    public void TestMethod()
    {
        var list = new MyAlias();
        Console.WriteLine(list.Count);
    }
}";
        await File.WriteAllTextAsync(_testFilePath, sourceCode, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Act
        var result = await CleanupUsingsTool.CleanupUsings(null, _testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.ShouldContain("Removed unused usings");
        var cleanedCode = await File.ReadAllTextAsync(_testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        cleanedCode.ShouldContain("using System;");
        cleanedCode.ShouldContain("using MyAlias = System.Collections.Generic.List<string>;");
        cleanedCode.ShouldNotContain("using System.Collections.Generic;"); // Should be removed as it's not directly used
        cleanedCode.ShouldNotContain("using System.Linq;"); // Should be removed as it's not used
    }

    /// <summary> /// CleanupUsings WithStaticUsings ShouldHandleThemCorrectly. /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task CleanupUsings_WithStaticUsings_ShouldHandleThemCorrectly()
    {
        // Arrange
        var sourceCode = @"
using System;
using System.Collections.Generic;
using static System.Console;
using static System.Math;

public class TestClass
{
    public void TestMethod()
    {
        WriteLine(""Hello"");
        var result = Sqrt(16);
    }
}";
        await File.WriteAllTextAsync(_testFilePath, sourceCode, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Act
        var result = await CleanupUsingsTool.CleanupUsings(null, _testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.ShouldContain("Removed unused usings");
        var cleanedCode = await File.ReadAllTextAsync(_testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        cleanedCode.ShouldNotContain("using System;"); // Should be removed since static usings are used
        cleanedCode.ShouldContain("using static System.Console;");
        cleanedCode.ShouldContain("using static System.Math;");
        cleanedCode.ShouldNotContain("using System.Collections.Generic;"); // Should be removed
    }

    /// <summary> /// CleanupUsings WithNamespaceUsings ShouldHandleThemCorrectly(). /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task CleanupUsings_WithNamespaceUsings_ShouldHandleThemCorrectly()
    {
        // Arrange
        var sourceCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var list = new List<string>();
            var result = list.Where(x => x.Length > 0).ToList();
            Console.WriteLine(result.Count.ToString());
        }
    }
}";
        await File.WriteAllTextAsync(_testFilePath, sourceCode, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Act
        var result = await CleanupUsingsTool.CleanupUsings(null, _testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.ShouldContain("Removed unused usings");
        var cleanedCode = await File.ReadAllTextAsync(_testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        cleanedCode.ShouldContain("using System;");
        cleanedCode.ShouldContain("using System.Collections.Generic;");
        cleanedCode.ShouldContain("using System.Linq;");
    }

    /// <summary> /// CleanupUsings WithCompilationErrors ShouldHandleGracefully(). /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task CleanupUsings_WithCompilationErrors_ShouldHandleGracefully()
    {
        // Arrange
        var sourceCode = @"
using System;
using System.Collections.Generic;

public class TestClass
{
    public void TestMethod()
    {
        // This will cause compilation errors
        var invalid = new NonExistentType();
        Console.WriteLine(invalid);
    }
}";
        await File.WriteAllTextAsync(_testFilePath, sourceCode, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Act
        var result = await CleanupUsingsTool.CleanupUsings(null, _testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.ShouldContain("Removed unused usings");
        // Should still attempt to clean up usings even with compilation errors
    }

    /// <summary> /// CleanupUsings WithSolutionContext ShouldUseSolutionAnalysis(). /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task CleanupUsings_WithSolutionContext_ShouldUseSolutionAnalysis()
    {
        // Arrange
        var sourceCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestClass
{
    public void TestMethod()
    {
        var list = new List<string>();
        var result = list.Where(x => x.Length > 0).ToList();
        Console.WriteLine(result.Count.ToString());
    }
}";
        await File.WriteAllTextAsync(_testFilePath, sourceCode, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Act
        var result = await CleanupUsingsTool.CleanupUsings(_testSolutionPath, _testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.ShouldContain("Removed unused usings");
        var cleanedCode = await File.ReadAllTextAsync(_testFilePath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        cleanedCode.ShouldContain("using System;");
        cleanedCode.ShouldContain("using System.Collections.Generic;");
        cleanedCode.ShouldContain("using System.Linq;");
    }

    /// <summary> /// CleanupUsings WithNonExistentFile ShouldThrowException(). /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task CleanupUsings_WithNonExistentFile_ShouldThrowException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_testDirectory, "NonExistent.cs");

        // Act & Assert
        await Assert.ThrowsAsync<IndFusion.Mcp.Core.Exceptions.McpException>(() =>
            CleanupUsingsTool.CleanupUsings(null, nonExistentPath, cancellationToken: Xunit.TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Removes unused using directives when present in the source.
    /// </summary>
    [Fact]
    public void CleanupUsingsInSource_WithUnusedUsings_ShouldRemoveThem()
    {
        // Arrange
        var sourceCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestClass
{
    public void TestMethod()
    {
        Console.WriteLine(""Hello"");
    }
}";

        // Act
        var result = CleanupUsingsTool.CleanupUsingsInSource(sourceCode);

        // Assert
        result.ShouldNotContain("using System.Collections.Generic;");
        result.ShouldNotContain("using System.Linq;");
        result.ShouldContain("using System;");
        result.ShouldContain("public class TestClass");
    }

    /// <summary> /// CleanupUsingsInSource WithAllUsedUsings ShouldKeepThem(). /// </summary>
    [Fact]
    public void CleanupUsingsInSource_WithAllUsedUsings_ShouldKeepThem()
    {
        // Arrange
        var sourceCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestClass
{
    public void TestMethod()
    {
        var list = new List<string>();
        var result = list.Where(x => x.Length > 0).ToList();
        Console.WriteLine(result.Count.ToString());
    }
}";

        // Act
        var result = CleanupUsingsTool.CleanupUsingsInSource(sourceCode);

        // Assert
        result.ShouldContain("using System;");
        result.ShouldContain("using System.Collections.Generic;");
        result.ShouldContain("using System.Linq;");
    }

    /// <summary> /// CleanupUsingsInSource WithCompilationErrors ShouldReturnOriginal(). /// </summary>
    [Fact]
    public void CleanupUsingsInSource_WithCompilationErrors_ShouldReturnOriginal()
    {
        // Arrange
        var sourceCode = @"
using System;
using System.Collections.Generic;

public class TestClass
{
    public void TestMethod()
    {
        // This will cause compilation errors
        var invalid = new NonExistentType();
        Console.WriteLine(invalid);
    }
}";

        // Act
        var result = CleanupUsingsTool.CleanupUsingsInSource(sourceCode);

        // Assert
        // Should clean up unused usings even with compilation errors
        result.ShouldNotContain("using System.Collections.Generic;");
        result.ShouldContain("using System;");
    }

    /// <summary> /// CleanupUsingsInSource WithEmptySource ShouldReturnEmpty(). /// </summary>
    [Fact]
    public void CleanupUsingsInSource_WithEmptySource_ShouldReturnEmpty()
    {
        // Arrange
        var sourceCode = "";

        // Act
        var result = CleanupUsingsTool.CleanupUsingsInSource(sourceCode);

        // Assert
        result.ShouldBeEmpty();
    }

    /// <summary>
    /// When only using directives are present, removes all as unused.
    /// </summary>
    [Fact]
    public void CleanupUsingsInSource_WithOnlyUsings_ShouldRemoveUnused()
    {
        // Arrange
        var sourceCode = @"
using System;
using System.Collections.Generic;
using System.Linq;
";

        // Act
        var result = CleanupUsingsTool.CleanupUsingsInSource(sourceCode);

        // Assert
        // All usings should be removed since none are used
        result.ShouldNotContain("using System;");
        result.ShouldNotContain("using System.Collections.Generic;");
        result.ShouldNotContain("using System.Linq;");
    }

    /// <summary>
    /// Removes a single unused using directive from the source.
    /// </summary>
    /// <param name="usingDirective">The unused using directive.</param>
    [Theory]
    [InlineData("using System;")]
    [InlineData("using System.Collections.Generic;")]
    [InlineData("using System.Linq;")]
    [InlineData("using static System.Console;")]
    [InlineData("using MyAlias = System.Collections.Generic.List<string>;")]
    public void CleanupUsingsInSource_WithSingleUnusedUsing_ShouldRemoveIt(string usingDirective)
    {
        // Arrange
        var sourceCode = $@"
{usingDirective}

public class TestClass
{{
    public void TestMethod()
    {{
        var x = 1;
    }}
}}";

        // Act
        var result = CleanupUsingsTool.CleanupUsingsInSource(sourceCode);

        // Assert
        result.ShouldNotContain(usingDirective);
        result.ShouldContain("public class TestClass");
    }

    private void CreateTestSolution()
    {
        // Create a simple test solution structure
        var solutionContent = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""TestProject"", ""TestProject.csproj"", ""{12345678-1234-1234-1234-123456789012}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{12345678-1234-1234-1234-123456789012}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{12345678-1234-1234-1234-123456789012}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{12345678-1234-1234-1234-123456789012}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{12345678-1234-1234-1234-123456789012}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
EndGlobal";

        var projectContent = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>";

        File.WriteAllText(_testSolutionPath, solutionContent);
        File.WriteAllText(Path.Combine(_testDirectory, "TestProject.csproj"), projectContent);
    }

    /// <summary> /// Dispose(). /// </summary>
    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}
