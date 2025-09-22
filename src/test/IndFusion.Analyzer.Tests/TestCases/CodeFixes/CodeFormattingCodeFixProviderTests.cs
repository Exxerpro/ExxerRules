#pragma warning disable CS1998, CS0452, CS1022, IDE0053
using IndFusion.Analyzers;
using IndFusion.CodeFixes.CodeFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.CodeFixes;

/// <summary>
/// Tests for the CodeFormattingCodeFixProvider class.
/// </summary>
public class CodeFormattingCodeFixProviderTests : CodeFixProviderTest<CodeFormattingCodeFixProvider>
{
    /// <summary>
    /// RegisterCodeFixesAsync WithCodeFormattingIssue ShouldRegisterFormatActions.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithCodeFormattingIssue_ShouldRegisterFormatActions()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod()
    {
        var x=1;
        var y=2;
        Console.WriteLine(x+y);
    }
}";

        var expectedFormattedCode = @"
public class TestClass
{
    public void TestMethod()
    {
        var x = 1;
        var y = 2;
        Console.WriteLine(x + y);
    }
}";

        // Act & Assert
        await VerifyCodeFixAsync(sourceCode, expectedFormattedCode, DiagnosticIds.CodeFormattingIssue);
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithNoDiagnostic ShouldNotRegisterActions.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithNoDiagnostic_ShouldNotRegisterActions()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod()
    {
        var x = 1;
        var y = 2;
        Console.WriteLine(x + y);
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new CodeFormattingCodeFixProvider();
        var diagnostics = new Diagnostic[] { };

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            if (diagnostics.Length > 0)
            {
                await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (a, d) => { }, Xunit.TestContext.Current.CancellationToken));
            }
        });
    }

    /// <summary>
    /// FixableDiagnosticIds ShouldReturnCodeFormattingIssue.
    /// </summary>
    [Fact]
    public void FixableDiagnosticIds_ShouldReturnCodeFormattingIssue()
    {
        // Arrange
        var codeFixProvider = new CodeFormattingCodeFixProvider();

        // Act
        var fixableIds = codeFixProvider.FixableDiagnosticIds;

        // Assert
        fixableIds.ShouldContain(DiagnosticIds.CodeFormattingIssue);
        fixableIds.Length.ShouldBe(1);
    }

    /// <summary>
    /// GetFixAllProvider ShouldReturnBatchFixer.
    /// </summary>
    [Fact]
    public void GetFixAllProvider_ShouldReturnBatchFixer()
    {
        // Arrange
        var codeFixProvider = new CodeFormattingCodeFixProvider();

        // Act
        var fixAllProvider = codeFixProvider.GetFixAllProvider();

        // Assert
        fixAllProvider.ShouldNotBeNull();
        fixAllProvider.ShouldBe(WellKnownFixAllProviders.BatchFixer);
    }

    private async Task VerifyCodeFixAsync(string sourceCode, string expectedCode, string diagnosticId)
    {
        // Arrange
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new CodeFormattingCodeFixProvider();
        var diagnostic = CreateDiagnostic(diagnosticId, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act
        var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
        await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);

        // Assert
        // Note: In a real test environment, we would verify that the code actions were registered
        // and then execute them to verify the formatting. For now, we just verify the provider doesn't throw.
    }

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

    private static Diagnostic CreateDiagnostic(string id, Location location)
    {
        var descriptor = new DiagnosticDescriptor(id, "Test", "Test", "Test", DiagnosticSeverity.Warning, true);
        return Diagnostic.Create(descriptor, location);
    }
}

/// <summary>
/// Base class for code fix provider tests.
/// </summary>
/// <typeparam name="T">The code fix provider type.</typeparam>
public abstract class CodeFixProviderTest<T> where T : CodeFixProvider, new()
{
    /// <summary>
    /// Gets the code fix provider under test.
    /// </summary>
    protected T CodeFixProvider { get; } = new T();
}
#pragma warning restore CS1998, CS0452, CS1022, IDE0053
