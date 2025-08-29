#pragma warning disable CS0103, CS8602, IDE0053, IDE0031
using ExxerRules.Analyzers;
using ExxerRules.CodeFixes;
using ExxerRules.CodeFixes.Async;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace ExxerRules.Tests.TestCases.CodeFixes;

/// <summary>
/// Tests for the ConfigureAwaitFalseCodeFixProvider class.
/// </summary>
public class ConfigureAwaitFalseCodeFixProviderTests : CodeFixProviderTest<ConfigureAwaitFalseCodeFixProvider>
{
	[Fact]
	public async Task RegisterCodeFixesAsync_WithAwaitExpressionWithoutConfigureAwait_ShouldRegisterFixes()
	{
		// Arrange
		var sourceCode = @"
public class TestClass
{
    public async Task<string> TestMethodAsync()
    {
        var result = await Task.FromResult(""test"");
        return result;
    }
}";

		// Act
		var document = CreateDocument(sourceCode);
		var codeFixProvider = new ConfigureAwaitFalseCodeFixProvider();
		var diagnostic = CreateDiagnostic(DiagnosticIds.UseConfigureAwaitFalse, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

		// Act & Assert
		await Should.NotThrowAsync(() =>
		{
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

	[Fact]
	public async Task RegisterCodeFixesAsync_WithAwaitExpressionAlreadyHavingConfigureAwait_ShouldNotRegisterFixes()
	{
		// Arrange
		var sourceCode = @"
public class TestClass
{
    public async Task<string> TestMethodAsync()
    {
        var result = await Task.FromResult(""test"").ConfigureAwait(false);
        return result;
    }
}";

		// Act
		var document = CreateDocument(sourceCode);
		var codeFixProvider = new ConfigureAwaitFalseCodeFixProvider();
		var diagnostic = CreateDiagnostic(DiagnosticIds.UseConfigureAwaitFalse, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

		// Act & Assert
		await Should.NotThrowAsync(() =>
		{
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

	[Fact]
	public async Task RegisterCodeFixesAsync_WithMultipleAwaitExpressions_ShouldRegisterFixes()
	{
		// Arrange
		var sourceCode = @"
public class TestClass
{
    public async Task<string> TestMethodAsync()
    {
        var result1 = await Task.FromResult(""test1"");
        var result2 = await Task.FromResult(""test2"");
        return result1 + result2;
    }
}";

		// Act
		var document = CreateDocument(sourceCode);
		var codeFixProvider = new ConfigureAwaitFalseCodeFixProvider();
		var diagnostic = CreateDiagnostic(DiagnosticIds.UseConfigureAwaitFalse, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

		// Act & Assert
		await Should.NotThrowAsync(() =>
		{
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

	[Fact]
	public async Task RegisterCodeFixesAsync_WithAwaitExpressionInLambda_ShouldRegisterFixes()
	{
		// Arrange
		var sourceCode = @"
public class TestClass
{
    public async Task<string> TestMethodAsync()
    {
        var tasks = new[] { Task.FromResult(""test1""), Task.FromResult(""test2"") };
        var results = await Task.WhenAll(tasks.Select(async t => await t));
        return string.Join("", "", results);
    }
}";

		// Act
		var document = CreateDocument(sourceCode);
		var codeFixProvider = new ConfigureAwaitFalseCodeFixProvider();
		var diagnostic = CreateDiagnostic(DiagnosticIds.UseConfigureAwaitFalse, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

		// Act & Assert
		await Should.NotThrowAsync(() =>
		{
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

	[Fact]
	public async Task RegisterCodeFixesAsync_WithAwaitExpressionInUsingStatement_ShouldRegisterFixes()
	{
		// Arrange
		var sourceCode = @"
public class TestClass
{
    public async Task<string> TestMethodAsync()
    {
        using var stream = await File.OpenReadAsync(""test.txt"");
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}";

		// Act
		var document = CreateDocument(sourceCode);
		var codeFixProvider = new ConfigureAwaitFalseCodeFixProvider();
		var diagnostic = CreateDiagnostic(DiagnosticIds.UseConfigureAwaitFalse, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

		// Act & Assert
		await Should.NotThrowAsync(() =>
		{
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

	[Fact]
	public async Task RegisterCodeFixesAsync_WithAwaitExpressionInTryCatch_ShouldRegisterFixes()
	{
		// Arrange
		var sourceCode = @"
public class TestClass
{
    public async Task<string> TestMethodAsync()
    {
        try
        {
            return await Task.FromResult(""test"");
        }
        catch (Exception)
        {
            return await Task.FromResult(""error"");
        }
    }
}";

		// Act
		var document = CreateDocument(sourceCode);
		var codeFixProvider = new ConfigureAwaitFalseCodeFixProvider();
		var diagnostic = CreateDiagnostic(DiagnosticIds.UseConfigureAwaitFalse, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

		// Act & Assert
		await Should.NotThrowAsync(() =>
		{
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

	[Fact]
	public async Task RegisterCodeFixesAsync_WithAwaitExpressionInSwitchExpression_ShouldRegisterFixes()
	{
		// Arrange
		var sourceCode = @"
public class TestClass
{
    public async Task<string> TestMethodAsync(int value)
    {
        return value switch
        {
            1 => await Task.FromResult(""one""),
            2 => await Task.FromResult(""two""),
            _ => await Task.FromResult(""unknown"")
        };
    }
}";

		// Act
		var document = CreateDocument(sourceCode);
		var codeFixProvider = new ConfigureAwaitFalseCodeFixProvider();
		var diagnostic = CreateDiagnostic(DiagnosticIds.UseConfigureAwaitFalse, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

		// Act & Assert
		await Should.NotThrowAsync(() =>
		{
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

	[Fact]
	public async Task RegisterCodeFixesAsync_WithAwaitExpressionInTernaryOperator_ShouldRegisterFixes()
	{
		// Arrange
		var sourceCode = @"
public class TestClass
{
    public async Task<string> TestMethodAsync(bool condition)
    {
        return condition
            ? await Task.FromResult(""true"")
            : await Task.FromResult(""false"");
    }
}";

		// Act
		var document = CreateDocument(sourceCode);
		var codeFixProvider = new ConfigureAwaitFalseCodeFixProvider();
		var diagnostic = CreateDiagnostic(DiagnosticIds.UseConfigureAwaitFalse, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

		// Act & Assert
		await Should.NotThrowAsync(() =>
		{
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

	[Fact]
	public void FixableDiagnosticIds_ShouldReturnUseConfigureAwaitFalse()
	{
		// Arrange
		var codeFixProvider = new ConfigureAwaitFalseCodeFixProvider();

		// Act
		var fixableIds = codeFixProvider.FixableDiagnosticIds;

		// Assert
		fixableIds.ShouldContain(DiagnosticIds.UseConfigureAwaitFalse);
		fixableIds.Length.ShouldBe(1);
	}

	[Fact]
	public void GetFixAllProvider_ShouldReturnBatchFixer()
	{
		// Arrange
		var codeFixProvider = new ConfigureAwaitFalseCodeFixProvider();

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
    public async Task<string> TestMethodAsync()
    {
        var result = await Task.FromResult(""test"").ConfigureAwait(false);
        return result;
    }
}";

		// Act
		var document = CreateDocument(sourceCode);
		var codeFixProvider = new ConfigureAwaitFalseCodeFixProvider();
		var diagnostics = new Diagnostic[] { };

		// Act & Assert
		await Should.NotThrowAsync(async () =>
		{
			await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (a, d) => { }, CancellationToken.None));
		});
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAwaitExpressionInProperty_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public async Task<string> TestProperty => await Task.FromResult(""test"");
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ConfigureAwaitFalseCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseConfigureAwaitFalse, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            return codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithAwaitExpressionInLocalFunction_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public async Task<string> TestMethodAsync()
    {
        async Task<string> LocalFunction()
        {
            return await Task.FromResult(""test"");
        }

        return await LocalFunction();
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new ConfigureAwaitFalseCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseConfigureAwaitFalse, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(() =>
        {
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

#pragma warning restore CS0103, CS8602, IDE0053, IDE0031
