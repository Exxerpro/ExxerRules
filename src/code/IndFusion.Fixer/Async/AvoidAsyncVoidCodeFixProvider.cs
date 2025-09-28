using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace IndFusion.CodeFixes.Async;

/// <summary>
/// Code fix provider that converts async void methods to async Task.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AvoidAsyncVoidCodeFixProvider)), Shared]
public class AvoidAsyncVoidCodeFixProvider : CodeFixProvider
{
	/// <summary>
	/// Gets the diagnostic IDs that this code fix can address.
	/// </summary>
	public override ImmutableArray<string> FixableDiagnosticIds =>
		ImmutableArray.Create(IndFusion.Analyzers.DiagnosticIds.AvoidAsyncVoid);

	/// <summary>
	/// Gets the provider used to implement Fix All operations for this code fix.
	/// </summary>
	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	/// <summary>
	/// Registers code fixes for the specified diagnostics in the given context.
	/// Converts <c>async void</c> methods to <c>async Task</c>.
	/// </summary>
	/// <param name="context">The context for code fix registration.</param>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
		if (root == null)
		{
			return;
		}

		foreach (var diagnostic in context.Diagnostics)
		{
			var diagnosticSpan = diagnostic.Location.SourceSpan;
			var node = root.FindNode(diagnosticSpan).FirstAncestorOrSelf<MethodDeclarationSyntax>();
			if (node == null)
			{
				continue;
			}

			context.RegisterCodeFix(
				CodeAction.Create(
					title: $"Convert '{node.Identifier.ValueText}' to async Task",
					createChangedDocument: c => ConvertAsyncVoidToTaskAsync(context.Document, node, c),
					equivalenceKey: "ConvertAsyncVoidToTask"),
				diagnostic);
		}
	}

	private static async Task<Document> ConvertAsyncVoidToTaskAsync(Document document, MethodDeclarationSyntax method, CancellationToken ct)
	{
		var editor = await DocumentEditor.CreateAsync(document, ct).ConfigureAwait(false);
		var newMethod = method.WithReturnType(SyntaxFactory.ParseTypeName("Task"));
		editor.ReplaceNode(method, newMethod);
		return editor.GetChangedDocument();
	}
}
