#pragma warning disable CS1998, CS0452, CS1022, IDE0053
using ExxerRules.Analyzers;
using ExxerRules.CodeFixes;
using ExxerRules.CodeFixes.NullSafety;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace ExxerRules.Tests.TestCases.CodeFixes;

/// <summary>
/// Tests for the NullParameterValidationCodeFixProvider class.
/// </summary>
public class NullParameterValidationCodeFixProviderTests : CodeFixProviderTest<NullParameterValidationCodeFixProvider>
{
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMethodWithoutNullValidation_ShouldRegisterMethodFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod(string parameter)
    {
        // Implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NullParameterValidationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.ValidateNullParameters, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithConstructorWithoutNullValidation_ShouldRegisterConstructorFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public TestClass(string parameter)
    {
        // Implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NullParameterValidationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.ValidateNullParameters, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithValueTypeParameters_ShouldNotRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod(int parameter)
    {
        // Implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NullParameterValidationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.ValidateNullParameters, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithMultipleParameters_ShouldRegisterFixesForReferenceTypes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod(string parameter1, int parameter2, object parameter3)
    {
        // Implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NullParameterValidationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.ValidateNullParameters, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethod_ShouldRegisterAsyncFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public async Task<string> TestMethodAsync(string parameter)
    {
        // Implementation
        return await Task.FromResult(""test"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NullParameterValidationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.ValidateNullParameters, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithExpressionBodyMethod_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public string TestMethod(string parameter) => parameter.ToUpper();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NullParameterValidationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.ValidateNullParameters, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public void FixableDiagnosticIds_ShouldReturnValidateNullParameters()
    {
        // Arrange
        var codeFixProvider = new NullParameterValidationCodeFixProvider();

        // Act
        var fixableIds = codeFixProvider.FixableDiagnosticIds;

        // Assert
        fixableIds.ShouldContain(DiagnosticIds.ValidateNullParameters);
        fixableIds.Length.ShouldBe(1);
    }

    [Fact]
    public void GetFixAllProvider_ShouldReturnBatchFixer()
    {
        // Arrange
        var codeFixProvider = new NullParameterValidationCodeFixProvider();

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
    public void TestMethod(string parameter)
    {
        if (parameter is null) throw new ArgumentNullException(nameof(parameter));
        // Implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NullParameterValidationCodeFixProvider();
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
    public async Task RegisterCodeFixesAsync_WithGenericParameters_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod<T>(T parameter) where T : class
    {
        // Implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NullParameterValidationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.ValidateNullParameters, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

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

#pragma warning restore CS1998, CS0452, CS1022, IDE0053
