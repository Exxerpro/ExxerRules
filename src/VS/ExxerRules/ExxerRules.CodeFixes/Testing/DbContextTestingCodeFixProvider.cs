using System.Collections.Immutable;
using System.Composition;
using ExxerRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;

namespace ExxerRules.CodeFixes.Testing;

/// <summary>
/// Code fix provider that replaces mocked DbContext with InMemory provider.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DbContextTestingCodeFixProvider)), Shared]
public class DbContextTestingCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override sealed ImmutableArray<string> FixableDiagnosticIds => 
		ImmutableArray.Create(DiagnosticIds.DoNotMockDbContext);

	/// <inheritdoc/>
	public override sealed FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	/// <inheritdoc/>
	public override sealed async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
		if (root == null)
		{
			return;
		}

		foreach (var diagnostic in context.Diagnostics)
		{
			var diagnosticSpan = diagnostic.Location.SourceSpan;
			var node = root.FindNode(diagnosticSpan);

			if (node == null)
			{
				continue;
			}

			// Register fixes based on the node type
			RegisterDbContextTestingFixes(context, diagnostic, node);
		}
	}

	/// <summary>
	/// Registers code fix options based on the type of DbContext mocking that can be converted.
	/// </summary>
	/// <param name="context">The code fix context.</param>
	/// <param name="diagnostic">The diagnostic to fix.</param>
	/// <param name="node">The syntax node that can be converted.</param>
	private static void RegisterDbContextTestingFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
	{
		switch (node)
		{
			case ObjectCreationExpressionSyntax objectCreation:
				RegisterObjectCreationFixes(context, diagnostic, objectCreation);
				break;
			case InvocationExpressionSyntax invocationExpression:
				RegisterInvocationFixes(context, diagnostic, invocationExpression);
				break;
			case UsingDirectiveSyntax usingDirective:
				RegisterUsingFixes(context, diagnostic, usingDirective);
				break;
			case VariableDeclaratorSyntax variableDeclarator:
				RegisterVariableFixes(context, diagnostic, variableDeclarator);
				break;
		}
	}

	/// <summary>
	/// Registers code fix options for object creation conversion.
	/// </summary>
	private static void RegisterObjectCreationFixes(CodeFixContext context, Diagnostic diagnostic, ObjectCreationExpressionSyntax objectCreation)
	{
		var typeName = objectCreation.Type.ToString();
		if (typeName.Contains("Mock<") && typeName.Contains("DbContext"))
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "🔄 Replace mocked DbContext with InMemory provider",
					createChangedDocument: c => ReplaceMockedDbContextWithInMemoryAsync(context.Document, objectCreation, c),
					equivalenceKey: "ReplaceMockedDbContextWithInMemory"),
				diagnostic);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: "🔄 Replace mocked DbContext with SQLite in-memory",
					createChangedDocument: c => ReplaceMockedDbContextWithSqliteAsync(context.Document, objectCreation, c),
					equivalenceKey: "ReplaceMockedDbContextWithSqlite"),
				diagnostic);
		}
	}

	/// <summary>
	/// Registers code fix options for invocation conversion.
	/// </summary>
	private static void RegisterInvocationFixes(CodeFixContext context, Diagnostic diagnostic, InvocationExpressionSyntax invocationExpression)
	{
		if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess)
		{
			var methodName = memberAccess.Name.Identifier.ValueText;

			if (methodName == "Setup" && IsDbContextMockSetup(invocationExpression))
			{
				context.RegisterCodeFix(
					CodeAction.Create(
						title: "🔄 Replace DbContext mock setup with InMemory data",
						createChangedDocument: c => ReplaceDbContextMockSetupAsync(context.Document, invocationExpression, c),
						equivalenceKey: "ReplaceDbContextMockSetup"),
					diagnostic);
			}
		}
	}

	/// <summary>
	/// Registers code fix options for using directive conversion.
	/// </summary>
	private static void RegisterUsingFixes(CodeFixContext context, Diagnostic diagnostic, UsingDirectiveSyntax usingDirective)
	{
		var usingName = usingDirective.Name?.ToString();
		if (usingName == "Moq" || usingName == "NSubstitute")
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "🔄 Add Entity Framework InMemory using",
					createChangedDocument: c => AddInMemoryUsingAsync(context.Document, usingDirective, c),
					equivalenceKey: "AddInMemoryUsing"),
				diagnostic);
		}
	}

	/// <summary>
	/// Registers code fix options for variable conversion.
	/// </summary>
	private static void RegisterVariableFixes(CodeFixContext context, Diagnostic diagnostic, VariableDeclaratorSyntax variableDeclarator)
	{
		var variableName = variableDeclarator.Identifier.ValueText;
		if (variableName.ToLower().Contains("mock") && variableName.ToLower().Contains("context"))
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: $"🔄 Convert {variableName} to InMemory DbContext",
					createChangedDocument: c => ConvertMockVariableToInMemoryAsync(context.Document, variableDeclarator, c),
					equivalenceKey: $"Convert{variableName}ToInMemory"),
				diagnostic);
		}
	}

	/// <summary>
	/// Replaces mocked DbContext with InMemory provider.
	/// </summary>
	private static async Task<Document> ReplaceMockedDbContextWithInMemoryAsync(Document document, ObjectCreationExpressionSyntax objectCreation, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var rewriter = new MockedDbContextToInMemoryRewriter();
		var newExpression = (ExpressionSyntax)rewriter.Visit(objectCreation);
		editor.ReplaceNode(objectCreation, newExpression);

		// Add InMemory using directive if not present
		await AddInMemoryUsingIfNeededAsync(editor, cancellationToken).ConfigureAwait(false);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Replaces mocked DbContext with SQLite in-memory.
	/// </summary>
	private static async Task<Document> ReplaceMockedDbContextWithSqliteAsync(Document document, ObjectCreationExpressionSyntax objectCreation, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var rewriter = new MockedDbContextToSqliteRewriter();
		var newExpression = (ExpressionSyntax)rewriter.Visit(objectCreation);
		editor.ReplaceNode(objectCreation, newExpression);

		// Add SQLite using directive if not present
		await AddSqliteUsingIfNeededAsync(editor, cancellationToken).ConfigureAwait(false);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Replaces DbContext mock setup with InMemory data.
	/// </summary>
	private static async Task<Document> ReplaceDbContextMockSetupAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var rewriter = new DbContextMockSetupToInMemoryRewriter();
		var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
		editor.ReplaceNode(invocationExpression, newExpression);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Adds Entity Framework InMemory using directive.
	/// </summary>
	private static async Task<Document> AddInMemoryUsingAsync(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.EntityFrameworkCore.InMemory"));
		editor.InsertAfter(usingDirective, newUsing);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts mock variable to InMemory DbContext.
	/// </summary>
	private static async Task<Document> ConvertMockVariableToInMemoryAsync(Document document, VariableDeclaratorSyntax variableDeclarator, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var rewriter = new MockVariableToInMemoryRewriter();
		var newVariable = (VariableDeclaratorSyntax)rewriter.Visit(variableDeclarator);
		editor.ReplaceNode(variableDeclarator, newVariable);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Checks if an invocation is a DbContext mock setup.
	/// </summary>
	private static bool IsDbContextMockSetup(InvocationExpressionSyntax invocationExpression)
	{
		if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess &&
			memberAccess.Name.Identifier.ValueText == "Setup")
		{
			var mockExpression = memberAccess.Expression.ToString();
			return mockExpression.ToLower().Contains("mock") && mockExpression.ToLower().Contains("context");
		}
		return false;
	}

	/// <summary>
	/// Adds InMemory using directive if not present.
	/// </summary>
	private static async Task AddInMemoryUsingIfNeededAsync(DocumentEditor editor, CancellationToken cancellationToken)
	{
		var document = editor.GetChangedDocument();
		var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
		
		if (root != null && !root.ToString().Contains("Microsoft.EntityFrameworkCore.InMemory"))
		{
			var compilationUnit = root as CompilationUnitSyntax;
			if (compilationUnit != null)
			{
				var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.EntityFrameworkCore.InMemory"));
				var newCompilationUnit = compilationUnit.AddUsings(newUsing);
				editor.ReplaceNode(compilationUnit, newCompilationUnit);
			}
		}
	}

	/// <summary>
	/// Adds SQLite using directive if not present.
	/// </summary>
	private static async Task AddSqliteUsingIfNeededAsync(DocumentEditor editor, CancellationToken cancellationToken)
	{
		var document = editor.GetChangedDocument();
		var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
		
		if (root != null && !root.ToString().Contains("Microsoft.EntityFrameworkCore.Sqlite"))
		{
			var compilationUnit = root as CompilationUnitSyntax;
			if (compilationUnit != null)
			{
				var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.EntityFrameworkCore.Sqlite"));
				var newCompilationUnit = compilationUnit.AddUsings(newUsing);
				editor.ReplaceNode(compilationUnit, newCompilationUnit);
			}
		}
	}

	/// <summary>
	/// Rewriter that converts mocked DbContext to InMemory provider.
	/// </summary>
	private class MockedDbContextToInMemoryRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
		{
			if (node.Type.ToString().Contains("Mock<") && node.Type.ToString().Contains("DbContext"))
			{
				// Convert Mock<DbContext> to InMemory DbContext
				var dbContextType = ExtractDbContextType(node.Type);
				if (dbContextType != null)
				{
					return SyntaxFactory.InvocationExpression(
						SyntaxFactory.MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							SyntaxFactory.MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								SyntaxFactory.IdentifierName("new"),
								SyntaxFactory.GenericName(
									SyntaxFactory.Identifier("DbContextOptionsBuilder"),
									SyntaxFactory.TypeArgumentList(
										SyntaxFactory.SingletonSeparatedList(dbContextType)))),
							SyntaxFactory.IdentifierName("UseInMemoryDatabase")),
						SyntaxFactory.ArgumentList(
							SyntaxFactory.SingletonSeparatedList(
								SyntaxFactory.Argument(
									SyntaxFactory.LiteralExpression(
										SyntaxKind.StringLiteralExpression,
										SyntaxFactory.Literal("TestDatabase"))))));
				}
			}

			return base.VisitObjectCreationExpression(node);
		}

		/// <summary>
		/// Extracts the DbContext type from Mock<T>.
		/// </summary>
		private static TypeSyntax? ExtractDbContextType(TypeSyntax type)
		{
			if (type is GenericNameSyntax genericName &&
				genericName.TypeArgumentList.Arguments.Count > 0)
			{
				return genericName.TypeArgumentList.Arguments[0];
			}
			return null;
		}
	}

	/// <summary>
	/// Rewriter that converts mocked DbContext to SQLite in-memory.
	/// </summary>
	private class MockedDbContextToSqliteRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
		{
			if (node.Type.ToString().Contains("Mock<") && node.Type.ToString().Contains("DbContext"))
			{
				// Convert Mock<DbContext> to SQLite in-memory DbContext
				var dbContextType = ExtractDbContextType(node.Type);
				if (dbContextType != null)
				{
					return SyntaxFactory.InvocationExpression(
						SyntaxFactory.MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							SyntaxFactory.MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								SyntaxFactory.IdentifierName("new"),
								SyntaxFactory.GenericName(
									SyntaxFactory.Identifier("DbContextOptionsBuilder"),
									SyntaxFactory.TypeArgumentList(
										SyntaxFactory.SingletonSeparatedList(dbContextType)))),
							SyntaxFactory.IdentifierName("UseSqlite")),
						SyntaxFactory.ArgumentList(
							SyntaxFactory.SingletonSeparatedList(
								SyntaxFactory.Argument(
									SyntaxFactory.LiteralExpression(
										SyntaxKind.StringLiteralExpression,
										SyntaxFactory.Literal("DataSource=:memory:"))))));
				}
			}

			return base.VisitObjectCreationExpression(node);
		}

		/// <summary>
		/// Extracts the DbContext type from Mock<T>.
		/// </summary>
		private static TypeSyntax? ExtractDbContextType(TypeSyntax type)
		{
			if (type is GenericNameSyntax genericName &&
				genericName.TypeArgumentList.Arguments.Count > 0)
			{
				return genericName.TypeArgumentList.Arguments[0];
			}
			return null;
		}
	}

	/// <summary>
	/// Rewriter that converts DbContext mock setup to InMemory data.
	/// </summary>
	private class DbContextMockSetupToInMemoryRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
				memberAccess.Name.Identifier.ValueText == "Setup" &&
				IsDbContextMockSetup(node))
			{
				// Convert mock setup to InMemory data seeding
				var setupArgument = node.ArgumentList?.Arguments.FirstOrDefault()?.Expression;
				if (setupArgument is LambdaExpressionSyntax lambda)
				{
					var methodCall = lambda.Body as InvocationExpressionSyntax;
					if (methodCall != null)
					{
						// Replace with InMemory data seeding
						return SyntaxFactory.InvocationExpression(
							SyntaxFactory.MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								SyntaxFactory.IdentifierName("context"),
								SyntaxFactory.IdentifierName("Add")),
							SyntaxFactory.ArgumentList(
								SyntaxFactory.SingletonSeparatedList(
									SyntaxFactory.Argument(
										SyntaxFactory.ObjectCreationExpression(
											SyntaxFactory.IdentifierName("TestEntity"))))));
					}
				}
			}

			return base.VisitInvocationExpression(node);
		}

		/// <summary>
		/// Checks if an invocation is a DbContext mock setup.
		/// </summary>
		private static bool IsDbContextMockSetup(InvocationExpressionSyntax invocationExpression)
		{
			if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess &&
				memberAccess.Name.Identifier.ValueText == "Setup")
			{
				var mockExpression = memberAccess.Expression.ToString();
				return mockExpression.ToLower().Contains("mock") && mockExpression.ToLower().Contains("context");
			}
			return false;
		}
	}

	/// <summary>
	/// Rewriter that converts mock variable to InMemory DbContext.
	/// </summary>
	private class MockVariableToInMemoryRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitVariableDeclarator(VariableDeclaratorSyntax node)
		{
			var variableName = node.Identifier.ValueText;
			if (variableName.ToLower().Contains("mock") && variableName.ToLower().Contains("context"))
			{
				// Convert mock variable name to context
				var newName = variableName.Replace("Mock", "").Replace("mock", "");
				if (string.IsNullOrEmpty(newName))
				{
					newName = "context";
				}

				return node.WithIdentifier(SyntaxFactory.Identifier(newName));
			}

			return base.VisitVariableDeclarator(node);
		}
	}
}