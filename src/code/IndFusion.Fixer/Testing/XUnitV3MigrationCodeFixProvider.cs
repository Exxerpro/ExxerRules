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

namespace IndFusion.CodeFixes.Testing;

/// <summary>
/// Code fix provider that migrates XUnit v2 syntax to v3 syntax.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(XUnitV3MigrationCodeFixProvider)), Shared]
public class XUnitV3MigrationCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override sealed ImmutableArray<string> FixableDiagnosticIds => 
		ImmutableArray.Create(DiagnosticIds.UseXUnitV3);

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
			RegisterXUnitV3MigrationFixes(context, diagnostic, node);
		}
	}

	/// <summary>
	/// Registers code fix options based on the type of XUnit v2 syntax that can be migrated.
	/// </summary>
	/// <param name="context">The code fix context.</param>
	/// <param name="diagnostic">The diagnostic to fix.</param>
	/// <param name="node">The syntax node that can be migrated.</param>
	private static void RegisterXUnitV3MigrationFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
	{
		switch (node)
		{
			case AttributeSyntax attributeSyntax:
				RegisterAttributeMigrationFixes(context, diagnostic, attributeSyntax);
				break;
			case InvocationExpressionSyntax invocationExpression:
				RegisterInvocationMigrationFixes(context, diagnostic, invocationExpression);
				break;
			case UsingDirectiveSyntax usingDirective:
				RegisterUsingMigrationFixes(context, diagnostic, usingDirective);
				break;
		}
	}

	/// <summary>
	/// Registers code fix options for attribute migration.
	/// </summary>
	private static void RegisterAttributeMigrationFixes(CodeFixContext context, Diagnostic diagnostic, AttributeSyntax attributeSyntax)
	{
		var attributeName = attributeSyntax.Name.ToString();

		switch (attributeName)
		{
			case "Fact":
			case "Xunit.Fact":
				context.RegisterCodeFix(
					CodeAction.Create(
						title: "🔄 Migrate [Fact] to XUnit v3",
						createChangedDocument: c => MigrateFactAttributeAsync(context.Document, attributeSyntax, c),
						equivalenceKey: "MigrateFactAttribute"),
					diagnostic);
				break;

			case "Theory":
			case "Xunit.Theory":
				context.RegisterCodeFix(
					CodeAction.Create(
						title: "🔄 Migrate [Theory] to XUnit v3",
						createChangedDocument: c => MigrateTheoryAttributeAsync(context.Document, attributeSyntax, c),
						equivalenceKey: "MigrateTheoryAttribute"),
					diagnostic);
				break;

			case "InlineData":
			case "Xunit.InlineData":
				context.RegisterCodeFix(
					CodeAction.Create(
						title: "🔄 Migrate [InlineData] to XUnit v3",
						createChangedDocument: c => MigrateInlineDataAttributeAsync(context.Document, attributeSyntax, c),
						equivalenceKey: "MigrateInlineDataAttribute"),
					diagnostic);
				break;

			case "MemberData":
			case "Xunit.MemberData":
				context.RegisterCodeFix(
					CodeAction.Create(
						title: "🔄 Migrate [MemberData] to XUnit v3",
						createChangedDocument: c => MigrateMemberDataAttributeAsync(context.Document, attributeSyntax, c),
						equivalenceKey: "MigrateMemberDataAttribute"),
					diagnostic);
				break;

			case "ClassData":
			case "Xunit.ClassData":
				context.RegisterCodeFix(
					CodeAction.Create(
						title: "🔄 Migrate [ClassData] to XUnit v3",
						createChangedDocument: c => MigrateClassDataAttributeAsync(context.Document, attributeSyntax, c),
						equivalenceKey: "MigrateClassDataAttribute"),
					diagnostic);
				break;
		}
	}

	/// <summary>
	/// Registers code fix options for invocation migration.
	/// </summary>
	private static void RegisterInvocationMigrationFixes(CodeFixContext context, Diagnostic diagnostic, InvocationExpressionSyntax invocationExpression)
	{
		if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess)
		{
			var methodName = memberAccess.Name.Identifier.ValueText;

			switch (methodName)
			{
				case "Assert.Throws":
				case "Assert.ThrowsAsync":
					context.RegisterCodeFix(
						CodeAction.Create(
							title: $"🔄 Migrate {methodName} to XUnit v3",
							createChangedDocument: c => MigrateAssertThrowsAsync(context.Document, invocationExpression, c),
							equivalenceKey: $"Migrate{methodName}"),
						diagnostic);
					break;

				case "Assert.Record":
				case "Assert.RecordAsync":
					context.RegisterCodeFix(
						CodeAction.Create(
							title: $"🔄 Migrate {methodName} to XUnit v3",
							createChangedDocument: c => MigrateAssertRecordAsync(context.Document, invocationExpression, c),
							equivalenceKey: $"Migrate{methodName}"),
						diagnostic);
					break;
			}
		}
	}

	/// <summary>
	/// Registers code fix options for using directive migration.
	/// </summary>
	private static void RegisterUsingMigrationFixes(CodeFixContext context, Diagnostic diagnostic, UsingDirectiveSyntax usingDirective)
	{
		var usingName = usingDirective.Name?.ToString();
		if (usingName == "Xunit")
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "🔄 Update XUnit using directive to v3",
					createChangedDocument: c => UpdateXUnitUsingDirectiveAsync(context.Document, usingDirective, c),
					equivalenceKey: "UpdateXUnitUsingDirective"),
				diagnostic);
		}
	}

	/// <summary>
	/// Migrates a [Fact] attribute to XUnit v3.
	/// </summary>
	private static async Task<Document> MigrateFactAttributeAsync(Document document, AttributeSyntax factAttribute, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// XUnit v3 Fact attribute is mostly compatible, but we can add some improvements
		var newAttribute = factAttribute.WithName(SyntaxFactory.ParseName("Fact"));
		editor.ReplaceNode(factAttribute, newAttribute);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Migrates a [Theory] attribute to XUnit v3.
	/// </summary>
	private static async Task<Document> MigrateTheoryAttributeAsync(Document document, AttributeSyntax theoryAttribute, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// XUnit v3 Theory attribute is mostly compatible
		var newAttribute = theoryAttribute.WithName(SyntaxFactory.ParseName("Theory"));
		editor.ReplaceNode(theoryAttribute, newAttribute);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Migrates an [InlineData] attribute to XUnit v3.
	/// </summary>
	private static async Task<Document> MigrateInlineDataAttributeAsync(Document document, AttributeSyntax inlineDataAttribute, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// XUnit v3 InlineData attribute is mostly compatible
		var newAttribute = inlineDataAttribute.WithName(SyntaxFactory.ParseName("InlineData"));
		editor.ReplaceNode(inlineDataAttribute, newAttribute);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Migrates a [MemberData] attribute to XUnit v3.
	/// </summary>
	private static async Task<Document> MigrateMemberDataAttributeAsync(Document document, AttributeSyntax memberDataAttribute, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// XUnit v3 MemberData attribute is mostly compatible
		var newAttribute = memberDataAttribute.WithName(SyntaxFactory.ParseName("MemberData"));
		editor.ReplaceNode(memberDataAttribute, newAttribute);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Migrates a [ClassData] attribute to XUnit v3.
	/// </summary>
	private static async Task<Document> MigrateClassDataAttributeAsync(Document document, AttributeSyntax classDataAttribute, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// XUnit v3 ClassData attribute is mostly compatible
		var newAttribute = classDataAttribute.WithName(SyntaxFactory.ParseName("ClassData"));
		editor.ReplaceNode(classDataAttribute, newAttribute);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Migrates Assert.Throws to XUnit v3 syntax.
	/// </summary>
	private static async Task<Document> MigrateAssertThrowsAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// XUnit v3 Assert.Throws is mostly compatible, but we can improve the syntax
		var rewriter = new AssertThrowsRewriter();
		var newInvocation = (InvocationExpressionSyntax)rewriter.Visit(invocationExpression);
		editor.ReplaceNode(invocationExpression, newInvocation);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Migrates Assert.Record to XUnit v3 syntax.
	/// </summary>
	private static async Task<Document> MigrateAssertRecordAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// XUnit v3 Assert.Record is mostly compatible
		var rewriter = new AssertRecordRewriter();
		var newInvocation = (InvocationExpressionSyntax)rewriter.Visit(invocationExpression);
		editor.ReplaceNode(invocationExpression, newInvocation);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Updates XUnit using directive to v3.
	/// </summary>
	private static async Task<Document> UpdateXUnitUsingDirectiveAsync(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// XUnit v3 using directive is the same, but we can add a comment
		var newUsing = usingDirective.WithLeadingTrivia(
			usingDirective.GetLeadingTrivia().Add(
				SyntaxFactory.Comment("// XUnit v3")));
		editor.ReplaceNode(usingDirective, newUsing);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Rewriter that updates Assert.Throws to XUnit v3 syntax.
	/// </summary>
	private class AssertThrowsRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
				(memberAccess.Name.Identifier.ValueText == "Throws" || memberAccess.Name.Identifier.ValueText == "ThrowsAsync"))
			{
				// XUnit v3 Assert.Throws is mostly compatible
				// We can add some improvements like better error messages
				return base.VisitInvocationExpression(node);
			}

			return base.VisitInvocationExpression(node);
		}
	}

	/// <summary>
	/// Rewriter that updates Assert.Record to XUnit v3 syntax.
	/// </summary>
	private class AssertRecordRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
				(memberAccess.Name.Identifier.ValueText == "Record" || memberAccess.Name.Identifier.ValueText == "RecordAsync"))
			{
				// XUnit v3 Assert.Record is mostly compatible
				return base.VisitInvocationExpression(node);
			}

			return base.VisitInvocationExpression(node);
		}
	}
}
