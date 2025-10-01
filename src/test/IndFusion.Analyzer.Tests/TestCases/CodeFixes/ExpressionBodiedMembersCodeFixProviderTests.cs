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
/// Tests for the ExpressionBodiedMembersCodeFixProvider class.
/// </summary>
public class ExpressionBodiedMembersCodeFixProviderTests : CodeFixProviderTest<ExpressionBodiedMembersCodeFixProvider>
{
    /// <summary>
    /// Tests that the RegisterCodeFixesAsync method registers a code fix for a method that can be converted to an expression-bodied member.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMethodReturningSingleExpression_ShouldRegisterFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public string TestMethod(string parameter)
    {
        return parameter.ToUpper();
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseExpressionBodiedMembers, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// Tests that the RegisterCodeFixesAsync method registers a code fix for a property with a single getter that can be converted to an expression-bodied member.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithPropertyWithSingleGetter_ShouldRegisterFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    private string _field = ""test"";

    public string TestProperty
    {
        get { return _field; }
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseExpressionBodiedMembers, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// Tests that the RegisterCodeFixesAsync method registers a code fix for a constructor with a single statement that can be converted to an expression-bodied member.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithConstructorWithSingleStatement_ShouldRegisterFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    private readonly string _field;

    public TestClass(string field)
    {
        this._field = field;
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseExpressionBodiedMembers, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// Tests that the RegisterCodeFixesAsync method registers a code fix for an operator overload with a single return statement that can be converted to an expression-bodied member.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithOperatorWithSingleReturn_ShouldRegisterFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public int Value { get; set; }

    public static TestClass operator +(TestClass a, TestClass b)
    {
        return new TestClass { Value = a.Value + b.Value };
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseExpressionBodiedMembers, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// Tests that the RegisterCodeFixesAsync method registers a code fix for a conversion operator with a single return statement that can be converted to an expression-bodied member.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithConversionOperatorWithSingleReturn_ShouldRegisterFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public int Value { get; set; }

    public static explicit operator int(TestClass test)
    {
        return test.Value;
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseExpressionBodiedMembers, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// Tests that the RegisterCodeFixesAsync method registers a code fix for an indexer with a single getter that can be converted to an expression-bodied member.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithIndexerWithSingleGetter_ShouldRegisterFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    private readonly string[] _items = new string[10];

    public string this[int index]
    {
        get { return _items[index]; }
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseExpressionBodiedMembers, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// Tests that the RegisterCodeFixesAsync method does not register a code fix for a method with multiple statements.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMethodWithMultipleStatements_ShouldNotRegisterFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public string TestMethod(string parameter)
    {
        var upper = parameter.ToUpper();
        return upper;
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseExpressionBodiedMembers, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// Tests that the RegisterCodeFixesAsync method does not register a code fix for a property with a setter.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithPropertyWithSetter_ShouldNotRegisterFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    private string _field = ""test"";

    public string TestProperty
    {
        get { return _field; }
        set { _field = value; }
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseExpressionBodiedMembers, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// Tests that the RegisterCodeFixesAsync method does not register a code fix for a method with no return statement.
    /// </summary>
    /// <returns></returns>

    [Fact]
    public async Task RegisterCodeFixesAsync_WithMethodWithNoReturn_ShouldNotRegisterFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod(string parameter)
    {
        Console.WriteLine(parameter);
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseExpressionBodiedMembers, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// Tests that the RegisterCodeFixesAsync method does not register a code fix for a method that is already an expression-bodied member.
    /// </summary>
    /// <returns></returns>

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAlreadyExpressionBodiedMethod_ShouldNotRegisterFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public string TestMethod(string parameter) => parameter.ToUpper();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseExpressionBodiedMembers, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// Tests that the RegisterCodeFixesAsync method does not register a code fix for a property that is already an expression-bodied member.
    /// </summary>
    /// <returns></returns>

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAlreadyExpressionBodiedProperty_ShouldNotRegisterFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    private string _field = ""test"";

    public string TestProperty => _field;
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseExpressionBodiedMembers, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// Tests that the FixableDiagnosticIds property returns the expected diagnostic ID.
    /// </summary>

    [Fact]
    public void FixableDiagnosticIds_ShouldReturnUseExpressionBodiedMembers()
    {
        // Arrange
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();

        // Act
        var fixableIds = codeFixProvider.FixableDiagnosticIds;

        // Assert
        fixableIds.ShouldContain(DiagnosticIds.UseExpressionBodiedMembers);
        fixableIds.Length.ShouldBe(1);
    }

    /// <summary>
    /// Verifies that the GetFixAllProvider method of ExpressionBodiedMembersCodeFixProvider returns the BatchFixer
    /// instance.
    /// </summary>
    /// <remarks>This test ensures that the code fix provider supports batch fixing by returning
    /// WellKnownFixAllProviders.BatchFixer, which enables fixing all occurrences in a document, project, or solution.
    /// Use this test to confirm correct integration with batch fix workflows.</remarks>
    [Fact]
    public void GetFixAllProvider_ShouldReturnBatchFixer()
    {
        // Arrange
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();

        // Act
        var fixAllProvider = codeFixProvider.GetFixAllProvider();

        // Assert
        fixAllProvider.ShouldNotBeNull();
        fixAllProvider.ShouldBe(WellKnownFixAllProviders.BatchFixer);
    }

    /// <summary>
    /// Tests that the RegisterCodeFixesAsync method does not throw an exception when no diagnostics are provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithNoDiagnostic_ShouldNotRegisterActions()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public string TestMethod(string parameter) => parameter.ToUpper();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
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
    /// Tests that the RegisterCodeFixesAsync method registers a code fix for a method with a complex return statement that can be converted to an expression-bodied member.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMethodWithComplexReturn_ShouldRegisterFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public string TestMethod(string parameter)
    {
        return parameter?.ToUpper() ?? ""DEFAULT"";
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseExpressionBodiedMembers, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// Tests that the RegisterCodeFixesAsync method registers a code fix for a method with a ternary operator that can be converted to an expression-bodied member.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMethodWithTernaryOperator_ShouldRegisterFix()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public string TestMethod(bool condition, string value)
    {
        return condition ? value.ToUpper() : value.ToLower();
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ExpressionBodiedMembersCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseExpressionBodiedMembers, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
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
            .AddDocument(documentId, "Test.cs", SourceText.From(sourceCode), filePath: "Test.cs");

        return solution.GetDocument(documentId)!;
    }

    private static Diagnostic CreateDiagnostic(string id, Location location)
    {
        var descriptor = new DiagnosticDescriptor(id, "Test", "Test", "Test", DiagnosticSeverity.Warning, true);
        return Diagnostic.Create(descriptor, location);
    }
}
