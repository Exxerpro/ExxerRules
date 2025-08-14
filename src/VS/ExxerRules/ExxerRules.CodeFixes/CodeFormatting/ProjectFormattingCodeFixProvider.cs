using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.IO;
using ExxerRules.Analyzers;
using ExxerRules.CodeFixes.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace ExxerRules.CodeFixes.CodeFormatting;

/// <summary>
/// Code fix provider that provides project-level formatting actions.
/// SRP: Responsible only for providing code fix actions for project-wide formatting issues.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ProjectFormattingCodeFixProvider)), Shared]
public class ProjectFormattingCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public sealed override ImmutableArray<string> FixableDiagnosticIds =>
		ImmutableArray.Create(DiagnosticIds.ProjectFormattingIssue);

	/// <inheritdoc/>
	public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	/// <inheritdoc/>
	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
		if (root == null)
		{
			return;
		}

		// Find the diagnostic for project formatting issues
		var diagnostic = context.Diagnostics.FirstOrDefault(d => d.Id == DiagnosticIds.ProjectFormattingIssue);
		if (diagnostic == null)
		{
			return;
		}

		// Register project formatting actions
		RegisterProjectFormattingActions(context, diagnostic);
	}

	/// <summary>
	/// Registers project-level formatting actions.
	/// </summary>
	/// <param name="context">The code fix context.</param>
	/// <param name="diagnostic">The diagnostic to fix.</param>
	private static void RegisterProjectFormattingActions(CodeFixContext context, Diagnostic diagnostic)
	{
		// Format entire project
		var formatProjectAction = CodeAction.Create(
			title: "📁 Format Entire Project",
			createChangedDocument: c => RoslynFormattingService.FormatProjectAsync(context.Document.Project, c),
			equivalenceKey: "FormatEntireProject");

		context.RegisterCodeFix(formatProjectAction, diagnostic);

		// Format project with .NET standards
		var formatProjectDotNetAction = CodeAction.Create(
			title: "🔧 Format Project with .NET Standards",
			createChangedDocument: c => RoslynFormattingService.FormatProjectAsync(
				context.Document.Project, 
				RoslynFormattingService.CreateDotNetFormattingOptions(), 
				c),
			equivalenceKey: "FormatProjectDotNet");

		context.RegisterCodeFix(formatProjectDotNetAction, diagnostic);

		// Format solution
		var formatSolutionAction = CodeAction.Create(
			title: "🏗️ Format Entire Solution",
			createChangedDocument: c => RoslynFormattingService.FormatSolutionAsync(context.Document.Project.Solution, c),
			equivalenceKey: "FormatEntireSolution");

		context.RegisterCodeFix(formatSolutionAction, diagnostic);
	}
}
