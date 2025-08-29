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

namespace IndFusion.CodeFixes.ModernCSharp;

/// <summary>
/// Code fix provider that converts methods and properties to expression-bodied members.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExpressionBodiedMembersCodeFixProvider)), Shared]
public class ExpressionBodiedMembersCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override sealed ImmutableArray<string> FixableDiagnosticIds => 
		ImmutableArray.Create(DiagnosticIds.UseExpressionBodiedMembers);

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
			RegisterExpressionBodiedFixes(context, diagnostic, node);
		}
	}

	/// <summary>
	/// Registers code fix options based on the type of member that can be converted.
	/// </summary>
	/// <param name="context">The code fix context.</param>
	/// <param name="diagnostic">The diagnostic to fix.</param>
	/// <param name="node">The syntax node that can be converted.</param>
	private static void RegisterExpressionBodiedFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
	{
		switch (node)
		{
			case MethodDeclarationSyntax methodDeclaration:
				RegisterMethodExpressionBodiedFix(context, diagnostic, methodDeclaration);
				break;
			case PropertyDeclarationSyntax propertyDeclaration:
				RegisterPropertyExpressionBodiedFix(context, diagnostic, propertyDeclaration);
				break;
			case ConstructorDeclarationSyntax constructorDeclaration:
				RegisterConstructorExpressionBodiedFix(context, diagnostic, constructorDeclaration);
				break;
			case OperatorDeclarationSyntax operatorDeclaration:
				RegisterOperatorExpressionBodiedFix(context, diagnostic, operatorDeclaration);
				break;
			case ConversionOperatorDeclarationSyntax conversionOperatorDeclaration:
				RegisterConversionOperatorExpressionBodiedFix(context, diagnostic, conversionOperatorDeclaration);
				break;
			case IndexerDeclarationSyntax indexerDeclaration:
				RegisterIndexerExpressionBodiedFix(context, diagnostic, indexerDeclaration);
				break;
		}
	}

	/// <summary>
	/// Registers code fix for converting a method to expression-bodied member.
	/// </summary>
	private static void RegisterMethodExpressionBodiedFix(CodeFixContext context, Diagnostic diagnostic, MethodDeclarationSyntax methodDeclaration)
	{
		if (CanConvertMethodToExpressionBodied(methodDeclaration))
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: $"⚡ Convert method '{methodDeclaration.Identifier.ValueText}' to expression-bodied member",
					createChangedDocument: c => ConvertMethodToExpressionBodiedAsync(context.Document, methodDeclaration, c),
					equivalenceKey: $"ConvertMethodToExpressionBodied_{methodDeclaration.Identifier.ValueText}"),
				diagnostic);
		}
	}

	/// <summary>
	/// Registers code fix for converting a property to expression-bodied member.
	/// </summary>
	private static void RegisterPropertyExpressionBodiedFix(CodeFixContext context, Diagnostic diagnostic, PropertyDeclarationSyntax propertyDeclaration)
	{
		if (CanConvertPropertyToExpressionBodied(propertyDeclaration))
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: $"⚡ Convert property '{propertyDeclaration.Identifier.ValueText}' to expression-bodied member",
					createChangedDocument: c => ConvertPropertyToExpressionBodiedAsync(context.Document, propertyDeclaration, c),
					equivalenceKey: $"ConvertPropertyToExpressionBodied_{propertyDeclaration.Identifier.ValueText}"),
				diagnostic);
		}
	}

	/// <summary>
	/// Registers code fix for converting a constructor to expression-bodied member.
	/// </summary>
	private static void RegisterConstructorExpressionBodiedFix(CodeFixContext context, Diagnostic diagnostic, ConstructorDeclarationSyntax constructorDeclaration)
	{
		if (CanConvertConstructorToExpressionBodied(constructorDeclaration))
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "⚡ Convert constructor to expression-bodied member",
					createChangedDocument: c => ConvertConstructorToExpressionBodiedAsync(context.Document, constructorDeclaration, c),
					equivalenceKey: "ConvertConstructorToExpressionBodied"),
				diagnostic);
		}
	}

	/// <summary>
	/// Registers code fix for converting an operator to expression-bodied member.
	/// </summary>
	private static void RegisterOperatorExpressionBodiedFix(CodeFixContext context, Diagnostic diagnostic, OperatorDeclarationSyntax operatorDeclaration)
	{
		if (CanConvertOperatorToExpressionBodied(operatorDeclaration))
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: $"⚡ Convert operator '{operatorDeclaration.OperatorToken.ValueText}' to expression-bodied member",
					createChangedDocument: c => ConvertOperatorToExpressionBodiedAsync(context.Document, operatorDeclaration, c),
					equivalenceKey: $"ConvertOperatorToExpressionBodied_{operatorDeclaration.OperatorToken.ValueText}"),
				diagnostic);
		}
	}

	/// <summary>
	/// Registers code fix for converting a conversion operator to expression-bodied member.
	/// </summary>
	private static void RegisterConversionOperatorExpressionBodiedFix(CodeFixContext context, Diagnostic diagnostic, ConversionOperatorDeclarationSyntax conversionOperatorDeclaration)
	{
		if (CanConvertConversionOperatorToExpressionBodied(conversionOperatorDeclaration))
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "⚡ Convert conversion operator to expression-bodied member",
					createChangedDocument: c => ConvertConversionOperatorToExpressionBodiedAsync(context.Document, conversionOperatorDeclaration, c),
					equivalenceKey: "ConvertConversionOperatorToExpressionBodied"),
				diagnostic);
		}
	}

	/// <summary>
	/// Registers code fix for converting an indexer to expression-bodied member.
	/// </summary>
	private static void RegisterIndexerExpressionBodiedFix(CodeFixContext context, Diagnostic diagnostic, IndexerDeclarationSyntax indexerDeclaration)
	{
		if (CanConvertIndexerToExpressionBodied(indexerDeclaration))
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "⚡ Convert indexer to expression-bodied member",
					createChangedDocument: c => ConvertIndexerToExpressionBodiedAsync(context.Document, indexerDeclaration, c),
					equivalenceKey: "ConvertIndexerToExpressionBodied"),
				diagnostic);
		}
	}

	/// <summary>
	/// Converts a method to expression-bodied member.
	/// </summary>
	private static async Task<Document> ConvertMethodToExpressionBodiedAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var expression = ExtractReturnExpression(methodDeclaration.Body);
		if (expression != null)
		{
			var newMethod = methodDeclaration
				.WithBody(null)
				.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expression))
				.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

			editor.ReplaceNode(methodDeclaration, newMethod);
		}

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts a property to expression-bodied member.
	/// </summary>
	private static async Task<Document> ConvertPropertyToExpressionBodiedAsync(Document document, PropertyDeclarationSyntax propertyDeclaration, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var expression = ExtractPropertyExpression(propertyDeclaration);
		if (expression != null)
		{
			var newProperty = propertyDeclaration
				.WithAccessorList(null)
				.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expression))
				.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

			editor.ReplaceNode(propertyDeclaration, newProperty);
		}

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts a constructor to expression-bodied member.
	/// </summary>
	private static async Task<Document> ConvertConstructorToExpressionBodiedAsync(Document document, ConstructorDeclarationSyntax constructorDeclaration, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var expression = ExtractConstructorExpression(constructorDeclaration);
		if (expression != null)
		{
			var newConstructor = constructorDeclaration
				.WithBody(null)
				.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expression))
				.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

			editor.ReplaceNode(constructorDeclaration, newConstructor);
		}

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts an operator to expression-bodied member.
	/// </summary>
	private static async Task<Document> ConvertOperatorToExpressionBodiedAsync(Document document, OperatorDeclarationSyntax operatorDeclaration, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var expression = ExtractReturnExpression(operatorDeclaration.Body);
		if (expression != null)
		{
			var newOperator = operatorDeclaration
				.WithBody(null)
				.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expression))
				.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

			editor.ReplaceNode(operatorDeclaration, newOperator);
		}

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts a conversion operator to expression-bodied member.
	/// </summary>
	private static async Task<Document> ConvertConversionOperatorToExpressionBodiedAsync(Document document, ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var expression = ExtractReturnExpression(conversionOperatorDeclaration.Body);
		if (expression != null)
		{
			var newConversionOperator = conversionOperatorDeclaration
				.WithBody(null)
				.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expression))
				.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

			editor.ReplaceNode(conversionOperatorDeclaration, newConversionOperator);
		}

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts an indexer to expression-bodied member.
	/// </summary>
	private static async Task<Document> ConvertIndexerToExpressionBodiedAsync(Document document, IndexerDeclarationSyntax indexerDeclaration, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		var expression = ExtractIndexerExpression(indexerDeclaration);
		if (expression != null)
		{
			var newIndexer = indexerDeclaration
				.WithAccessorList(null)
				.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expression))
				.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

			editor.ReplaceNode(indexerDeclaration, newIndexer);
		}

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Checks if a method can be converted to expression-bodied member.
	/// </summary>
	private static bool CanConvertMethodToExpressionBodied(MethodDeclarationSyntax methodDeclaration)
	{
		return methodDeclaration.Body != null &&
			   methodDeclaration.Body.Statements.Count == 1 &&
			   methodDeclaration.Body.Statements[0] is ReturnStatementSyntax returnStatement &&
			   returnStatement.Expression != null;
	}

	/// <summary>
	/// Checks if a property can be converted to expression-bodied member.
	/// </summary>
	private static bool CanConvertPropertyToExpressionBodied(PropertyDeclarationSyntax propertyDeclaration)
	{
		return propertyDeclaration.AccessorList != null &&
			   propertyDeclaration.AccessorList.Accessors.Count == 1 &&
			   propertyDeclaration.AccessorList.Accessors[0].Kind() == SyntaxKind.GetAccessorDeclaration &&
			   propertyDeclaration.AccessorList.Accessors[0].Body != null &&
			   propertyDeclaration.AccessorList.Accessors[0].Body.Statements.Count == 1 &&
			   propertyDeclaration.AccessorList.Accessors[0].Body.Statements[0] is ReturnStatementSyntax returnStatement &&
			   returnStatement.Expression != null;
	}

	/// <summary>
	/// Checks if a constructor can be converted to expression-bodied member.
	/// </summary>
	private static bool CanConvertConstructorToExpressionBodied(ConstructorDeclarationSyntax constructorDeclaration)
	{
		return constructorDeclaration.Body != null &&
			   constructorDeclaration.Body.Statements.Count == 1 &&
			   constructorDeclaration.Body.Statements[0] is ExpressionStatementSyntax expressionStatement &&
			   expressionStatement.Expression is InvocationExpressionSyntax invocationExpression &&
			   invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess &&
			   memberAccess.Name.Identifier.ValueText == "this";
	}

	/// <summary>
	/// Checks if an operator can be converted to expression-bodied member.
	/// </summary>
	private static bool CanConvertOperatorToExpressionBodied(OperatorDeclarationSyntax operatorDeclaration)
	{
		return operatorDeclaration.Body != null &&
			   operatorDeclaration.Body.Statements.Count == 1 &&
			   operatorDeclaration.Body.Statements[0] is ReturnStatementSyntax returnStatement &&
			   returnStatement.Expression != null;
	}

	/// <summary>
	/// Checks if a conversion operator can be converted to expression-bodied member.
	/// </summary>
	private static bool CanConvertConversionOperatorToExpressionBodied(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration)
	{
		return conversionOperatorDeclaration.Body != null &&
			   conversionOperatorDeclaration.Body.Statements.Count == 1 &&
			   conversionOperatorDeclaration.Body.Statements[0] is ReturnStatementSyntax returnStatement &&
			   returnStatement.Expression != null;
	}

	/// <summary>
	/// Checks if an indexer can be converted to expression-bodied member.
	/// </summary>
	private static bool CanConvertIndexerToExpressionBodied(IndexerDeclarationSyntax indexerDeclaration)
	{
		return indexerDeclaration.AccessorList != null &&
			   indexerDeclaration.AccessorList.Accessors.Count == 1 &&
			   indexerDeclaration.AccessorList.Accessors[0].Kind() == SyntaxKind.GetAccessorDeclaration &&
			   indexerDeclaration.AccessorList.Accessors[0].Body != null &&
			   indexerDeclaration.AccessorList.Accessors[0].Body.Statements.Count == 1 &&
			   indexerDeclaration.AccessorList.Accessors[0].Body.Statements[0] is ReturnStatementSyntax returnStatement &&
			   returnStatement.Expression != null;
	}

	/// <summary>
	/// Extracts the return expression from a method body.
	/// </summary>
	private static ExpressionSyntax? ExtractReturnExpression(BlockSyntax? body)
	{
		if (body?.Statements.Count == 1 && body.Statements[0] is ReturnStatementSyntax returnStatement)
		{
			return returnStatement.Expression;
		}
		return null;
	}

	/// <summary>
	/// Extracts the expression from a property accessor.
	/// </summary>
	private static ExpressionSyntax? ExtractPropertyExpression(PropertyDeclarationSyntax propertyDeclaration)
	{
		var getter = propertyDeclaration.AccessorList?.Accessors.FirstOrDefault(a => a.Kind() == SyntaxKind.GetAccessorDeclaration);
		if (getter?.Body?.Statements.Count == 1 && getter.Body.Statements[0] is ReturnStatementSyntax returnStatement)
		{
			return returnStatement.Expression;
		}
		return null;
	}

	/// <summary>
	/// Extracts the expression from a constructor body.
	/// </summary>
	private static ExpressionSyntax? ExtractConstructorExpression(ConstructorDeclarationSyntax constructorDeclaration)
	{
		if (constructorDeclaration.Body?.Statements.Count == 1 && 
			constructorDeclaration.Body.Statements[0] is ExpressionStatementSyntax expressionStatement)
		{
			return expressionStatement.Expression;
		}
		return null;
	}

	/// <summary>
	/// Extracts the expression from an indexer accessor.
	/// </summary>
	private static ExpressionSyntax? ExtractIndexerExpression(IndexerDeclarationSyntax indexerDeclaration)
	{
		var getter = indexerDeclaration.AccessorList?.Accessors.FirstOrDefault(a => a.Kind() == SyntaxKind.GetAccessorDeclaration);
		if (getter?.Body?.Statements.Count == 1 && getter.Body.Statements[0] is ReturnStatementSyntax returnStatement)
		{
			return returnStatement.Expression;
		}
		return null;
	}
}
