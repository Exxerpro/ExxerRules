using System.Collections.Immutable;
using System.Composition;
using ExxerRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.Linq;

namespace ExxerRules.CodeFixes.ErrorHandling;

/// <summary>
/// Code fix provider that converts exception-throwing code to use the Result pattern.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseResultPatternCodeFixProvider)), Shared]
public class UseResultPatternCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override sealed ImmutableArray<string> FixableDiagnosticIds =>
		ImmutableArray.Create(DiagnosticIds.UseResultPattern);

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

			RegisterResultPatternFixes(context, diagnostic, node);
		}
	}

	/// <summary>
	/// Registers code fix options based on the type of exception-throwing code that can be converted.
	/// </summary>
	private static void RegisterResultPatternFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
	{
		switch (node)
		{
			case ThrowStatementSyntax throwStatement:
				RegisterThrowStatementFixes(context, diagnostic, throwStatement);
				break;

			case InvocationExpressionSyntax invocationExpression:
				RegisterInvocationFixes(context, diagnostic, invocationExpression);
				break;
		}
	}

	/// <summary>
	/// Registers code fix options for throw statement conversion.
	/// </summary>
	private static void RegisterThrowStatementFixes(CodeFixContext context, Diagnostic diagnostic, ThrowStatementSyntax throwStatement)
	{
		context.RegisterCodeFix(
			CodeAction.Create(
				title: "🔄 Convert throw to Result.Failure",
				createChangedDocument: c => ConvertThrowToResultFailureAsync(context.Document, throwStatement, c),
				equivalenceKey: "ConvertThrowToResultFailure"),
			diagnostic);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: "🔄 Convert throw to Result.WithFailure",
				createChangedDocument: c => ConvertThrowToResultWithFailureAsync(context.Document, throwStatement, c),
				equivalenceKey: "ConvertThrowToResultWithFailure"),
			diagnostic);
	}

	/// <summary>
	/// Registers code fix options for invocation conversion.
	/// </summary>
	private static void RegisterInvocationFixes(CodeFixContext context, Diagnostic diagnostic, InvocationExpressionSyntax invocationExpression)
	{
		if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess)
		{
			var methodName = memberAccess.Name.Identifier.ValueText;

			switch (methodName)
			{
				case "Throw":
					context.RegisterCodeFix(
						CodeAction.Create(
							title: "🔄 Convert Throw to Result.Failure",
							createChangedDocument: c => ConvertInvocationToResultFailureAsync(context.Document, invocationExpression, c),
							equivalenceKey: "ConvertThrowToResultFailure"),
						diagnostic);
					break;
			}
		}
	}

	/// <summary>
	/// Converts a throw statement to Result.Failure.
	/// </summary>
	private static async Task<Document> ConvertThrowToResultFailureAsync(Document document, ThrowStatementSyntax throwStatement, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
		var rewriter = new ThrowToResultRewriter("Failure");
		var newStatement = (StatementSyntax)rewriter.Visit(throwStatement);
		editor.ReplaceNode(throwStatement, newStatement);
		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts a throw statement to Result.WithFailure.
	/// </summary>
	private static async Task<Document> ConvertThrowToResultWithFailureAsync(Document document, ThrowStatementSyntax throwStatement, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
		var rewriter = new ThrowToResultRewriter("WithFailure");
		var newStatement = (StatementSyntax)rewriter.Visit(throwStatement);
		editor.ReplaceNode(throwStatement, newStatement);
		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts a throw invocation to Result.Failure.
	/// </summary>
	private static async Task<Document> ConvertInvocationToResultFailureAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
		var rewriter = new ThrowToResultRewriter("Failure");
		var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
		editor.ReplaceNode(invocationExpression, newExpression);
		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Rewriter that converts throw statements to Result pattern.
	/// </summary>
	private class ThrowToResultRewriter : CSharpSyntaxRewriter
	{
		private readonly string resultMethod;

		public ThrowToResultRewriter(string resultMethod)
		{
			this.resultMethod = resultMethod;
		}

		public override SyntaxNode? VisitThrowStatement(ThrowStatementSyntax node)
		{
			if (node.Expression != null)
			{
				var errorMessage = ExtractErrorMessage(node.Expression);
				var resultCall = SyntaxFactory.InvocationExpression(
					SyntaxFactory.MemberAccessExpression(
						SyntaxKind.SimpleMemberAccessExpression,
						SyntaxFactory.IdentifierName("Result"),
						SyntaxFactory.IdentifierName(resultMethod)),
					SyntaxFactory.ArgumentList(
						SyntaxFactory.SingletonSeparatedList(
							SyntaxFactory.Argument(
								SyntaxFactory.LiteralExpression(
									SyntaxKind.StringLiteralExpression,
									SyntaxFactory.Literal(errorMessage))))));

				return SyntaxFactory.ReturnStatement(resultCall);
			}

			return base.VisitThrowStatement(node);
		}

		public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
				memberAccess.Name.Identifier.ValueText == "Throw")
			{
				var arguments = node.ArgumentList?.Arguments;
				var args = arguments;
				if (args != null && args.Value.Count > 0)
				{
					var errorMessage = ExtractErrorMessage(args.Value[0].Expression);
					return SyntaxFactory.InvocationExpression(
						SyntaxFactory.MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							SyntaxFactory.IdentifierName("Result"),
							SyntaxFactory.IdentifierName(resultMethod)),
						SyntaxFactory.ArgumentList(
							SyntaxFactory.SingletonSeparatedList(
								SyntaxFactory.Argument(
									SyntaxFactory.LiteralExpression(
										SyntaxKind.StringLiteralExpression,
										SyntaxFactory.Literal(errorMessage))))));
				}
			}

			return base.VisitInvocationExpression(node);
		}

		/// <summary>
		/// Extracts an error message from an expression.
		/// </summary>
		private static string ExtractErrorMessage(ExpressionSyntax expression)
		{
			if (expression is LiteralExpressionSyntax literal && literal.Kind() == SyntaxKind.StringLiteralExpression)
			{
				return literal.Token.ValueText;
			}
			else if (expression is ObjectCreationExpressionSyntax objectCreation)
			{
				var typeName = objectCreation.Type.ToString();
				return $"An error occurred: {typeName}";
			}

			return "An error occurred";
		}
	}
}
