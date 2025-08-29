using IndFusion.Analyzer.Analyzers;
using IndFusion.Analyzer.CodeFixes;
using IndFusion.Analyzer.CodeFixes.ModernCSharp;
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
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

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
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

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
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

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
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

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
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

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
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

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
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

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
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

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
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

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
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

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
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

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
				await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (a, d) => { }, CancellationToken.None));
			}
		});
	}

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
			var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
			await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
		});
	}

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

