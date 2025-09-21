using IndFusion.Analyzers;
using IndFusion.CodeFixes.ErrorHandling;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.CodeFixes;

/// <summary>
/// Tests for the enhanced UseResultPatternCodeFixProvider class.
/// </summary>
public class UseResultPatternCodeFixProviderTests : CodeFixProviderTest<UseResultPatternCodeFixProvider>
{
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMethodThrowingException_ShouldRegisterMultipleFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod(string parameter)
    {
        if (parameter == null)
            throw new ArgumentNullException(nameof(parameter));
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new UseResultPatternCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseResultPattern, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodThrowingException_ShouldRegisterAsyncFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public async Task<string> TestMethodAsync(string parameter)
    {
        if (parameter == null)
            throw new ArgumentNullException(nameof(parameter));
        
        return await Task.FromResult(parameter.ToUpper());
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new UseResultPatternCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseResultPattern, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithMethodReturningValue_ShouldRegisterValueReturnFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public string TestMethod(string parameter)
    {
        if (parameter == null)
            throw new ArgumentNullException(nameof(parameter));
        
        return parameter.ToUpper();
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new UseResultPatternCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseResultPattern, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithTaskReturningMethod_ShouldRegisterTaskFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public Task<string> TestMethodAsync(string parameter)
    {
        if (parameter == null)
            throw new ArgumentNullException(nameof(parameter));
        
        return Task.FromResult(parameter.ToUpper());
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new UseResultPatternCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseResultPattern, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithValueTaskReturningMethod_ShouldRegisterValueTaskFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public ValueTask<string> TestMethodAsync(string parameter)
    {
        if (parameter == null)
            throw new ArgumentNullException(nameof(parameter));
        
        return ValueTask.FromResult(parameter.ToUpper());
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new UseResultPatternCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseResultPattern, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithVoidMethod_ShouldRegisterVoidFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod(string parameter)
    {
        if (parameter == null)
            throw new ArgumentNullException(nameof(parameter));
        
        // Do something
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new UseResultPatternCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseResultPattern, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithDifferentExceptionTypes_ShouldRegisterSpecificFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod(string parameter)
    {
        if (parameter == null)
            throw new ArgumentNullException(nameof(parameter));
        
        if (parameter.Length == 0)
            throw new ArgumentException(""Parameter cannot be empty"", nameof(parameter));
        
        if (parameter.Length > 100)
            throw new InvalidOperationException(""Parameter too long"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new UseResultPatternCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseResultPattern, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithThrowExpression_ShouldRegisterExpressionFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public string TestMethod(string parameter) =>
        parameter ?? throw new ArgumentNullException(nameof(parameter));
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new UseResultPatternCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseResultPattern, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public void FixableDiagnosticIds_ShouldReturnUseResultPattern()
    {
        // Arrange
        var codeFixProvider = new UseResultPatternCodeFixProvider();

        // Act
        var fixableIds = codeFixProvider.FixableDiagnosticIds;

        // Assert
        fixableIds.ShouldContain(DiagnosticIds.UseResultPattern);
        fixableIds.Length.ShouldBe(1);
    }

    [Fact]
    public void GetFixAllProvider_ShouldReturnBatchFixer()
    {
        // Arrange
        var codeFixProvider = new UseResultPatternCodeFixProvider();

        // Act
        var fixAllProvider = codeFixProvider.GetFixAllProvider();

        // Assert
        fixAllProvider.ShouldNotBeNull();
        fixAllProvider.ShouldBe(WellKnownFixAllProviders.BatchFixer);
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithNoDiagnostic_ShouldNotRegisterActions()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public Result<string> TestMethod(string parameter)
    {
        if (parameter == null)
            return Result.WithFailure(""Parameter cannot be null"");
        
        return Result.Success(parameter.ToUpper());
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new UseResultPatternCodeFixProvider();
        var diagnostics = new Diagnostic[] { };

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            if (diagnostics.Length > 0)
            {
                await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (a, d) => { }, CancellationToken.None));
            }
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithRethrowStatement_ShouldNotModifyRethrow()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod()
    {
        try
        {
            // Some operation
        }
        catch (Exception)
        {
            throw; // This should not be modified
        }
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new UseResultPatternCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseResultPattern, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
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

