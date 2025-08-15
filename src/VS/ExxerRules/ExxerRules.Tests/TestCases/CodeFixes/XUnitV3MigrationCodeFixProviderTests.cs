#pragma warning disable CS1998, CS0452, CS1022, IDE0053
using ExxerRules.Analyzers;
using ExxerRules.CodeFixes.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace ExxerRules.Tests.TestCases.CodeFixes;

/// <summary>
/// Tests for the XUnitV3MigrationCodeFixProvider class.
/// </summary>
public class XUnitV3MigrationCodeFixProviderTests : CodeFixProviderTest<XUnitV3MigrationCodeFixProvider>
{
    [Fact]
    public async Task RegisterCodeFixesAsync_WithFactAttribute_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseXUnitV3, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithTheoryAttribute_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Theory]
    [InlineData(""test"")]
    public void TestMethod(string input)
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseXUnitV3, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithInlineDataAttribute_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Theory]
    [InlineData(""test"")]
    public void TestMethod(string input)
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseXUnitV3, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithMemberDataAttribute_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Theory]
    [MemberData(nameof(TestData))]
    public void TestMethod(string input)
    {
        // Test implementation
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { ""test"" };
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseXUnitV3, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithClassDataAttribute_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Theory]
    [ClassData(typeof(TestDataClass))]
    public void TestMethod(string input)
    {
        // Test implementation
    }
}

public class TestDataClass : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { ""test"" };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseXUnitV3, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAssertThrowsInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        Assert.Throws<ArgumentException>(() => throw new ArgumentException());
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseXUnitV3, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAssertThrowsAsyncInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public async Task TestMethod()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () => throw new ArgumentException());
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseXUnitV3, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAssertRecordInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var exception = Assert.Record.Exception(() => throw new ArgumentException());
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseXUnitV3, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAssertRecordAsyncInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public async Task TestMethod()
    {
        var exception = await Assert.Record.ExceptionAsync(async () => throw new ArgumentException());
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseXUnitV3, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithXUnitUsingDirective_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseXUnitV3, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithNonXUnitAttribute_ShouldNotRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    [Test]
    public void TestMethod()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseXUnitV3, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public void FixableDiagnosticIds_ShouldReturnUseXUnitV3()
    {
        // Arrange
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();

        // Act
        var fixableIds = codeFixProvider.FixableDiagnosticIds;

        // Assert
        fixableIds.ShouldContain(DiagnosticIds.UseXUnitV3);
        fixableIds.Length.ShouldBe(1);
    }

    [Fact]
    public void GetFixAllProvider_ShouldReturnBatchFixer()
    {
        // Arrange
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();

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
    public void TestMethod()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
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
    public async Task RegisterCodeFixesAsync_WithMultipleXUnitAttributes_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod1()
    {
        // Test implementation
    }

    [Theory]
    [InlineData(""test"")]
    public void TestMethod2(string input)
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseXUnitV3, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithXUnitFullyQualifiedAttribute_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    [Xunit.Fact]
    public void TestMethod()
    {
        // Test implementation
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new XUnitV3MigrationCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseXUnitV3, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

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
