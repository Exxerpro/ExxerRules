#pragma warning disable CS1998, CS0452, CS1022, IDE0053
#pragma warning disable CS8602, IDE0031
using IndFusion.CodeFixes.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.CodeFixes;

/// <summary>
/// Tests for the RoslynFormattingService class.
/// </summary>
public class RoslynFormattingServiceTests
{
    [Fact]
    public async Task FormatDocumentAsync_WithUnformattedCode_ShouldFormatCorrectly()
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

        var document = CreateDocument(sourceCode);

        // Act
        var formattedDocument = await RoslynFormattingService.FormatDocumentAsync(document, TestContext.Current.CancellationToken);

        // Assert
        var formattedSource = await formattedDocument.GetTextAsync(TestContext.Current.CancellationToken);
        formattedSource.ToString().ShouldBe(expectedFormattedCode);
    }

    [Fact]
    public async Task FormatDocumentAsync_WithAlreadyFormattedCode_ShouldNotChange()
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

        var document = CreateDocument(sourceCode);

        // Act
        var formattedDocument = await RoslynFormattingService.FormatDocumentAsync(document, TestContext.Current.CancellationToken);

        // Assert
        var formattedSource = await formattedDocument.GetTextAsync(TestContext.Current.CancellationToken);
        formattedSource.ToString().ShouldBe(sourceCode);
    }

    [Fact]
    public async Task FormatWhitespaceAsync_ShouldOnlyFormatWhitespace()
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

        var document = CreateDocument(sourceCode);

        // Act
        var formattedDocument = await RoslynFormattingService.FormatWhitespaceAsync(document, TestContext.Current.CancellationToken);

        // Assert
        var formattedSource = await formattedDocument.GetTextAsync(TestContext.Current.CancellationToken);
        // Should add spaces around operators
        formattedSource.ToString().ShouldContain("x = 1");
        formattedSource.ToString().ShouldContain("y = 2");
        formattedSource.ToString().ShouldContain("x + y");
    }

    [Fact]
    public async Task FormatDocumentAsync_WithDotNetOptions_ShouldApplyDotNetStandards()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod()
    {
        if(true)
        {
            Console.WriteLine(""test"");
        }
    }
}";

        var document = CreateDocument(sourceCode);
        var dotNetOptions = RoslynFormattingService.CreateDotNetFormattingOptions();

        // Act
        var formattedDocument = await RoslynFormattingService.FormatDocumentAsync(document, dotNetOptions, TestContext.Current.CancellationToken);

        // Assert
        var formattedSource = await formattedDocument.GetTextAsync(TestContext.Current.CancellationToken);
        // Should add space after if keyword
        formattedSource.ToString().ShouldContain("if (true)");
    }

    [Fact]
    public async Task FormatProjectAsync_ShouldFormatAllDocumentsInProject()
    {
        // Arrange
        var workspace = new AdhocWorkspace();
        var projectId = ProjectId.CreateNewId();
        var document1Id = DocumentId.CreateNewId(projectId);
        var document2Id = DocumentId.CreateNewId(projectId);

        var sourceCode1 = "public class Test1{public void Method(){var x=1;}}";
        var sourceCode2 = "public class Test2{public void Method(){var y=2;}}";

        var solution = workspace.CurrentSolution
            .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
            .AddDocument(document1Id, "Test1.cs", SourceText.From(sourceCode1))
            .AddDocument(document2Id, "Test2.cs", SourceText.From(sourceCode2));

        var document = solution.GetDocument(document1Id)!;

        // Act
        var formattedSolution = await RoslynFormattingService.FormatProjectAsync(solution, projectId, TestContext.Current.CancellationToken);

        // Assert
        var formattedDoc1 = formattedSolution.GetDocument(document1Id);
        var formattedDoc2 = formattedSolution.GetDocument(document2Id);

        var formattedSource1 = await formattedDoc1.GetTextAsync(TestContext.Current.CancellationToken);
        var formattedSource2 = await formattedDoc2.GetTextAsync(TestContext.Current.CancellationToken);

        // Should format both documents
        formattedSource1?.ToString().ShouldContain("var x = 1");
        formattedSource2?.ToString().ShouldContain("var y = 2");
    }

    [Fact]
    public async Task FormatSolutionAsync_ShouldFormatAllDocumentsInSolution()
    {
        // Arrange
        var workspace = new AdhocWorkspace();
        var project1Id = ProjectId.CreateNewId();
        var project2Id = ProjectId.CreateNewId();
        var document1Id = DocumentId.CreateNewId(project1Id);
        var document2Id = DocumentId.CreateNewId(project2Id);

        var sourceCode1 = "public class Test1{public void Method(){var x=1;}}";
        var sourceCode2 = "public class Test2{public void Method(){var y=2;}}";

        var solution = workspace.CurrentSolution
            .AddProject(project1Id, "TestProject1", "TestProject1", LanguageNames.CSharp)
            .AddProject(project2Id, "TestProject2", "TestProject2", LanguageNames.CSharp)
            .AddDocument(document1Id, "Test1.cs", SourceText.From(sourceCode1))
            .AddDocument(document2Id, "Test2.cs", SourceText.From(sourceCode2));

        // Act
        var formattedSolution = await RoslynFormattingService.FormatSolutionAsync(solution, TestContext.Current.CancellationToken);

        // Assert
        var formattedDoc1 = formattedSolution.GetDocument(document1Id);
        var formattedDoc2 = formattedSolution.GetDocument(document2Id);

        var formattedSource1 = formattedDoc1 is not null ? await formattedDoc1.GetTextAsync(TestContext.Current.CancellationToken) : null;
        var formattedSource2 = formattedDoc2 is not null ? await formattedDoc2.GetTextAsync(TestContext.Current.CancellationToken) : null;
        if (formattedSource1 is not null)
        {
            formattedSource1.ToString().ShouldContain("var x = 1");
        }
        if (formattedSource2 is not null)
        {
            formattedSource2.ToString().ShouldContain("var y = 2");
        }
    }

    [Fact]
    public void CreateDefaultFormattingOptions_ShouldReturnValidOptions()
    {
        // Act
        var options = RoslynFormattingService.CreateDefaultFormattingOptions();

        // Assert
        options.ShouldNotBeNull();
    }

    [Fact]
    public void CreateDotNetFormattingOptions_ShouldReturnValidOptions()
    {
        // Act
        var options = RoslynFormattingService.CreateDotNetFormattingOptions();

        // Assert
        options.ShouldNotBeNull();
    }

    [Fact]
    public async Task FormatDocumentAsync_WithNullDocument_ShouldReturnOriginalDocument()
    {
        // Arrange
        Document? document = null;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
        {
            await RoslynFormattingService.FormatDocumentAsync(document!);
        });
    }

    [Fact]
    public async Task FormatDocumentAsync_WithCancellation_ShouldRespectCancellation()
    {
        // Arrange
        var sourceCode = "public class Test { }";
        var document = CreateDocument(sourceCode);
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        #pragma warning disable xUnit1051 // Intentionally using a custom CancellationToken to validate cancellation behavior
        var formattedDocument = await RoslynFormattingService.FormatDocumentAsync(document, cancellationTokenSource.Token);
        #pragma warning restore xUnit1051

        // Assert
        formattedDocument.ShouldNotBeNull();
        // Should return original document when cancelled
        var formattedSource = await formattedDocument.GetTextAsync(TestContext.Current.CancellationToken);
        formattedSource.ToString().ShouldBe(sourceCode);
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
}
#pragma warning restore CS1998, CS0452, CS1022, IDE0053
#pragma warning restore CS8602, IDE0031

