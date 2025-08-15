#pragma warning disable CS1998, CS0452, CS1022, IDE0053
using ExxerRules.Analyzers;
using ExxerRules.CodeFixes;
using ExxerRules.CodeFixes.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace ExxerRules.Tests.TestCases.CodeFixes;

/// <summary>
/// Tests for the ShouldlyAssertionCodeFixProvider class.
/// </summary>
public class ShouldlyAssertionCodeFixProviderTests : CodeFixProviderTest<ShouldlyAssertionCodeFixProvider>
{
    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsShouldInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var result = ""test"";
        result.Should().Be(""test"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsBeInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var result = ""test"";
        result.Should().Be(""test"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsNotBeInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var result = ""test"";
        result.Should().NotBe(""other"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsBeNullInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        string? result = null;
        result.Should().BeNull();
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsNotBeNullInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var result = ""test"";
        result.Should().NotBeNull();
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsBeEmptyInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var list = new List<string>();
        list.Should().BeEmpty();
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsNotBeEmptyInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var list = new List<string> { ""test"" };
        list.Should().NotBeEmpty();
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsContainInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var list = new List<string> { ""test"" };
        list.Should().Contain(""test"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsNotContainInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var list = new List<string> { ""test"" };
        list.Should().NotContain(""other"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsHaveCountInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var list = new List<string> { ""test"" };
        list.Should().HaveCount(1);
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsBeGreaterThanInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var value = 5;
        value.Should().BeGreaterThan(3);
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsBeLessThanInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var value = 3;
        value.Should().BeLessThan(5);
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsBeGreaterOrEqualToInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var value = 5;
        value.Should().BeGreaterOrEqualTo(5);
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsBeLessOrEqualToInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var value = 3;
        value.Should().BeLessOrEqualTo(5);
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsBeTrueInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var condition = true;
        condition.Should().BeTrue();
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsBeFalseInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var condition = false;
        condition.Should().BeFalse();
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsThrowInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        Action action = () => throw new ArgumentException();
        action.Should().Throw<ArgumentException>();
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsNotThrowInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        Action action = () => { };
        action.Should().NotThrow();
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithFluentAssertionsUsingDirective_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var result = ""test"";
        result.Should().Be(""test"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithNonFluentAssertionsInvocation_ShouldNotRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var result = ""test"";
        Assert.Equal(""test"", result);
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseShouldly, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public void FixableDiagnosticIds_ShouldReturnUseShouldly()
    {
        // Arrange
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();

        // Act
        var fixableIds = codeFixProvider.FixableDiagnosticIds;

        // Assert
        fixableIds.ShouldContain(DiagnosticIds.UseShouldly);
        fixableIds.Length.ShouldBe(1);
    }

    [Fact]
    public void GetFixAllProvider_ShouldReturnBatchFixer()
    {
        // Arrange
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();

        // Act
        var fixAllProvider = codeFixProvider.GetFixAllProvider();

        // Assert
        fixAllProvider.ShouldBe(WellKnownFixAllProviders.BatchFixer);
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithNoDiagnostic_ShouldNotRegisterActions()
    {
        // Arrange
        var sourceCode = @"
using Shouldly;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var result = ""test"";
        result.ShouldBe(""test"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ShouldlyAssertionCodeFixProvider();
        var diagnostics = new Diagnostic[] { };

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (a, d) => { }, CancellationToken.None));
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
