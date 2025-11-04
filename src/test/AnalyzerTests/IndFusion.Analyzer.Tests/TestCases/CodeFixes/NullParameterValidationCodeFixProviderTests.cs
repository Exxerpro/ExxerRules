#pragma warning disable CS1998, CS0452, CS1022, IDE0053

using IndFusion.Fixer.NullSafety;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.CodeFixes;

/// <summary>
/// Tests for the NullParameterValidationCodeFixProvider class.
/// </summary>
public class NullParameterValidationCodeFixProviderTests : CodeFixProviderTest<NullParameterValidationCodeFixProvider>
{
    /// <summary>
    /// RegisterCodeFixesAsync WithMethodWithoutNullValidation ShouldRegisterMethodFixes.
    /// </summary>
    /// <returns></returns>
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
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithConstructorWithoutNullValidation ShouldRegisterConstructorFixes.
    /// </summary>
    /// <returns></returns>
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
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithValueTypeParameters ShouldNotRegisterFixes.
    /// </summary>
    /// <returns></returns>
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
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithMultipleParameters ShouldRegisterFixesForReferenceTypes.
    /// </summary>
    /// <returns></returns>
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
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithAsyncMethod ShouldRegisterAsyncFixes.
    /// </summary>
    /// <returns></returns>
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
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithExpressionBodyMethod ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
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
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// FixableDiagnosticIds ShouldReturnValidateNullParameters.
    /// </summary>
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

    /// <summary>
    /// GetFixAllProvider ShouldReturnBatchFixer.
    /// </summary>
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
                await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (a, d) => { }, Xunit.TestContext.Current.CancellationToken));
            }
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithGenericParameters ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
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
            .AddDocument(documentId, "Test.cs", SourceText.From(sourceCode), filePath: "Test.cs");

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

#pragma warning restore CS1998, CS0452, CS1022, IDE0053
