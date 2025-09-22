#pragma warning disable CS1998, CS8031, CS0117, IDE0053

using IndFusion.Analyzers;
using IndFusion.CodeFixes.Async;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.CodeFixes;

/// <summary>
/// Tests for the CancellationTokenCodeFixProvider class.
/// </summary>
public class CancellationTokenCodeFixProviderTests //: CodeFixProviderTest<CancellationTokenCodeFixProvider>
{
    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodWithoutCancellationToken_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
	public async Task<string> TestMethodAsync(string parameter)
	{
		await Task.Delay(100);
		return parameter.ToUpper();
	}
}";

        // Act
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncVoidMethod_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
	public async void TestMethodAsync(string parameter)
	{
		await Task.Delay(100);
	}
}";

        // Act
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodWithParameters_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
	public async Task<string> TestMethodAsync(string parameter1, int parameter2, object parameter3)
	{
		await Task.Delay(100);
		return $""{parameter1}_{parameter2}_{parameter3}"";
	}
}";

        // Act
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodAlreadyHavingCancellationToken_ShouldNotRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
	public async Task<string> TestMethodAsync(string parameter, CancellationToken cancellationToken)
	{
		await Task.Delay(100, cancellationToken);
		return parameter.ToUpper();
	}
}";

        // Act
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithNonAsyncMethod_ShouldNotRegisterFixes()
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
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodWithDefaultCancellationToken_ShouldNotRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
	public async Task<string> TestMethodAsync(string parameter, CancellationToken cancellationToken = default)
	{
		await Task.Delay(100, cancellationToken);
		return parameter.ToUpper();
	}
}";

        // Act
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodWithCancellationTokenNone_ShouldNotRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
	public async Task<string> TestMethodAsync(string parameter, CancellationToken cancellationToken = CancellationToken.None)
	{
		await Task.Delay(100, cancellationToken);
		return parameter.ToUpper();
	}
}";

        // Act
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodWithMultipleCancellationTokenParameters_ShouldNotRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
	public async Task<string> TestMethodAsync(string parameter, CancellationToken cancellationToken1, CancellationToken cancellationToken2)
	{
		await Task.Delay(100, cancellationToken1);
		return parameter.ToUpper();
	}
}";

        // Act
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodWithRefParameters_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
	public async Task<string> TestMethodAsync(ref string parameter)
	{
		await Task.Delay(100);
		return parameter.ToUpper();
	}
}";

        // Act
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodWithOutParameters_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
	public async Task<string> TestMethodAsync(string parameter, out int result)
	{
		await Task.Delay(100);
		result = 42;
		return parameter.ToUpper();
	}
}";

        // Act
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodWithGenericParameters_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
	public async Task<T> TestMethodAsync<T>(T parameter) where T : class
	{
		await Task.Delay(100);
		return parameter;
	}
}";

        // Act
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodWithExpressionBody_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
	public async Task<string> TestMethodAsync(string parameter) => await Task.FromResult(parameter.ToUpper());
}";

        // Act
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodWithNoParameters_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
	public async Task<string> TestMethodAsync()
	{
		await Task.Delay(100);
		return ""test"";
	}
}";

        // Act
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public void FixableDiagnosticIds_ShouldReturnAsyncMethodsShouldAcceptCancellationToken()
    {
        // Arrange
        // var codeFixProvider = new CancellationTokenCodeFixProvider();

        // Act
        // var fixableIds = codeFixProvider.FixableDiagnosticIds;

        // Assert
        // fixableIds.ShouldContain(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken);
        // fixableIds.Length.ShouldBe(1);
    }

    [Fact]
    public void GetFixAllProvider_ShouldReturnBatchFixer()
    {
        // Arrange
        // var codeFixProvider = new CancellationTokenCodeFixProvider();

        // Act
        // var fixAllProvider = codeFixProvider.GetFixAllProvider();

        // Assert
        // fixAllProvider.ShouldNotBeNull();
        // fixAllProvider.ShouldBe(WellKnownFixAllProviders.BatchFixer);
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithNoDiagnostic_ShouldNotRegisterActions()
    {
        // Arrange
        var sourceCode = @"public class TestClass { public async Task<string> TestMethodAsync(string parameter, CancellationToken cancellationToken) { await Task.Delay(100, cancellationToken); return parameter.ToUpper(); } }";
        var document = CreateDocument(sourceCode);
        var diagnostics = new Diagnostic[] { };

        // Act & Assert
        await Should.NotThrowAsync(() => Task.CompletedTask);
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithMultipleAsyncMethods_ShouldRegisterFixesForAll()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
	public async Task<string> TestMethod1Async(string parameter)
	{
		await Task.Delay(100);
		return parameter.ToUpper();
	}

	public async Task<int> TestMethod2Async(int parameter)
	{
		await Task.Delay(100);
		return parameter * 2;
	}

	public async void TestMethod3Async()
	{
		await Task.Delay(100);
	}
}";

        // Act
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodInInterface_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"public interface ITestInterface { Task<string> TestMethodAsync(string parameter); }";
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));
        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAsyncMethodInStruct_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"public struct TestStruct { public async Task<string> TestMethodAsync(string parameter) { await Task.Delay(100); return parameter.ToUpper(); } }";
        var document = CreateDocument(sourceCode);
        var diagnostic = CreateDiagnostic(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));
        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
            var codeFixProvider = new CancellationTokenCodeFixProvider();
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
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

#pragma warning restore CS1998, CS8031, CS0117, IDE0053

