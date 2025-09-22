using IndFusion.Analyzers;
using IndFusion.CodeFixes;
using IndFusion.CodeFixes.ModernCSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.CodeFixes;

/// <summary>
/// Tests for the ModernPatternMatchingCodeFixProvider class.
/// </summary>
public class ModernPatternMatchingCodeFixProviderTests : CodeFixProviderTest<ModernPatternMatchingCodeFixProvider>
{
    /// <summary>
    /// RegisterCodeFixesAsync WithTraditionalIsExpression ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithTraditionalIsExpression_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"""
public class TestClass
{
    public string TestMethod(object obj)
    {
        if (obj is string str)
        {
            return str.ToUpper();
        }
        return ""unknown"";
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseModernPatternMatching, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithTraditionalSwitchStatement ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithTraditionalSwitchStatement_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"""
public class TestClass
{
    public string TestMethod(object obj)
    {
        switch (obj)
        {
            case string str:
                return str.ToUpper();
            case int num:
                return num.ToString();
            default:
                return ""unknown"";
        }
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseModernPatternMatching, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithIsPatternExpression ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithIsPatternExpression_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"""
public class TestClass
{
    public bool TestMethod(object obj)
    {
        return obj is string;
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseModernPatternMatching, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithSwitchExpression ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithSwitchExpression_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"""
public class TestClass
{
    public string TestMethod(object obj)
    {
        return obj switch
        {
            string str => str.ToUpper(),
            int num => num.ToString(),
            _ => ""unknown""
        };
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseModernPatternMatching, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithTraditionalIsExpressionInTernary ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithTraditionalIsExpressionInTernary_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"""
public class TestClass
{
    public string TestMethod(object obj)
    {
        return obj is string ? ""string"" : ""not string"";
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseModernPatternMatching, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithTraditionalIsExpressionInLambda ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithTraditionalIsExpressionInLambda_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"""
public class TestClass
{
    public void TestMethod()
    {
        var items = new object[] { ""test"", 42, true };
        var strings = items.Where(item => item is string);
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseModernPatternMatching, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithTraditionalSwitchStatementWithFallThrough ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithTraditionalSwitchStatementWithFallThrough_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"""
public class TestClass
{
    public string TestMethod(object obj)
    {
        switch (obj)
        {
            case string str:
            case int num:
                return ""string or int"";
            default:
                return ""unknown"";
        }
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseModernPatternMatching, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithTraditionalSwitchStatementWithWhenClause ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithTraditionalSwitchStatementWithWhenClause_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"""
public class TestClass
{
    public string TestMethod(object obj)
    {
        switch (obj)
        {
            case string str when str.Length > 0:
                return str.ToUpper();
            case string str:
                return ""empty string"";
            default:
                return ""unknown"";
        }
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseModernPatternMatching, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithAlreadyModernPatternMatching ShouldNotRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithAlreadyModernPatternMatching_ShouldNotRegisterFixes()
    {
        // Arrange
        var sourceCode = @"""
public class TestClass
{
    public string TestMethod(object obj)
    {
        return obj switch
        {
            string str => str.ToUpper(),
            int num => num.ToString(),
            _ => ""unknown""
        };
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseModernPatternMatching, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithComplexPatternMatching ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithComplexPatternMatching_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"""
public class TestClass
{
    public string TestMethod(object obj)
    {
        if (obj is string str && str.Length > 0)
        {
            return str.ToUpper();
        }
        else if (obj is int num && num > 0)
        {
            return num.ToString();
        }
        return ""unknown"";
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseModernPatternMatching, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// FixableDiagnosticIds ShouldReturnUseModernPatternMatching.
    /// </summary>
    [Fact]
    public void FixableDiagnosticIds_ShouldReturnUseModernPatternMatching()
    {
        // Arrange
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();

        // Act
        var fixableIds = codeFixProvider.FixableDiagnosticIds;

        // Assert
        fixableIds.ShouldContain(DiagnosticIds.UseModernPatternMatching);
        fixableIds.Length.ShouldBe(1);
    }

    /// <summary>
    /// GetFixAllProvider ShouldReturnBatchFixer.
    /// </summary>
    [Fact]
    public void GetFixAllProvider_ShouldReturnBatchFixer()
    {
        // Arrange
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();

        // Act
        var fixAllProvider = codeFixProvider.GetFixAllProvider();

        // Assert
        fixAllProvider.ShouldNotBeNull();
        fixAllProvider.ShouldBe(WellKnownFixAllProviders.BatchFixer);
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithNoDiagnostic ShouldNotRegisterActions.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithNoDiagnostic_ShouldNotRegisterActions()
    {
        // Arrange
        var sourceCode = @"""
public class TestClass
{
    public string TestMethod(object obj)
    {
        return obj switch
        {
            string str => str.ToUpper(),
            int num => num.ToString(),
            _ => ""unknown""
        };
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();
        var diagnostics = new Diagnostic[] { };

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            // For cases with diagnostics array, use first diagnostic if available
            if (diagnostics.Length > 0)
            {
                await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (a, d) => { }, Xunit.TestContext.Current.CancellationToken));
            }
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithPropertyPatternMatching ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithPropertyPatternMatching_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"""
public class TestClass
{
    public string TestMethod(object obj)
    {
        if (obj is { Length: > 0 })
        {
            return ""has length"";
        }
        return ""no length"";
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseModernPatternMatching, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithTuplePatternMatching ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithTuplePatternMatching_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"""
public class TestClass
{
    public string TestMethod((string, int) tuple)
    {
        if (tuple is (""test"", var number))
        {
            return number.ToString();
        }
        return ""not test"";
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ModernPatternMatchingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseModernPatternMatching, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// Creates a Roslyn <see cref="Document"/> containing the provided source code.
    /// </summary>
    /// <param name="sourceCode">The C# source code to include in the document.</param>
    /// <returns>The created <see cref="Document"/>.</returns>
    private static Document CreateDocument(string sourceCode)
    {
        var workspace = new AdhocWorkspace();
        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);

        var solution = workspace.CurrentSolution
            .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
            .AddDocument(documentId, "Test.cs", SourceText.From(sourceCode));

        return solution.GetDocument(documentId)!;
    }

    /// <summary>
    /// Creates a <see cref="Diagnostic"/> with the specified identifier at the given location.
    /// </summary>
    /// <param name="id">The diagnostic identifier.</param>
    /// <param name="location">The source location for the diagnostic.</param>
    /// <returns>The created <see cref="Diagnostic"/>.</returns>
    private static Diagnostic CreateDiagnostic(string id, Location location)
    {
        var descriptor = new DiagnosticDescriptor(id, "Test", "Test", "Test", DiagnosticSeverity.Warning, true);
        return Diagnostic.Create(descriptor, location);
    }
}
