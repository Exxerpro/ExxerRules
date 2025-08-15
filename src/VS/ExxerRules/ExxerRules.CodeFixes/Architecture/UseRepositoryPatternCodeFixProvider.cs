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

namespace ExxerRules.CodeFixes.Architecture;

/// <summary>
/// Code fix provider that promotes repository pattern usage.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseRepositoryPatternCodeFixProvider)), Shared]
public class UseRepositoryPatternCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override sealed ImmutableArray<string> FixableDiagnosticIds => 
		ImmutableArray.Create(DiagnosticIds.UseRepositoryPattern);

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
			RegisterRepositoryPatternFixes(context, diagnostic, node);
		}
	}

	/// <summary>
	/// Registers code fix options based on the type of direct data access that can be converted.
	/// </summary>
	/// <param name="context">The code fix context.</param>
	/// <param name="diagnostic">The diagnostic to fix.</param>
	/// <param name="node">The syntax node that can be converted.</param>
	private static void RegisterRepositoryPatternFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
	{
		switch (node)
		{
			case InvocationExpressionSyntax invocationExpression:
				RegisterInvocationFixes(context, diagnostic, invocationExpression);
				break;
			case ObjectCreationExpressionSyntax objectCreation:
				RegisterObjectCreationFixes(context, diagnostic, objectCreation);
				break;
			case UsingDirectiveSyntax usingDirective:
				RegisterUsingFixes(context, diagnostic, usingDirective);
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
				case "Find":
				case "FindAsync":
					context.RegisterCodeFix(
						CodeAction.Create(
							title: $"🔄 Convert {methodName} to repository pattern",
							createChangedDocument: c => ConvertFindToRepositoryAsync(context.Document, invocationExpression, c),
							equivalenceKey: $"Convert{methodName}ToRepository"),
						diagnostic);
					break;

				case "Add":
				case "AddAsync":
					context.RegisterCodeFix(
						CodeAction.Create(
							title: $"🔄 Convert {methodName} to repository pattern",
							createChangedDocument: c => ConvertAddToRepositoryAsync(context.Document, invocationExpression, c),
							equivalenceKey: $"Convert{methodName}ToRepository"),
						diagnostic);
					break;

				case "Update":
				case "UpdateAsync":
					context.RegisterCodeFix(
						CodeAction.Create(
							title: $"🔄 Convert {methodName} to repository pattern",
							createChangedDocument: c => ConvertUpdateToRepositoryAsync(context.Document, invocationExpression, c),
							equivalenceKey: $"Convert{methodName}ToRepository"),
						diagnostic);
					break;

				case "Remove":
				case "RemoveAsync":
					context.RegisterCodeFix(
						CodeAction.Create(
							title: $"🔄 Convert {methodName} to repository pattern",
							createChangedDocument: c => ConvertRemoveToRepositoryAsync(context.Document, invocationExpression, c),
							equivalenceKey: $"Convert{methodName}ToRepository"),
						diagnostic);
					break;

				case "ToList":
				case "ToListAsync":
					context.RegisterCodeFix(
						CodeAction.Create(
							title: $"🔄 Convert {methodName} to repository pattern",
							createChangedDocument: c => ConvertToListToRepositoryAsync(context.Document, invocationExpression, c),
							equivalenceKey: $"Convert{methodName}ToRepository"),
						diagnostic);
					break;
			}
		}
	}

	/// <summary>
	/// Registers code fix options for object creation conversion.
	/// </summary>
	private static void RegisterObjectCreationFixes(CodeFixContext context, Diagnostic diagnostic, ObjectCreationExpressionSyntax objectCreation)
	{
		var typeName = objectCreation.Type.ToString();
		if (typeName.Contains("DbContext") || typeName.Contains("SqlConnection"))
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "🔄 Replace direct data access with repository",
					createChangedDocument: c => ReplaceDirectDataAccessWithRepositoryAsync(context.Document, objectCreation, c),
					equivalenceKey: "ReplaceDirectDataAccessWithRepository"),
				diagnostic);
		}
	}

	/// <summary>
	/// Registers code fix options for using directive conversion.
	/// </summary>
	private static void RegisterUsingFixes(CodeFixContext context, Diagnostic diagnostic, UsingDirectiveSyntax usingDirective)
	{
		var usingName = usingDirective.Name?.ToString();
		if (usingName is "Microsoft.EntityFrameworkCore" or "System.Data.SqlClient")
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "🔄 Add repository interface using",
					createChangedDocument: c => AddRepositoryUsingAsync(context.Document, usingDirective, c),
					equivalenceKey: "AddRepositoryUsing"),
				diagnostic);
		}
	}

	/// <summary>
	/// Converts Find to repository pattern.
	/// </summary>
	private static async Task<Document> ConvertFindToRepositoryAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var rewriter = new FindToRepositoryRewriter();
		var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
		editor.ReplaceNode(invocationExpression, newExpression);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts Add to repository pattern.
	/// </summary>
	private static async Task<Document> ConvertAddToRepositoryAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var rewriter = new AddToRepositoryRewriter();
		var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
		editor.ReplaceNode(invocationExpression, newExpression);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts Update to repository pattern.
	/// </summary>
	private static async Task<Document> ConvertUpdateToRepositoryAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var rewriter = new UpdateToRepositoryRewriter();
		var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
		editor.ReplaceNode(invocationExpression, newExpression);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts Remove to repository pattern.
	/// </summary>
	private static async Task<Document> ConvertRemoveToRepositoryAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var rewriter = new RemoveToRepositoryRewriter();
		var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
		editor.ReplaceNode(invocationExpression, newExpression);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts ToList to repository pattern.
	/// </summary>
	private static async Task<Document> ConvertToListToRepositoryAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var rewriter = new ToListToRepositoryRewriter();
		var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
		editor.ReplaceNode(invocationExpression, newExpression);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Replaces direct data access with repository.
	/// </summary>
	private static async Task<Document> ReplaceDirectDataAccessWithRepositoryAsync(Document document, ObjectCreationExpressionSyntax objectCreation, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var rewriter = new DirectDataAccessToRepositoryRewriter();
		var newExpression = (ExpressionSyntax)rewriter.Visit(objectCreation);
		editor.ReplaceNode(objectCreation, newExpression);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Adds repository interface using directive.
	/// </summary>
	private static async Task<Document> AddRepositoryUsingAsync(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("YourNamespace.Repositories"));
		editor.InsertAfter(usingDirective, newUsing);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Rewriter that converts Find to repository pattern.
	/// </summary>
	private class FindToRepositoryRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
				(memberAccess.Name.Identifier.ValueText == "Find" || memberAccess.Name.Identifier.ValueText == "FindAsync"))
			{
				var methodName = memberAccess.Name.Identifier.ValueText;
				var arguments = node.ArgumentList?.Arguments;
				
				var args = arguments;
				if (args != null && args.Value.Count > 0)
				{
					return SyntaxFactory.InvocationExpression(
						SyntaxFactory.MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							SyntaxFactory.IdentifierName("_repository"),
							SyntaxFactory.IdentifierName(methodName)),
						SyntaxFactory.ArgumentList(
							SyntaxFactory.SeparatedList<ArgumentSyntax>(args.Value)));
				}
			}

			return base.VisitInvocationExpression(node);
		}
	}

	/// <summary>
	/// Rewriter that converts Add to repository pattern.
	/// </summary>
	private class AddToRepositoryRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
				(memberAccess.Name.Identifier.ValueText == "Add" || memberAccess.Name.Identifier.ValueText == "AddAsync"))
			{
				var methodName = memberAccess.Name.Identifier.ValueText;
				var arguments = node.ArgumentList?.Arguments;
				
				var args = arguments;
				if (args != null && args.Value.Count > 0)
				{
					return SyntaxFactory.InvocationExpression(
						SyntaxFactory.MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							SyntaxFactory.IdentifierName("_repository"),
							SyntaxFactory.IdentifierName(methodName)),
						SyntaxFactory.ArgumentList(
							SyntaxFactory.SeparatedList<ArgumentSyntax>(args.Value)));
				}
			}

			return base.VisitInvocationExpression(node);
		}
	}

	/// <summary>
	/// Rewriter that converts Update to repository pattern.
	/// </summary>
	private class UpdateToRepositoryRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
				(memberAccess.Name.Identifier.ValueText == "Update" || memberAccess.Name.Identifier.ValueText == "UpdateAsync"))
			{
				var methodName = memberAccess.Name.Identifier.ValueText;
				var arguments = node.ArgumentList?.Arguments;
				
				var args = arguments;
				if (args != null && args.Value.Count > 0)
				{
					return SyntaxFactory.InvocationExpression(
						SyntaxFactory.MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							SyntaxFactory.IdentifierName("_repository"),
							SyntaxFactory.IdentifierName(methodName)),
						SyntaxFactory.ArgumentList(
							SyntaxFactory.SeparatedList<ArgumentSyntax>(args.Value)));
				}
			}

			return base.VisitInvocationExpression(node);
		}
	}

	/// <summary>
	/// Rewriter that converts Remove to repository pattern.
	/// </summary>
	private class RemoveToRepositoryRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
				(memberAccess.Name.Identifier.ValueText == "Remove" || memberAccess.Name.Identifier.ValueText == "RemoveAsync"))
			{
				var methodName = memberAccess.Name.Identifier.ValueText;
				var arguments = node.ArgumentList?.Arguments;
				
				var args = arguments;
				if (args != null && args.Value.Count > 0)
				{
					return SyntaxFactory.InvocationExpression(
						SyntaxFactory.MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							SyntaxFactory.IdentifierName("_repository"),
							SyntaxFactory.IdentifierName(methodName)),
						SyntaxFactory.ArgumentList(
							SyntaxFactory.SeparatedList<ArgumentSyntax>(args.Value)));
				}
			}

			return base.VisitInvocationExpression(node);
		}
	}

	/// <summary>
	/// Rewriter that converts ToList to repository pattern.
	/// </summary>
	private class ToListToRepositoryRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
				(memberAccess.Name.Identifier.ValueText == "ToList" || memberAccess.Name.Identifier.ValueText == "ToListAsync"))
			{
				var methodName = memberAccess.Name.Identifier.ValueText;
				
				return SyntaxFactory.InvocationExpression(
					SyntaxFactory.MemberAccessExpression(
						SyntaxKind.SimpleMemberAccessExpression,
						SyntaxFactory.IdentifierName("_repository"),
						SyntaxFactory.IdentifierName(methodName)),
					SyntaxFactory.ArgumentList());
			}

			return base.VisitInvocationExpression(node);
		}
	}

	/// <summary>
	/// Rewriter that converts direct data access to repository pattern.
	/// </summary>
	private class DirectDataAccessToRepositoryRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
		{
			var typeName = node.Type.ToString();
			if (typeName.Contains("DbContext") || typeName.Contains("SqlConnection"))
			{
				// Replace direct data access with repository injection
				return SyntaxFactory.IdentifierName("_repository");
			}

			return base.VisitObjectCreationExpression(node);
		}
	}
}
