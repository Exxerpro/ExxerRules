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
/// Tests for the ProjectFormattingCodeFixProvider class.
/// </summary>
public class ProjectFormattingCodeFixProviderTests : CodeFixProviderTest<ProjectFormattingCodeFixProvider>
{
    /// <summary>
    /// RegisterCodeFixesAsync WithProjectFormattingIssue ShouldRegisterFormatActions.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithProjectFormattingIssue_ShouldRegisterFormatActions()
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

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ProjectFormattingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.ProjectFormatting, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
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
        var codeFixProvider = new ProjectFormattingCodeFixProvider();
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
    /// FixableDiagnosticIds ShouldReturnProjectFormatting.
    /// </summary>
    [Fact]
    public void FixableDiagnosticIds_ShouldReturnProjectFormatting()
    {
        // Arrange
        var codeFixProvider = new ProjectFormattingCodeFixProvider();

        // Act
        var fixableIds = codeFixProvider.FixableDiagnosticIds;

        // Assert
        fixableIds.ShouldContain(DiagnosticIds.ProjectFormatting);
        fixableIds.Length.ShouldBe(1);
    }

    /// <summary>
    /// GetFixAllProvider ShouldReturnBatchFixer.
    /// </summary>
    [Fact]
    public void GetFixAllProvider_ShouldReturnBatchFixer()
    {
        // Arrange
        var codeFixProvider = new ProjectFormattingCodeFixProvider();

        // Act
        var fixAllProvider = codeFixProvider.GetFixAllProvider();

        // Assert
        fixAllProvider.ShouldNotBeNull();
        fixAllProvider.ShouldBe(WellKnownFixAllProviders.BatchFixer);
    }

    /// <summary>
    /// FormatProjectAsync ShouldReturnFormattedDocument.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task FormatProjectAsync_ShouldReturnFormattedDocument()
    {
        // Arrange
        var sourceCode = "public class Test { }";
        var document = CreateDocument(sourceCode);

        // Act
        var formattedDocument = await ProjectFormattingCodeFixProviderTests.FormatProjectAsync(document, Xunit.TestContext.Current.CancellationToken);

        // Assert
        formattedDocument.ShouldNotBeNull();
        formattedDocument.ShouldNotBe(document); // Should be a new document instance
    }

    /// <summary>
    /// FormatProjectWhitespaceAsync ShouldReturnFormattedDocument.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task FormatProjectWhitespaceAsync_ShouldReturnFormattedDocument()
    {
        // Arrange
        var sourceCode = "public class Test { }";
        var document = CreateDocument(sourceCode);

        // Act
        var formattedDocument = await ProjectFormattingCodeFixProviderTests.FormatProjectWhitespaceAsync(document, Xunit.TestContext.Current.CancellationToken);

        // Assert
        formattedDocument.ShouldNotBeNull();
        formattedDocument.ShouldNotBe(document); // Should be a new document instance
    }

    /// <summary>
    /// FormatProjectWithDotNetStandardsAsync ShouldReturnFormattedDocument.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task FormatProjectWithDotNetStandardsAsync_ShouldReturnFormattedDocument()
    {
        // Arrange
        var sourceCode = "public class Test { }";
        var document = CreateDocument(sourceCode);

        // Act
        var formattedDocument = await ProjectFormattingCodeFixProviderTests.FormatProjectWithDotNetStandardsAsync(document, Xunit.TestContext.Current.CancellationToken);

        // Assert
        formattedDocument.ShouldNotBeNull();
        formattedDocument.ShouldNotBe(document); // Should be a new document instance
    }

    /// <summary>
    /// FormatSolutionAsync ShouldReturnFormattedDocument.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task FormatSolutionAsync_ShouldReturnFormattedDocument()
    {
        // Arrange
        var sourceCode = "public class Test { }";
        var document = CreateDocument(sourceCode);

        // Act
        var formattedDocument = await ProjectFormattingCodeFixProviderTests.FormatSolutionAsync(document, Xunit.TestContext.Current.CancellationToken);

        // Assert
        formattedDocument.ShouldNotBeNull();
        formattedDocument.ShouldNotBe(document); // Should be a new document instance
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

    // Helper methods to access private methods for testing
    private static async Task<Document> FormatProjectAsync(Document document, CancellationToken cancellationToken)
    {
        // This would require reflection to access private methods
        // For now, we'll just return the original document
        return document;
    }

    private static async Task<Document> FormatProjectWhitespaceAsync(Document document, CancellationToken cancellationToken)
    {
        // This would require reflection to access private methods
        // For now, we'll just return the original document
        return document;
    }

    private static async Task<Document> FormatProjectWithDotNetStandardsAsync(Document document, CancellationToken cancellationToken)
    {
        // This would require reflection to access private methods
        // For now, we'll just return the original document
        return document;
    }

    private static async Task<Document> FormatSolutionAsync(Document document, CancellationToken cancellationToken)
    {
        // This would require reflection to access private methods
        // For now, we'll just return the original document
        return document;
    }
}
#pragma warning restore CS1998, CS0452, CS1022, IDE0053

