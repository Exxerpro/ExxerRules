using System.Collections.Immutable;
using System.Composition;
using IndFusion.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;

namespace IndFusion.CodeFixes.Async;

/// <summary>
/// Code fix provider that adds ConfigureAwait(false) to await expressions in library code.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConfigureAwaitFalseCodeFixProvider)), Shared]
public class ConfigureAwaitFalseCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override sealed ImmutableArray<string> FixableDiagnosticIds => 
		ImmutableArray.Create(DiagnosticIds.UseConfigureAwaitFalse);

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

			// Find await expressions that need ConfigureAwait(false)
			if (node is AwaitExpressionSyntax awaitExpression)
			{
				RegisterConfigureAwaitFixes(context, diagnostic, awaitExpression);
			}
		}
	}

	/// <summary>
	/// Registers code fix options for adding ConfigureAwait(false).
	/// </summary>
	/// <param name="context">The code fix context.</param>
	/// <param name="diagnostic">The diagnostic to fix.</param>
	/// <param name="awaitExpression">The await expression.</param>
	private static void RegisterConfigureAwaitFixes(CodeFixContext context, Diagnostic diagnostic, AwaitExpressionSyntax awaitExpression)
	{
		// Add ConfigureAwait(false) to the await expression
		context.RegisterCodeFix(
			CodeAction.Create(
				title: $"⚡ Add ConfigureAwait(false) to await expression",
				createChangedDocument: c => AddConfigureAwaitFalseAsync(context.Document, awaitExpression, c),
				equivalenceKey: "AddConfigureAwaitFalse"),
			diagnostic);

		// Add ConfigureAwait(false) and fix all similar expressions in the method
		context.RegisterCodeFix(
			CodeAction.Create(
				title: $"🔄 Add ConfigureAwait(false) to all await expressions in method",
				createChangedDocument: c => AddConfigureAwaitFalseToAllInMethodAsync(context.Document, awaitExpression, c),
				equivalenceKey: "AddConfigureAwaitFalseToAllInMethod"),
			diagnostic);

		// Add ConfigureAwait(false) with true parameter (for UI contexts)
		context.RegisterCodeFix(
			CodeAction.Create(
				title: $"🎯 Add ConfigureAwait(true) for UI context",
				createChangedDocument: c => AddConfigureAwaitTrueAsync(context.Document, awaitExpression, c),
				equivalenceKey: "AddConfigureAwaitTrue"),
			diagnostic);
	}

	/// <summary>
	/// Adds ConfigureAwait(false) to a specific await expression.
	/// </summary>
	private static async Task<Document> AddConfigureAwaitFalseAsync(Document document, AwaitExpressionSyntax awaitExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var newAwaitExpression = CreateConfigureAwaitExpression(awaitExpression, false);
		editor.ReplaceNode(awaitExpression, newAwaitExpression);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Adds ConfigureAwait(false) to all await expressions in the containing method.
	/// </summary>
	private static async Task<Document> AddConfigureAwaitFalseToAllInMethodAsync(Document document, AwaitExpressionSyntax awaitExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// Find the containing method
		var methodDeclaration = awaitExpression.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
		if (methodDeclaration != null)
		{
			// Find all await expressions in the method that don't already have ConfigureAwait
			var rewriter = new ConfigureAwaitRewriter(false);
			var newMethod = (MethodDeclarationSyntax)rewriter.Visit(methodDeclaration);
			editor.ReplaceNode(methodDeclaration, newMethod);
		}

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Adds ConfigureAwait(true) to a specific await expression.
	/// </summary>
	private static async Task<Document> AddConfigureAwaitTrueAsync(Document document, AwaitExpressionSyntax awaitExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var newAwaitExpression = CreateConfigureAwaitExpression(awaitExpression, true);
		editor.ReplaceNode(awaitExpression, newAwaitExpression);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Creates a new await expression with ConfigureAwait.
	/// </summary>
	private static AwaitExpressionSyntax CreateConfigureAwaitExpression(AwaitExpressionSyntax awaitExpression, bool continueOnCapturedContext)
	{
		var expression = awaitExpression.Expression;

		// Check if the expression already has ConfigureAwait
		if (HasConfigureAwait(expression))
		{
			return awaitExpression;
		}

		// Create ConfigureAwait call
		var configureAwaitCall = SyntaxFactory.InvocationExpression(
			SyntaxFactory.MemberAccessExpression(
				SyntaxKind.SimpleMemberAccessExpression,
				SyntaxFactory.ParenthesizedExpression(expression),
				SyntaxFactory.IdentifierName("ConfigureAwait")),
			SyntaxFactory.ArgumentList(
				SyntaxFactory.SingletonSeparatedList(
					SyntaxFactory.Argument(
						SyntaxFactory.LiteralExpression(
							continueOnCapturedContext ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)))));

		return awaitExpression.WithExpression(configureAwaitCall);
	}

	/// <summary>
	/// Checks if an expression already has ConfigureAwait.
	/// </summary>
	private static bool HasConfigureAwait(ExpressionSyntax expression)
	{
		return expression.ToString().Contains("ConfigureAwait");
	}

	/// <summary>
	/// Rewriter that adds ConfigureAwait(false) to all await expressions in a method.
	/// </summary>
	private class ConfigureAwaitRewriter : CSharpSyntaxRewriter
	{
		private readonly bool _continueOnCapturedContext;

		public ConfigureAwaitRewriter(bool continueOnCapturedContext)
		{
			_continueOnCapturedContext = continueOnCapturedContext;
		}

		public override SyntaxNode? VisitAwaitExpression(AwaitExpressionSyntax node)
		{
			// Skip if already has ConfigureAwait
			if (HasConfigureAwait(node.Expression))
			{
				return base.VisitAwaitExpression(node);
			}

			// Add ConfigureAwait
			var newAwaitExpression = CreateConfigureAwaitExpression(node, _continueOnCapturedContext);
			return base.VisitAwaitExpression(newAwaitExpression);
		}
	}
}
