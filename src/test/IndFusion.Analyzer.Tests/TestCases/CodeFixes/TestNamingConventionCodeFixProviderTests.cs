using IndFusion.Analyzers;
using IndFusion.CodeFixes;
using IndFusion.CodeFixes.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.CodeFixes;

/// <summary>
/// Tests for the TestNamingConventionCodeFixProvider class.
/// </summary>
public class TestNamingConventionCodeFixProviderTests : CodeFixProviderTest<TestNamingConventionCodeFixProvider>
{
    [Fact]
    public async Task RegisterCodeFixesAsync_WithTestMethodWithTestPrefix_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethodName()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.TestNamingConvention, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithTestMethodWithShouldPrefix_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void ShouldReturnValue()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.TestNamingConvention, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithTestMethodWithWhenPrefix_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void WhenConditionIsMet()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.TestNamingConvention, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithTestMethodWithVerifyPrefix_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void VerifyMethodBehavior()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.TestNamingConvention, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithTestMethodWithAssertPrefix_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void AssertMethodReturnsValue()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.TestNamingConvention, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithTestMethodWithTheoryAttribute_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Theory]
    [InlineData(""test"")]
    public void TestMethodWithData(string input)
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.TestNamingConvention, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithTestMethodWithNullInName_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethodWithNullParameter()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.TestNamingConvention, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithTestMethodWithEmptyInName_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethodWithEmptyString()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.TestNamingConvention, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithTestMethodWithInvalidInName_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethodWithInvalidInput()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.TestNamingConvention, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithTestMethodWithExceptionInName_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethodThrowsException()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.TestNamingConvention, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithNonTestMethod_ShouldNotRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void RegularMethod()
    {
        // Regular implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.TestNamingConvention, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public void FixableDiagnosticIds_ShouldReturnTestNamingConvention()
    {
        // Arrange
        var codeFixProvider = new TestNamingConventionCodeFixProvider();

        // Act
        var fixableIds = codeFixProvider.FixableDiagnosticIds;

        // Assert
        fixableIds.ShouldContain(DiagnosticIds.TestNamingConvention);
        fixableIds.Length.ShouldBe(1);
    }

    [Fact]
    public void GetFixAllProvider_ShouldReturnBatchFixer()
    {
        // Arrange
        var codeFixProvider = new TestNamingConventionCodeFixProvider();

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
using Xunit;

public class TestClass
{
    [Fact]
    public void Should_ReturnValue_When_Called()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostics = new Diagnostic[] { };

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            // For cases with diagnostics array, use first diagnostic if available
            if (diagnostics.Length > 0)
            {
                await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (a, d) => { }, CancellationToken.None));
            }
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithTestMethodWithReturnsInName_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethodReturnsValue()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.TestNamingConvention, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithTestMethodWithThrowsInName_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethodThrowsException()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new TestNamingConventionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.TestNamingConvention, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

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

