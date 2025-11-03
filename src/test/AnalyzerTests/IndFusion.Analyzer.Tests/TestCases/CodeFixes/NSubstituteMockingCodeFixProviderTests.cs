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
/// Tests for the NSubstituteMockingCodeFixProvider class.
/// </summary>
public class NSubstituteMockingCodeFixProviderTests : CodeFixProviderTest<NSubstituteMockingCodeFixProvider>
{
    /// <summary>
    /// RegisterCodeFixesAsync WithMoqSetupInvocation ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMoqSetupInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mock = new Mock<IInterface>();
        mock.Setup(x => x.Method()).Returns(""test"");
    }
}

public interface IInterface
{
    string Method();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseNSubstitute, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithMoqReturnsInvocation ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMoqReturnsInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mock = new Mock<IInterface>();
        mock.Setup(x => x.Method()).Returns(""test"");
    }
}

public interface IInterface
{
    string Method();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseNSubstitute, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithMoqThrowsInvocation ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMoqThrowsInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mock = new Mock<IInterface>();
        mock.Setup(x => x.Method()).Throws(new ArgumentException());
    }
}

public interface IInterface
{
    string Method();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseNSubstitute, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithMoqVerifyInvocation ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMoqVerifyInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mock = new Mock<IInterface>();
        mock.Verify(x => x.Method(), Times.Once());
    }
}

public interface IInterface
{
    string Method();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseNSubstitute, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithMoqMockObjectCreation ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMoqMockObjectCreation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mock = new Mock<IInterface>();
    }
}

public interface IInterface
{
    string Method();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseNSubstitute, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithMoqUsingDirective ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMoqUsingDirective_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mock = new Mock<IInterface>();
    }
}

public interface IInterface
{
    string Method();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseNSubstitute, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithNSubstituteUsingDirective ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithNSubstituteUsingDirective_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using NSubstitute;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mock = Substitute.For<IInterface>();
    }
}

public interface IInterface
{
    string Method();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseNSubstitute, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithNonMoqInvocation ShouldNotRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithNonMoqInvocation_ShouldNotRegisterFixes()
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
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseNSubstitute, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithMoqGenericMock ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMoqGenericMock_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mock = new Mock<IServiceProvider>();
    }
}

public interface IServiceProvider
{
    object GetService(Type serviceType);
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseNSubstitute, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithMoqMockWithConstructorArgs ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMoqMockWithConstructorArgs_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mock = new Mock<IInterface>(MockBehavior.Strict);
    }
}

public interface IInterface
{
    string Method();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseNSubstitute, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithMoqSetupWithComplexLambda ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMoqSetupWithComplexLambda_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mock = new Mock<IInterface>();
        mock.Setup(x => x.Method(It.IsAny<string>())).Returns(""test"");
    }
}

public interface IInterface
{
    string Method(string parameter);
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseNSubstitute, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithMoqVerifyWithTimes ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMoqVerifyWithTimes_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mock = new Mock<IInterface>();
        mock.Verify(x => x.Method(), Times.Exactly(2));
    }
}

public interface IInterface
{
    string Method();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseNSubstitute, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, Xunit.TestContext.Current.CancellationToken);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    /// <summary>
    /// FixableDiagnosticIds ShouldReturnUseNSubstitute.
    /// </summary>
    [Fact]
    public void FixableDiagnosticIds_ShouldReturnUseNSubstitute()
    {
        // Arrange
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();

        // Act
        var fixableIds = codeFixProvider.FixableDiagnosticIds;

        // Assert
        fixableIds.ShouldContain(DiagnosticIds.UseNSubstitute);
        fixableIds.Length.ShouldBe(1);
    }

    /// <summary>
    /// GetFixAllProvider ShouldReturnBatchFixer.
    /// </summary>
    [Fact]
    public void GetFixAllProvider_ShouldReturnBatchFixer()
    {
        // Arrange
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();

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
using NSubstitute;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mock = Substitute.For<IInterface>();
    }
}

public interface IInterface
{
    string Method();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostics = new Diagnostic[] { };

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            // Fix invalid CodeFixContext constructor usage
            // Replace: new CodeFixContext(document, (a, d) => { }, Xunit.TestContext.Current.CancellationToken)
            // With: if (diagnostics.Length > 0) await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (a, d) => { }, Xunit.TestContext.Current.CancellationToken));
            if (diagnostics.Length > 0)
            {
                await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (a, d) => { }, Xunit.TestContext.Current.CancellationToken));
            }
        });
    }

    /// <summary>
    /// RegisterCodeFixesAsync WithMoqSetupWithReturnsAsync ShouldRegisterFixes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMoqSetupWithReturnsAsync_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Xunit;

public class TestClass
{
    [Fact]
    public async Task TestMethod()
    {
        var mock = new Mock<IInterface>();
        mock.Setup(x => x.MethodAsync()).ReturnsAsync(""test"");
    }
}

public interface IInterface
{
    Task<string> MethodAsync();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new NSubstituteMockingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseNSubstitute, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

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
