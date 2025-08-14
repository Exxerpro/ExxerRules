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

namespace ExxerRules.CodeFixes.Logging;

/// <summary>
/// Code fix provider that converts traditional logging to structured logging.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StructuredLoggingCodeFixProvider)), Shared]
public class StructuredLoggingCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override sealed ImmutableArray<string> FixableDiagnosticIds => 
		ImmutableArray.Create(DiagnosticIds.UseStructuredLogging);

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
			RegisterStructuredLoggingFixes(context, diagnostic, node);
		}
	}

	/// <summary>
	/// Registers code fix options based on the type of logging that can be converted.
	/// </summary>
	/// <param name="context">The code fix context.</param>
	/// <param name="diagnostic">The diagnostic to fix.</param>
	/// <param name="node">The syntax node that can be converted.</param>
	private static void RegisterStructuredLoggingFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
	{
		switch (node)
		{
			case InvocationExpressionSyntax invocationExpression:
				RegisterInvocationFixes(context, diagnostic, invocationExpression);
				break;
			case InterpolatedStringExpressionSyntax interpolatedString:
				RegisterInterpolatedStringFixes(context, diagnostic, interpolatedString);
				break;
			case BinaryExpressionSyntax binaryExpression:
				RegisterBinaryExpressionFixes(context, diagnostic, binaryExpression);
				break;
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

			switch (methodName)
			{
				case "LogInformation":
				case "LogWarning":
				case "LogError":
				case "LogDebug":
				case "LogTrace":
				case "LogCritical":
					context.RegisterCodeFix(
						CodeAction.Create(
							title: $"🔄 Convert {methodName} to structured logging",
							createChangedDocument: c => ConvertToStructuredLoggingAsync(context.Document, invocationExpression, c),
							equivalenceKey: $"Convert{methodName}ToStructuredLogging"),
						diagnostic);
					break;
			}
		}
	}

	/// <summary>
	/// Registers code fix options for interpolated string conversion.
	/// </summary>
	private static void RegisterInterpolatedStringFixes(CodeFixContext context, Diagnostic diagnostic, InterpolatedStringExpressionSyntax interpolatedString)
	{
		context.RegisterCodeFix(
			CodeAction.Create(
				title: "🔄 Convert interpolated string to structured logging",
				createChangedDocument: c => ConvertInterpolatedStringToStructuredLoggingAsync(context.Document, interpolatedString, c),
				equivalenceKey: "ConvertInterpolatedStringToStructuredLogging"),
			diagnostic);
	}

	/// <summary>
	/// Registers code fix options for binary expression conversion.
	/// </summary>
	private static void RegisterBinaryExpressionFixes(CodeFixContext context, Diagnostic diagnostic, BinaryExpressionSyntax binaryExpression)
	{
		context.RegisterCodeFix(
			CodeAction.Create(
				title: "🔄 Convert string concatenation to structured logging",
				createChangedDocument: c => ConvertBinaryExpressionToStructuredLoggingAsync(context.Document, binaryExpression, c),
				equivalenceKey: "ConvertBinaryExpressionToStructuredLogging"),
			diagnostic);
	}

	/// <summary>
	/// Converts traditional logging to structured logging.
	/// </summary>
	private static async Task<Document> ConvertToStructuredLoggingAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var rewriter = new TraditionalToStructuredLoggingRewriter();
		var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
		editor.ReplaceNode(invocationExpression, newExpression);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts interpolated string to structured logging.
	/// </summary>
	private static async Task<Document> ConvertInterpolatedStringToStructuredLoggingAsync(Document document, InterpolatedStringExpressionSyntax interpolatedString, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var structuredLogging = ConvertInterpolatedStringToStructuredLogging(interpolatedString);
		editor.ReplaceNode(interpolatedString, structuredLogging);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts binary expression to structured logging.
	/// </summary>
	private static async Task<Document> ConvertBinaryExpressionToStructuredLoggingAsync(Document document, BinaryExpressionSyntax binaryExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var structuredLogging = ConvertBinaryExpressionToStructuredLogging(binaryExpression);
		editor.ReplaceNode(binaryExpression, structuredLogging);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts an interpolated string to structured logging format.
	/// </summary>
	private static ExpressionSyntax ConvertInterpolatedStringToStructuredLogging(InterpolatedStringExpressionSyntax interpolatedString)
	{
		var parts = new List<InterpolatedStringContentSyntax>();
		var parameters = new List<ArgumentSyntax>();

		foreach (var content in interpolatedString.Contents)
		{
			if (content is InterpolatedStringTextSyntax text)
			{
				parts.Add(text);
			}
			else if (content is InterpolatedStringInsertSyntax insert)
			{
				// Convert interpolation to structured logging parameter
				var parameterName = GenerateParameterName(insert.Expression);
				parts.Add(SyntaxFactory.InterpolatedStringText($"{{{parameterName}}}"));
				parameters.Add(SyntaxFactory.Argument(insert.Expression));
			}
		}

		// Create structured logging call
		var message = string.Join("", parts.Select(p => p.ToString()));
		var messageArgument = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(message)));

		var argumentList = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { messageArgument }.Concat(parameters)));

		return SyntaxFactory.InvocationExpression(
			SyntaxFactory.MemberAccessExpression(
				SyntaxKind.SimpleMemberAccessExpression,
				SyntaxFactory.IdentifierName("_logger"),
				SyntaxFactory.IdentifierName("LogInformation")),
			argumentList);
	}

	/// <summary>
	/// Converts a binary expression to structured logging format.
	/// </summary>
	private static ExpressionSyntax ConvertBinaryExpressionToStructuredLogging(BinaryExpressionSyntax binaryExpression)
	{
		// Convert string concatenation to structured logging
		var left = binaryExpression.Left.ToString();
		var right = binaryExpression.Right.ToString();

		var message = $"{left} {{Value}}";
		var valueArgument = SyntaxFactory.Argument(binaryExpression.Right);

		return SyntaxFactory.InvocationExpression(
			SyntaxFactory.MemberAccessExpression(
				SyntaxKind.SimpleMemberAccessExpression,
				SyntaxFactory.IdentifierName("_logger"),
				SyntaxFactory.IdentifierName("LogInformation")),
			SyntaxFactory.ArgumentList(
				SyntaxFactory.SeparatedList(new[]
				{
					SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(message))),
					valueArgument
				})));
	}

	/// <summary>
	/// Generates a parameter name for structured logging.
	/// </summary>
	private static string GenerateParameterName(ExpressionSyntax expression)
	{
		if (expression is IdentifierNameSyntax identifier)
		{
			return identifier.Identifier.ValueText;
		}
		else if (expression is MemberAccessExpressionSyntax memberAccess)
		{
			return memberAccess.Name.Identifier.ValueText;
		}
		else if (expression is InvocationExpressionSyntax invocation)
		{
			if (invocation.Expression is MemberAccessExpressionSyntax invocationMember)
			{
				return invocationMember.Name.Identifier.ValueText;
			}
		}

		return "Value";
	}

	/// <summary>
	/// Rewriter that converts traditional logging to structured logging.
	/// </summary>
	private class TraditionalToStructuredLoggingRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			if (node.Expression is MemberAccessExpressionSyntax memberAccess)
			{
				var methodName = memberAccess.Name.Identifier.ValueText;
				var arguments = node.ArgumentList?.Arguments;

				if (arguments != null && arguments.Count > 0)
				{
					var firstArgument = arguments[0].Expression;

					// Convert string concatenation to structured logging
					if (firstArgument is BinaryExpressionSyntax binaryExpression &&
						binaryExpression.OperatorToken.IsKind(SyntaxKind.PlusToken))
					{
						var structuredLogging = ConvertBinaryExpressionToStructuredLogging(binaryExpression);
						return SyntaxFactory.InvocationExpression(
							SyntaxFactory.MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								SyntaxFactory.IdentifierName("_logger"),
								SyntaxFactory.IdentifierName(methodName)),
							SyntaxFactory.ArgumentList(
								SyntaxFactory.SeparatedList(new[]
								{
									SyntaxFactory.Argument(structuredLogging)
								})));
					}

					// Convert interpolated string to structured logging
					if (firstArgument is InterpolatedStringExpressionSyntax interpolatedString)
					{
						var structuredLogging = ConvertInterpolatedStringToStructuredLogging(interpolatedString);
						return SyntaxFactory.InvocationExpression(
							SyntaxFactory.MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								SyntaxFactory.IdentifierName("_logger"),
								SyntaxFactory.IdentifierName(methodName)),
							SyntaxFactory.ArgumentList(
								SyntaxFactory.SeparatedList(new[]
								{
									SyntaxFactory.Argument(structuredLogging)
								})));
					}
				}
			}

			return base.VisitInvocationExpression(node);
		}
	}
}